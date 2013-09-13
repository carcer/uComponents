﻿using System.Web.UI;
using uComponents.Core;
using uComponents.DataTypes.Shared.Extensions;
using umbraco.cms.businesslogic.datatype;
using umbraco.editorControls;

[assembly: WebResource("uComponents.Legacy.DataTypes.MultiUrlPicker.MultiUrlPickerScripts.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.Legacy.DataTypes.MultiUrlPicker.MultiUrlPickerStyles.css", Constants.MediaTypeNames.Text.Css)]

namespace uComponents.DataTypes.MultiUrlPicker
{
    /// <summary>
    /// Extension methods for the Multi URL picker
    /// </summary>
    public static class MultiUrlPickerExtensions
    {
        /// <summary>
        /// Adds the JS/CSS required for the MultiUrlPicker
        /// </summary>
        /// <param name="ctl"></param>
        public static void AddAllMultiUrlPickerClientDependencies(this Control ctl)
        {
            //get the urls for the embedded resources
            AddCssMultiUrlPickerClientDependencies(ctl);
            AddJsMultiUrlPickerClientDependencies(ctl);
        }

        /// <summary>
        /// Adds the CSS required for the MultiUrlPicker
        /// </summary>
        /// <param name="ctl"></param>
        public static void AddCssMultiUrlPickerClientDependencies(this Control ctl)
        {
            ctl.RegisterEmbeddedClientResource("uComponents.Legacy.DataTypes.MultiUrlPicker.MultiUrlPickerStyles.css", ClientDependencyType.Css);
        }

        /// <summary>
        /// Adds the JS required for the MultiUrlPicker
        /// </summary>
        /// <param name="ctl"></param>
        public static void AddJsMultiUrlPickerClientDependencies(this Control ctl)
        {
            ctl.RegisterEmbeddedClientResource("uComponents.DataTypes.Shared.Resources.Scripts.json2.js", ClientDependencyType.Javascript);
            ctl.RegisterEmbeddedClientResource("uComponents.Legacy.DataTypes.MultiUrlPicker.MultiUrlPickerScripts.js", ClientDependencyType.Javascript);
        }
    }
}
