﻿using System.Collections.Generic;
using System.Xml;
using uComponents.Core;
using umbraco.MacroEngines;

namespace uComponents.DataTypes.RazorDataTypeModels.TextstringArray
{
	/// <summary>
	/// Model binder for the TextstringArray data-type.
	/// </summary>
	[RazorDataTypeModel(Constants.DataTypes.TextstringArrayId)]
	public class TextstringArrayModelBinder : IRazorDataTypeModel
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
			if (!Settings.RazorModelBindingEnabled)
			{
#pragma warning disable 0618
				instance = new DynamicXml(PropertyData);
#pragma warning restore 0618
				return true;
			}

			var values = new List<string[]>();

			if (!string.IsNullOrEmpty(PropertyData))
			{
				var xml = new XmlDocument();
				xml.LoadXml(PropertyData);

				foreach (XmlNode node in xml.SelectNodes("/TextstringArray/values"))
				{
					var value = new List<string>();
					foreach (XmlNode child in node.SelectNodes("value"))
					{
						value.Add(child.InnerText);
					}

					values.Add(value.ToArray());
				}
			}

			instance = values;

			return true;
		}
	}
}