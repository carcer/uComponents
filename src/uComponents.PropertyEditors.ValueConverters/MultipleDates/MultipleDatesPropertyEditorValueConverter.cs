﻿using System;
using System.Collections.Generic;
using System.Linq;
using uComponents.DataTypes;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;

namespace uComponents.PropertyEditors.ValueConverters.MultipleDates
{
	/// <summary>
	/// Property-editor value converter for the Multiple Dates data-type.
	/// </summary>
	public class MultipleDatesPropertyEditorValueConverter : IPropertyEditorValueConverter
	{
		/// <summary>
		/// Returns true if this converter can perform the value conversion for the specified property editor id
		/// </summary>
		/// <param name="propertyEditorId"></param>
		/// <param name="docTypeAlias"></param>
		/// <param name="propertyTypeAlias"></param>
		/// <returns></returns>
		public bool IsConverterFor(Guid propertyEditorId, string docTypeAlias, string propertyTypeAlias)
		{
			return Guid.Parse(uComponents.Core.Constants.DataTypes.MultipleDatesId).Equals(propertyEditorId);
		}

		/// <summary>
		/// Attempts to convert the value specified into a useable value on the front-end
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Attempt<object> ConvertPropertyValue(object value)
		{
			if (value != null && value.ToString().Length > 0)
			{
				var dates = new List<DateTime>();
				var items = value.ToString().Split(uComponents.Core.Constants.Common.COMMA).Select(s => s.Trim());
				foreach (var item in items)
				{
					DateTime date;
					if (DateTime.TryParse(item, out date))
						dates.Add(date);
				}

				return new Attempt<object>(true, dates);
			}

			return Attempt<object>.False;
		}
	}
}