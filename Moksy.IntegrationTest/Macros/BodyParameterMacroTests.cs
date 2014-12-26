using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.IntegrationTest.Macros
{
    /// <summary>
    /// Ensures that Body Parameters macros / values are returned in the response body or as part of any returned headers. 
    /// </summary>
    [TestClass]
    public class BodyParameterMacroTests : TestBase
    {
        public BodyParameterMacroTests()
        {
        }

        [TestInitialize]
        public void Initialize()
        {
            Proxy = new Proxy(10011);
            Proxy.DeleteAll();
        }

        Proxy Proxy;



        [TestMethod]
        public void RequestHeaderMissing()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").AsBodyParameters().Then.Return.StatusCode(System.Net.HttpStatusCode.Created).With.Body("a{Request:BodyParameter:NoneExistentParameter}b");
            Proxy.Add(s);

            var response = Post("/Pet", "Kind=Dog");
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual("a{Request:BodyParameter:NoneExistentParameter}b", response.Content);
        }

        [TestMethod]
        public void RequestHeaderProvidedCaseSensitive()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").AsBodyParameters().Then.Return.StatusCode(System.Net.HttpStatusCode.Created).With.Body("a{Request:BodyParameter:Kind}b");
            Proxy.Add(s);

            var response = Post("/Pet", "Kind=Dog");
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual("aDogb", response.Content);
        }

        [TestMethod]
        public void RequestHeaderProvidedCaseInsensitive()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").AsBodyParameters().Then.Return.StatusCode(System.Net.HttpStatusCode.Created).With.Body("a{request:bodyparameter:kind}b");
            Proxy.Add(s);

            var response = Post("/Pet", "Kind=D/og");
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual("aD/ogb", response.Content);
        }

        [TestMethod]
        public void RequestHeaderProvidedCaseInsensitiveUrlEncoded()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").AsBodyParameters().Then.Return.StatusCode(System.Net.HttpStatusCode.Created).With.Body("a{request:bodyparameter:urlencoded:kind}b");
            Proxy.Add(s);

            var response = Post("/Pet", "Kind=D/og");
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual("aD%2fogb", response.Content);
        }

    }
}
