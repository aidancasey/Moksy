using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.IntegrationTest.Macros
{
    /// <summary>
    /// Ensures that Header macros / values are returned in the response body or as part of any returned headers. 
    /// Sometimes we want to 'echo' back the values of headers that were provided.
    /// </summary>
    [TestClass]
    public class HeaderMacroTests : TestBase
    {
        public HeaderMacroTests()
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
            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body("a{Request:Header:NoneExistentHeader}b");
            Proxy.Add(s);

            var response = Get("/Pet");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("a{Request:Header:NoneExistentHeader}b", response.Content);
        }

        [TestMethod]
        public void RequestHeaderProvidedCaseSensitive()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body("a{Request:Header:Owner}b");
            Proxy.Add(s);

            var response = Get("/Pet", new Header[] { new Header("Owner", "Me") });
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("aMeb", response.Content);
        }

        [TestMethod]
        public void RequestHeaderProvidedCaseInsensitive()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body("a{request:header:owner}b");
            Proxy.Add(s);

            var response = Get("/Pet", new Header[] { new Header("Owner", "Me") });
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("aMeb", response.Content);
        }

        [TestMethod]
        public void RequestHeaderProvidedNotEncoded()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body("a{request:Header:Owner}b");
            Proxy.Add(s);

            var response = Get("/Pet", new Header[] { new Header("Owner", "M/e") });
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("aM/eb", response.Content);
        }

        [TestMethod]
        public void RequestHeaderProvidedEncoded()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body("a{request:Header:UrlEncoded:owner}b");
            Proxy.Add(s);

            var response = Get("/Pet", new Header[] { new Header("Owner", "M/e") });
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("aM%2feb", response.Content);
        }



        [TestMethod]
        public void ResponseHeaderMissing()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body("a{Response:Header:NoneExistentHeader}b");
            Proxy.Add(s);

            var response = Get("/Pet");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("a{Response:Header:NoneExistentHeader}b", response.Content);
        }

        [TestMethod]
        public void ResponseHeaderProvided()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Header("MyHeader", "MyValue").With.Body("a{Response:Header:MyHeader}b");
            Proxy.Add(s);

            var response = Get("/Pet");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("aMyValueb", response.Content);
        }



        [TestMethod]
        public void BuiltInVariablesGet()
        {
            // Some variables are available to all methods as placeholders. 
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body("{Request:Url:Root}-{Request:Url:Scheme}-{Request:Url:Host}-{Request:Url:Port}");
            Proxy.Add(simulation1);

            var response = Get("/Pet");
            Assert.AreEqual("http://localhost:10011-http-localhost-10011", response.Content);
        }
    }
}
