using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.IntegrationTest.Macros
{
    /// <summary>
    /// Ensures that Query Parameters macros / values are returned in the response body or as part of any returned headers. 
    /// </summary>
    [TestClass]
    public class QueryParameterTests : TestBase
    {
        public QueryParameterTests()
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
        public void RequestQueryMissing()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet").AsBodyParameters().Then.Return.StatusCode(System.Net.HttpStatusCode.Created).With.Body("a{Request:QueryParameter:NoneExistentParameter}b");
            Proxy.Add(s);

            var response = Get("/Pet?Kind=Dog");
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual("a{Request:QueryParameter:NoneExistentParameter}b", response.Content);
        }


        [TestMethod]
        public void RequestQueryProvided()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet").AsBodyParameters().Then.Return.StatusCode(System.Net.HttpStatusCode.Created).With.Body("a{Request:QueryParameter:Kind}b");
            Proxy.Add(s);

            var response = Get("/Pet?Kind=Dog");
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual("aDogb", response.Content);
        }

        [TestMethod]
        public void RequestQueryNotEncoded()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet").AsBodyParameters().Then.Return.StatusCode(System.Net.HttpStatusCode.Created).With.Body("a{Request:QueryParameter:Kind}b");
            Proxy.Add(s);

            var response = Get("/Pet?Kind=D%2fog");
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual("aD/ogb", response.Content);
        }

        [TestMethod]
        public void RequestQueryEncoded()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet").AsBodyParameters().Then.Return.StatusCode(System.Net.HttpStatusCode.Created).With.Body("a{Request:QueryParameter:UrlEncoded:Kind}b");
            Proxy.Add(s);

            var response = Get("/Pet?Kind=D%2fog");
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual("aD%2fogb", response.Content);
        }
    }
}
