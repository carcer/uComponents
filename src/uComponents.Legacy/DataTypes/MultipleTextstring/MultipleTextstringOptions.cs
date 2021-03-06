﻿using System;
using System.ComponentModel;

using uComponents.DataTypes.Shared.PrevalueEditors;
using umbraco.editorControls;

namespace uComponents.DataTypes.MultipleTextstring
{
	/// <summary>
	/// The options for the Multiple Textstring data-type.
	/// </summary>
	public class MultipleTextstringOptions : AbstractOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MultipleTextstringOptions"/> class.
		/// </summary>
		public MultipleTextstringOptions()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MultipleTextstringOptions"/> class.
		/// </summary>
		/// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
		public MultipleTextstringOptions(bool loadDefaults)
			: base(loadDefaults)
		{
		}

		/// <summary>
		/// Gets or sets the maximum.
		/// </summary>
		/// <value>The maximum.</value>
		[DefaultValue(-1)]
		public int Maximum { get; set; }

		/// <summary>
		/// Gets or sets the minimum.
		/// </summary>
		/// <value>The minimum.</value>
		[DefaultValue(1)]
		public int Minimum { get; set; }
	}
}
