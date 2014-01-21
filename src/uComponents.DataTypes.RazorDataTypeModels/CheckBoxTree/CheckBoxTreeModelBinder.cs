﻿using System.Linq;
using uComponents.Core;
using umbraco;
using umbraco.MacroEngines;
using umbraco.MacroEngines.Library;

namespace uComponents.DataTypes.RazorDataTypeModels.CheckBoxTree
{
	/// <summary>
	/// Model binder for the CheckBoxTree data-type.
	/// </summary>
	[RazorDataTypeModel(Constants.DataTypes.CheckBoxTreeId)]
	public class CheckBoxTreeModelBinder : IRazorDataTypeModel
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
				if (Helper.Xml.CouldItBeXml(PropertyData))
				{
#pragma warning disable 0618
					instance = new DynamicXml(PropertyData);
#pragma warning restore 0618
					return true;
				}

				instance = PropertyData;
				return true;
			}

			var nodeIds = Helper.Xml.CouldItBeXml(PropertyData) ? uQuery.GetXmlIds(PropertyData) : uQuery.ConvertToIntArray(uQuery.GetCsvIds(PropertyData));
			var library = new RazorLibraryCore(null);

			instance = library.NodesById(nodeIds.ToList()) as DynamicNodeList;

			return true;
		}
	}
}