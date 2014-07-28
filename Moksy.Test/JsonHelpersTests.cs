using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Test
{
    [TestClass]
    public class JsonHelpersTests
    {
        public JsonHelpersTests()
        {
        }

        [TestInitialize]
        public void Init()
        {
            Helpers = new JsonHelpers();
        }

        JsonHelpers Helpers;

        [TestMethod]
        public void NullPropertyName()
        {
            var result = Helpers.SetProperty(null, "woo", "{}");
            Assert.AreEqual("{}", result);
        }

        [TestMethod]
        public void NullPropertyValue()
        {
            var result = Helpers.SetProperty("TheProperty", null, "{}");
            Assert.AreEqual(@"{""TheProperty"":null}", result);
        }

        [TestMethod]
        public void NullContentIsCoerced()
        {
            var result = Helpers.SetProperty("TheProperty", null, null);
            Assert.AreEqual(@"{""TheProperty"":null}", result);
        }

        [TestMethod]
        public void NoneNullPropertyValue()
        {
            var result = Helpers.SetProperty("TheProperty", "TheValue", null);
            Assert.AreEqual(@"{""TheProperty"":""TheValue""}", result);
        }

        [TestMethod]
        public void SetNestedJson()
        {
            var result = Helpers.SetProperty("TheProperty", @"{}", null);
            Assert.AreEqual(@"{""TheProperty"":{}}", result);
        }

        [TestMethod]
        public void SetNestedComplex()
        {
            var result = Helpers.SetProperty("TheProperty", @"{""A"":""B""}", null);
            Assert.AreEqual(@"{""TheProperty"":{""A"":""B""}}", result);
        }


    }
}
