﻿using System.Collections.Generic;
using uComponents.Core;
using umbraco.MacroEngines;

namespace uComponents.DataTypes.RazorDataTypeModels.CountryPicker
{
	/// <summary>
	/// Model binder for the Country Picker data-type.
	/// </summary>
	[RazorDataTypeModel(Constants.DataTypes.CountryPickerId)]
	public class CountryPickerModelBinder : IRazorDataTypeModel
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
				instance = PropertyData;
				return true;
			}

			var values = new List<string>();

			if (!string.IsNullOrEmpty(PropertyData))
			{
				foreach (var value in PropertyData.Split(Constants.Common.COMMA))
				{
					values.Add(value);
				}
			}

			if (values.Count == 1)
			{
				instance = values[0];
			}
			else
			{
				instance = values;
			}

			return true;
		}
	}
}