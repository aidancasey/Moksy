using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Common.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.IntegrationTest.Imdb
{
    [TestClass]
    public class BodyParameterTests : TestBase 
    {
        public BodyParameterTests()
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
        public void SanityPostAndAdd()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").AsBodyParameters().And.NotExists("{Kind}").Then.AddToImdb().And.Return.StatusCode(System.Net.HttpStatusCode.Created);
            Proxy.Add(s);

            var response = Post("/Pet", "Kind=Dog&Name=Rover");
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
        }

        [TestMethod]
        public void SanityPostAndAddAndGet()
        {
            AssertGetContentReturned("Kind=Dog&Name=Rover", "{value}", "Kind=Dog&Name=Rover");
        }

        [TestMethod]
        public void SanityPostAndAddAndGetAsJson()
        {
            AssertGetContentReturned("Kind=Dog&Name=Rover", "{valueAsJson}", @"{""Kind"":""Dog"",""Name"":""Rover""}");
        }

        [TestMethod]
        public void SanityPostAndAddAndGetAsJsonBpEncoded()
        {
            AssertGetContentReturned("Kind=Dog&Name=Ro%2fver", "{valueAsBodyParameters}", "Kind=Dog&Name=Ro%2fver");
        }

        [TestMethod]
        public void SanityPostAndAddAndGetAsJsonBpNotEncoded()
        {
            AssertGetContentReturned("Kind=Dog&Name=Rov%2fer", "{valueAsBodyParametersNotEncoded}", "Kind=Dog&Name=Rov/er");
        }

        protected void AssertGetContentReturned(string valueToPost, string bodyTokens, string value)
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").AsBodyParameters().And.NotExists("{Kind}").Then.AddToImdb().And.Return.StatusCode(System.Net.HttpStatusCode.Created).With.Body(bodyTokens);
            Proxy.Add(s);

            s = Moksy.Common.SimulationFactory.When.I.Get().FromImdb("/Pet/{Kind}").AsBodyParameters().And.Exists("{Kind}").Then.Return.Body(bodyTokens).And.Return.StatusCode(System.Net.HttpStatusCode.OK);
            Proxy.Add(s);

            var response = Post("/Pet", valueToPost);
            Assert.AreEqual(value, response.Content);

            response = Get("/Pet/Dog");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(value, response.Content);
        }



        [TestMethod]
        public void SanityPut()
        {
            AssertGetContentReturnedWithPut("Kind=Dog&Name=Ro%2fver", "{valueAsBodyParameters}", "Kind=Dog&Name=Ro%2fver", "Kind=Dog&Name=K9");
        }



        protected void AssertGetContentReturnedWithPut(string valueToPost, string bodyTokens, string value, string putValue)
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").AsBodyParameters().And.NotExists("{Kind}").Then.AddToImdb().And.Return.StatusCode(System.Net.HttpStatusCode.Created).With.Body(bodyTokens);
            Proxy.Add(s);

            s = Moksy.Common.SimulationFactory.When.I.Put().ToImdb("/Pet/{Kind}").AsBodyParameters().And.Exists("{Kind}").Then.AddToImdb().And.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body(bodyTokens);
            Proxy.Add(s);

            s = Moksy.Common.SimulationFactory.When.I.Get().FromImdb("/Pet/{Kind}").AsBodyParameters().And.Exists("{Kind}").Then.Return.Body(bodyTokens).And.Return.StatusCode(System.Net.HttpStatusCode.OK);
            Proxy.Add(s);

            var response = Post("/Pet", valueToPost);
            Assert.AreEqual(value, response.Content);

            response = Get("/Pet/Dog");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(value, response.Content);

            response = Put("/Pet/Dog", putValue);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(putValue, response.Content);
        }



        // Two sanity tests for the constraints to ensure the code paths are being exercised in the service for Body Parameters. 

        [TestMethod]
        public void OneMatchingViolationBetweenIsReturnedCustomResponse()
        {
            var between = new LengthBetween("Kind", 5, 8) { Response = @"{""Kind"":""OutOfRangeException"",""PropertyName"":""Kind"",""MinimumLength"":{MinimumLength}}" };

            var simulation1 = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").AsBodyParameters().And.NotExists("{Kind}").With.Constraint(between).And.HasConstraintViolations().Then.Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).With.Body("{violationResponses}").And.AddToImdb();
            Proxy.Add(simulation1);

            var response = Post("/Pet", @"Kind=Cat&Name=Sir");
            Assert.AreEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);
            Assert.AreEqual(@"[{""Kind"":""OutOfRangeException"",""PropertyName"":""Kind"",""MinimumLength"":5}]", response.Content);
        }

        [TestMethod]
        public void OneMatchingViolationBetweenIsReturnedCustomResponsePut()
        {
            var between = new LengthBetween("Kind", 5, 8) { Response = @"{""Kind"":""OutOfRangeException"",""PropertyName"":""Kind"",""MinimumLength"":{MinimumLength}}" };

            var simulation1 = Moksy.Common.SimulationFactory.When.I.Put().ToImdb("/Pet").AsBodyParameters().And.NotExists("{Kind}").With.Constraint(between).And.HasConstraintViolations().Then.Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).With.Body("{violationResponses}").And.AddToImdb();
            Proxy.Add(simulation1);

            var response = Put("/Pet", @"Kind=Cat&Name=Sir");
            Assert.AreEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);
            Assert.AreEqual(@"[{""Kind"":""OutOfRangeException"",""PropertyName"":""Kind"",""MinimumLength"":5}]", response.Content);
        }
    }
}
