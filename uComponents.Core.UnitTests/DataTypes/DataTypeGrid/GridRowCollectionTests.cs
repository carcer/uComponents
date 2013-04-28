﻿namespace uComponents.Core.UnitTests.DataTypes.DataTypeGrid
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Umbraco.Core.Dynamics;

    using uComponents.DataTypes.DataTypeGrid.Model;

    [TestClass]
    public class GridRowCollectionTests
    {
        [TestMethod]
        public void AsDynamicXml_WhenGivenValidGridRowCollection_ShouldReturnDynamicXml()
        {
            var c = new GridRowCollection();
            var x = c.AsDynamicXml();

            Assert.IsInstanceOfType(x, typeof(DynamicXml));
        }

        [TestMethod]
        public void AsDynamicXml_WhenGivenValidXml_ShouldReturnDynamicXml()
        {
            var c = new GridRowCollection("<items><item id=\"2\"><member nodeName=\"Member\" nodeType=\"1036\">1312</member></item><item id=\"3\"><member nodeName=\"Member\" nodeType=\"1036\">1189</member></item><item id=\"4\"><member nodeName=\"Member\" nodeType=\"1036\">1370</member></item></items>");
            var x = c.AsDynamicXml();

            Assert.IsInstanceOfType(x, typeof(DynamicXml));
        }
    }
}