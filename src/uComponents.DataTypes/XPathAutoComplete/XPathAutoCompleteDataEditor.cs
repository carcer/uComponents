﻿using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using uComponents.Core;
using umbraco;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.property;
using umbraco.editorControls;
using umbraco.interfaces;

[assembly: WebResource("uComponents.DataTypes.XPathAutoComplete.XPathAutoComplete.css", Constants.MediaTypeNames.Text.Css)]
[assembly: WebResource("uComponents.DataTypes.XPathAutoComplete.XPathAutoComplete.js", Constants.MediaTypeNames.Application.JavaScript)]
namespace uComponents.DataTypes.XPathAutoComplete
{
    /// <summary>
    /// DataEditor for the XPath AutoComplete data-type.
    /// </summary>
    [ClientDependency.Core.ClientDependency(ClientDependency.Core.ClientDependencyType.Javascript, "ui/jqueryui.js", "UmbracoClient")]
    public class XPathAutoCompleteDataEditor : CompositeControl, IDataEditor
    {
        /// <summary>
        /// Field for the data.
        /// </summary>
        private IData data;

        /// <summary>
        /// Field for the options.
        /// </summary>
        private XPathAutoCompleteOptions options;

        /// <summary>
        /// TextBox to attach the js autocompete, using this ClientId we can walk up the dom to the wrapping div to find everything else
        /// </summary>
        private TextBox autoCompleteTextBox = new TextBox() { ID = "AutoCompleteTextBox" };

        /// <summary>
        /// Stores the selected values
        /// </summary>
        private HiddenField selectedItemsHiddenField = new HiddenField() { ID = "SelectedItems" };

        /// <summary>
        /// Ensure number of items selected is within any min and max configuration settings
        /// </summary>
        private CustomValidator customValidator = new CustomValidator();

        /// <summary>
        /// Initializes a new instance of XPathAutoCompleteDataEditor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="options"></param>
        internal XPathAutoCompleteDataEditor(IData data, XPathAutoCompleteOptions options)
        {
            this.data = data;
            this.options = options;
        }

        /// <summary>
        /// Gets the property / datatypedefinition id - used to identify the current instance
        /// </summary>
        private int DataTypeDefinitionId
        {
            get
            {
                return ((XmlData)this.data).DataTypeDefinitionId;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [treat as rich text editor].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [treat as rich text editor]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool TreatAsRichTextEditor
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [show label].
        /// </summary>
        /// <value><c>true</c> if [show label]; otherwise, <c>false</c>.</value>
        public virtual bool ShowLabel
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the editor.
        /// </summary>
        /// <value>The editor.</value>
        public Control Editor
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Gets the selected items hidden field.
        /// </summary>
        /// <value>The selected items hidden field.</value>
        public HiddenField SelectedItemsHiddenField
        {
            get
            {
                return this.selectedItemsHiddenField;
            }
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            // wrapping div
            HtmlGenericControl div = new HtmlGenericControl("div");

            // ul list for the selected items
            HtmlGenericControl ul = new HtmlGenericControl("ul");

            div.Attributes.Add("class", "xpath-auto-complete");
            div.Attributes.Add("data-xpath-autocomplete-id", DataTypeConstants.XPathAutoCompleteId);
            div.Attributes.Add("data-datatype-definition-id", this.DataTypeDefinitionId.ToString());
            div.Attributes.Add("data-current-id", uQuery.GetIdFromQueryString());
            div.Attributes.Add("data-type", this.options.Type);
            div.Attributes.Add("data-min-length", this.options.MinLength.ToString());
            //div.Attributes.Add("data-min-items", this.options.MinItems.ToString()); -- not required client side - TODO: could visually indicate number required ?
            div.Attributes.Add("data-max-items", this.options.MaxItems.ToString());
            div.Attributes.Add("data-allow-duplicates", this.options.AllowDuplicates.ToString());

            ul.Attributes.Add("class", "propertypane");

            this.autoCompleteTextBox.CssClass = "umbEditorTextField";

            div.Controls.Add(ul);
            div.Controls.Add(autoCompleteTextBox);
            div.Controls.Add(this.selectedItemsHiddenField);

            this.Controls.Add(div);
            this.Controls.Add(this.customValidator);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.EnsureChildControls();

            this.RegisterEmbeddedClientResource("uComponents.DataTypes.XPathAutoComplete.XPathAutoComplete.css", ClientDependency.Core.ClientDependencyType.Css);
            this.RegisterEmbeddedClientResource("uComponents.DataTypes.XPathAutoComplete.XPathAutoComplete.js", ClientDependency.Core.ClientDependencyType.Javascript);

            string startupScript = @"
                <script language='javascript' type='text/javascript'>
                    $(document).ready(function () {
                        XPathAutoComplete.init(jQuery('input#" + this.autoCompleteTextBox.ClientID + @"'));
                    });
                </script>";

            ScriptManager.RegisterStartupScript(this, typeof(XPathAutoCompleteDataEditor), this.ClientID + "_init", startupScript, false);

            if (!this.Page.IsPostBack && this.data.Value != null)
            {
                this.selectedItemsHiddenField.Value = this.data.Value.ToString();
            }

            // put the options obj into cache so that the /base method can request it (where the XPath query is being used)
            HttpContext.Current.Cache[string.Concat(DataTypeConstants.XPathAutoCompleteId, "_options_", this.DataTypeDefinitionId)] = this.options;
        }

        /// <summary>
        /// Called by Umbraco when saving the node
        /// </summary>
        public void Save()
        {
            var xml = this.selectedItemsHiddenField.Value;
            var items = 0; // to validate on the number of items selected

            // There should be a valid xml fragment (or empty) in the hidden field
            if (!string.IsNullOrWhiteSpace(xml))
            {
                try
                {
                    var xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(xml);

                    items = xmlDocument.SelectNodes("//Item").Count;
                }
                catch (XmlException)
                {
                    return;
                }
            }

            if (this.options.MinItems > 0 && items < this.options.MinItems)
            {
                customValidator.IsValid = false;
            }

            // fail safe check as UI shouldn't allow excess items to be selected (datatype configuration could have been changed since data was stored)
            if (this.options.MaxItems > 0 && items > this.options.MaxItems)
            {
                customValidator.IsValid = false;
            }

            if (!customValidator.IsValid)
            {
                var property = new Property(((XmlData)this.data).PropertyId);
                // property.PropertyType.Mandatory - IGNORE, always use the configuration parameters

                this.customValidator.ErrorMessage = ui.Text("errorHandling", "errorRegExpWithoutTab", property.PropertyType.Name, User.GetCurrent());
            }

            this.data.Value = xml;
        }
    }
}
