using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Test.Imdb
{
    [TestClass]
    public class BodyParameterTests
    {
        public BodyParameterTests()
        {
        }


        [TestInitialize]
        public void Setup()
        {
            Manager = new SimulationManager();
        }

        internal SimulationManager Manager;


        [TestMethod]
        public void CanAddSanityKey()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").AsBodyParameters().And.NotExists("{Kind}").Then.AddToImdb();
            Assert.IsTrue(SimulationManager.Instance.CanAddObject(s.Simulation, "/Pet", "Kind", null, null));
        }



        [TestMethod]
        public void ConvertBodyParametersToJsonNull()
        {
            Assert.AreEqual("{}", Manager.ConvertBodyParametersToJson(null));
        }

        [TestMethod]
        public void ConvertBodyParametersToJsonEmptyString()
        {
            Assert.AreEqual("{}", Manager.ConvertBodyParametersToJson(""));
        }

        [TestMethod]
        public void ConvertBodyParametersToJsonEmptyParameterNameNoValue()
        {
            Assert.AreEqual(@"{""A"":null}", Manager.ConvertBodyParametersToJson("A"));
        }

        [TestMethod]
        public void ConvertBodyParametersToJsonEmptyParameterNameAndValue()
        {
            Assert.AreEqual(@"{""A"":""B""}", Manager.ConvertBodyParametersToJson("A=B"));
        }

        [TestMethod]
        public void ConvertBodyParametersToJsonEmptyParameterNameAndValueThree()
        {
            Assert.AreEqual(@"{""A"":""B"",""c"":""d"",""E"":""f""}", Manager.ConvertBodyParametersToJson("A=B&c=d&E=f"));
        }



        [TestMethod]
        public void ConvertBodyParametersToJsonEmptyParameterNameAndValueWithDecode()
        {
            Assert.AreEqual(@"{""A"":""B/C""}", Manager.ConvertBodyParametersToJson("A=B%2fC"));
        }



        [TestMethod]
        public void ConvertJsonToBodyParametersNull()
        {
            Assert.AreEqual("", Manager.ConvertJsonToBodyParameters(null));
        }

        [TestMethod]
        public void ConvertJsonToBodyParametersEmptyString()
        {
            Assert.AreEqual("", Manager.ConvertJsonToBodyParameters(""));
        }

        [TestMethod]
        public void ConvertJsonToBodyParametersEmptyJson()
        {
            Assert.AreEqual("", Manager.ConvertJsonToBodyParameters(@"{}"));
        }

        [TestMethod]
        public void ConvertJsonToBodyParametersEmptyValue()
        {
            Assert.AreEqual("A=", Manager.ConvertJsonToBodyParameters(@"{""A"":null}"));
        }

        [TestMethod]
        public void ConvertJsonToBodyParametersNameAndValue()
        {
            Assert.AreEqual("A=B", Manager.ConvertJsonToBodyParameters(@"{""A"":""B""}"));
        }

        [TestMethod]
        public void ConvertJsonToBodyParametersNameAndValueEncodedImplicit()
        {
            Assert.AreEqual("A=B%2fC", Manager.ConvertJsonToBodyParameters(@"{""A"":""B/C""}"));
        }

        [TestMethod]
        public void ConvertJsonToBodyParametersNameAndValueEncodedExplicit()
        {
            Assert.AreEqual("A=B%2fC", Manager.ConvertJsonToBodyParameters(@"{""A"":""B/C""}", true));
        }

        [TestMethod]
        public void ConvertJsonToBodyParametersNameAndValueNotEncodedExplicit()
        {
            Assert.AreEqual("A=B/C", Manager.ConvertJsonToBodyParameters(@"{""A"":""B/C""}", false));
        }



        [TestMethod]
        public void ConvertJsonToBodyParametersFourValues()
        {
            Assert.AreEqual("A=B&C=&D=&E=f", Manager.ConvertJsonToBodyParameters(@"{""A"":""B"",""C"":null,""D"":"""",""E"":""f""}"));
        }
    }
}
