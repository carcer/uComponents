﻿using System.Linq;
using uComponents.Core;
using umbraco;
using umbraco.MacroEngines;
using umbraco.MacroEngines.Library;
using uComponents.DataTypes.ImagePoint;

namespace uComponents.DataTypes.RazorDataTypeModels.ImagePoint
{
    [RazorDataTypeModel(Constants.DataTypes.ImagePointId)]
    public class ImagePointModelBinder : IRazorDataTypeModel
    {
        public bool Init(int CurrentNodeId, string PropertyData, out object instance)
        {
            // if model binding is disbaled, return a DynamicXml object
            if (!Settings.RazorModelBindingEnabled)
            {
#pragma warning disable 0618
                instance = new DynamicXml(PropertyData);
#pragma warning restore 0618
                return true;
            }

            DataTypes.ImagePoint.ImagePoint imagePoint = new DataTypes.ImagePoint.ImagePoint();

            // use the uQuery IGetProperty interface to do the work
            ((uQuery.IGetProperty)imagePoint).LoadPropertyValue(PropertyData);

            instance = imagePoint;
            return true;
        }
    }
}
