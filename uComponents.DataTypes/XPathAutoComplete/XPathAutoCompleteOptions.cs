﻿using System;
using System.ComponentModel;
using System.Configuration;
using umbraco;
using umbraco.editorControls;

namespace uComponents.DataTypes.XPathAutoComplete
{
    /// <summary>
    /// The options for the XPathAutoCompleteOptions data-type.
    /// </summary>
    public class XPathAutoCompleteOptions : AbstractOptions
    {
        /// <summary>
        /// Initializes an instance of XPathAutoCompleteOptions
        /// </summary>
        public XPathAutoCompleteOptions()
        {
        }

        public XPathAutoCompleteOptions(bool loadDefaults)
            : base(loadDefaults)
        {
        }

        [DefaultValue("c66ba18e-eaf3-4cff-8a22-41b16d66a972")]
        public string Type { get; set; }

        [DefaultValue("//*")]
        public string XPath { get; set; }

        [DefaultValue("")]
        public string Property { get; set; }

        [DefaultValue(1)]
        public int MinLength { get; set; }

        [DefaultValue(0)]
        public int MaxSuggestions { get; set; }

        [DefaultValue(0)]
        public int MinItems { get; set; }

        [DefaultValue(0)]
        public int MaxItems { get; set; }

        [DefaultValue(false)]
        public bool AllowDuplicates { get; set; }

        /// <summary>
        /// Helper to get the UmbracoObjectType from the stored string guid
        /// </summary>
        public uQuery.UmbracoObjectType UmbracoObjectType
        {
            get
            {
                return uQuery.GetUmbracoObjectType(new Guid(this.Type));
            }
        }
    }
}