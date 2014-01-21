﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using uComponents.Core;
using uComponents.DataTypes.Shared.Models;
using umbraco.MacroEngines;

namespace uComponents.DataTypes.RazorDataTypeModels.SqlAutoComplete
{
	/// <summary>
	/// Model binder for the SqlAutoComplete data-type.
	/// </summary>
	[RazorDataTypeModel(Constants.DataTypes.SqlAutoCompleteId)]
	public class SqlAutoCompleteModelBinder : IRazorDataTypeModel
	{
		/// <summary>
		/// Inits the specified current node id.
		/// </summary>
		/// <param name="CurrentNodeId">The current node id.</param>
		/// <param name="PropertyData">The property data.</param>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		public bool Init(int CurrentNodeId, string PropertyData, out object instance)
		{
			// check if the data is XML
			if (Helper.Xml.CouldItBeXml(PropertyData))
			{
				// if model binding is disbaled, return a DynamicXml object
				if (!Settings.RazorModelBindingEnabled)
				{
#pragma warning disable 0618
					instance = new DynamicXml(PropertyData);
#pragma warning restore 0618
					return true;
				}

				// parse the XML
				var xdoc = XDocument.Parse(PropertyData);
				if (xdoc != null)
				{
					// select the Item nodes
					var nodes = xdoc.XPathSelectElements("//Item");
					if (nodes != null && nodes.Count() > 0)
					{
						// iterate over the nodes; building collection of AutoCompleteItem(s)
						var items = new List<AutoCompleteItem>();
						foreach (var node in nodes)
						{
							// check if the node has any attributes
							if (!node.HasAttributes)
								continue;

							var item = new AutoCompleteItem();

							// loop through the attributes, setting the properties accordingly
							foreach (var attribute in node.Attributes())
							{
								switch (attribute.Name.LocalName)
								{
									case "Text":
										item.Text = attribute.Value;
										break;

									case "Value":
										item.Value = attribute.Value;
										break;

									default:
										break;
								}
							}

							items.Add(item);
						}

						instance = items;
						return true;
					}
				}
			}

			// all else fails, return default value
			instance = PropertyData;
			return true;
		}
	}
}