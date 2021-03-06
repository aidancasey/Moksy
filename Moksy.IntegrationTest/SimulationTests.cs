﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Common;
using Moksy.Common.Constraints;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.IntegrationTest
{
    /// <summary>
    /// Full tests exercising the end-to-end functionality of Simulations. This will set up a Simulation and then hit the end-point to confirm the 
    /// response. 
    /// </summary>
    [TestClass]
    [DeploymentItem("TestData", "TestData")]
    public class SimulationTests : TestBase
    {
        public SimulationTests()
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
        public void HitAnEndpointWithNothingDefined()
        {
            var response = Get("/nonexistent");
            Assert.AreEqual(System.Net.HttpStatusCode.NotImplemented, response.StatusCode);
        }

        [TestMethod]
        public void GetUrlWithNoHeadersReturnsCorrectStatusCode()
        {
            var simulation = Moksy.Common.SimulationFactory.New("Pf").Get().From("/Something").Return.StatusCode(System.Net.HttpStatusCode.PreconditionFailed);
            Proxy.Add(simulation);

            var response = Get("/Something");
            Assert.AreEqual(System.Net.HttpStatusCode.PreconditionFailed, response.StatusCode);
        }

        [TestMethod]
        public void GetUrlWithHeaderReturnsNotFoundIfHeaderIsNotProvidedInRequest()
        {
            var simulation = Moksy.Common.SimulationFactory.New("Pf").Get().From("/Something").With.Header("MyHeader", "MyValue").Return.StatusCode(System.Net.HttpStatusCode.PreconditionFailed);
            Proxy.Add(simulation);

            var response = Get("/Something");
            Assert.AreEqual(System.Net.HttpStatusCode.NotImplemented, response.StatusCode);
        }

        [TestMethod]
        public void GetUrlWithHeaderReturnsFoundIfHeaderIsProvidedInRequest()
        {
            var simulation = Moksy.Common.SimulationFactory.New("Pf").Get().From("/Something").With.Header("MyHeader", "MyValue").Return.StatusCode(System.Net.HttpStatusCode.NotModified);
            Proxy.Add(simulation);

            var response = Get("/Something", new List<Header>() { new Header("MyHeader", "MyValue") });
            Assert.AreEqual(System.Net.HttpStatusCode.NotModified, response.StatusCode);
        }

        [TestMethod]
        public void GetUrlWithHeaderReturnsNotFoundIfOneOfTwoHeadersIsProvidedInRequest()
        {
            var simulation = Moksy.Common.SimulationFactory.New("Pf").Get().From("/Something").With.Header("MyHeader", "MyValue").Header("MyOtherHeader", "MyOtherValue").Return.StatusCode(System.Net.HttpStatusCode.NotModified);
            Proxy.Add(simulation);

            var response = Get("/Something", new List<Header>() { new Header("MyHeader", "MyValue") });
            Assert.AreEqual(System.Net.HttpStatusCode.NotImplemented, response.StatusCode);
        }

        [TestMethod]
        public void GetUrlWithNoHeadersReturnsContent()
        {
            var simulation = Moksy.Common.SimulationFactory.New("Pf").Get().From("/Something").Return.Body("My own content").StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(simulation);

            var response = Get("/Something");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
            Assert.AreEqual("My own content", response.Content);
        }

        [TestMethod]
        public void GetUrlWithNoHeadersReturnsResponseHeaders()
        {
            var simulation = Moksy.Common.SimulationFactory.New("Pf").Get().From("/Something").Return.Body("My own content").With.Header("Woo", "Hoo").StatusCode(System.Net.HttpStatusCode.NotAcceptable);
            Proxy.Add(simulation);

            var response = Get("/Something");
            Assert.AreEqual(System.Net.HttpStatusCode.NotAcceptable, response.StatusCode);
            Assert.AreEqual("My own content", response.Content);

            var match = response.Headers.FirstOrDefault(f => f.Name == "Woo");
            Assert.IsNotNull(match);
            Assert.AreEqual("Hoo", match.Value);
        }



        [TestMethod]
        public void DeleteMatches()
        {
            var simulation = Moksy.Common.SimulationFactory.New("Pf").Delete().From("/Something").Return.Body("My own content").With.Header("Woo", "Hoo").StatusCode(System.Net.HttpStatusCode.NotAcceptable);
            Proxy.Add(simulation);

            var response = Delete("/Something");
            Assert.AreEqual("My own content", response.Content);
            Assert.AreEqual(System.Net.HttpStatusCode.NotAcceptable, response.StatusCode);

            var match = response.Headers.FirstOrDefault(f => f.Name == "Woo");
            Assert.IsNotNull(match);
            Assert.AreEqual("Hoo", match.Value);
        }



        [TestMethod]
        public void DeleteEvaluatesOnlyOnce()
        {
            var simulation = Moksy.Common.SimulationFactory.New("Pf").Delete().From("/Something").Once().Return.Body("My own content").With.Header("Woo", "Hoo").StatusCode(System.Net.HttpStatusCode.NotAcceptable);
            Proxy.Add(simulation);

            var response = Delete("/Something");
            Assert.AreEqual("My own content", response.Content);
            Assert.AreEqual(System.Net.HttpStatusCode.NotAcceptable, response.StatusCode);

            // The first (above) should have evaluated once and then been removed; the second time, the simulation should not exist. 
            response = Delete("/Something");
            Assert.AreEqual(System.Net.HttpStatusCode.NotImplemented, response.StatusCode);
        }



        [TestMethod]
        public void Mega()
        {
            var del = Moksy.Common.SimulationFactory.New("ToDelete").Delete().From("/Something").Return.Body("MydeletedContent").With.Header("Woodel", "Hoodel").StatusCode(System.Net.HttpStatusCode.NotAcceptable);
            Proxy.Add(del);

            var add = Moksy.Common.SimulationFactory.New("ToGet").Get().From("/Something").Return.Body("MyAddedContent").With.Header("Wooadd", "Hooadd").StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(add);

            var response = Delete("/Nothing");
            Assert.AreEqual(System.Net.HttpStatusCode.NotImplemented, response.StatusCode);

            // DELETE
            response = Delete("/Something");
            Assert.AreEqual(System.Net.HttpStatusCode.NotAcceptable, response.StatusCode);

            // GET
            response = Get("/Something");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }



        [TestMethod]
        public void InMemoryForPostAndGetSingleBothWithImdbAndAdd()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Product").Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices).With.AddToImdb("{Kind}");
            Proxy.Add(s);

            var t = Moksy.Common.SimulationFactory.When.I.Get().FromImdb("/Product").Then.Return.StatusCode(System.Net.HttpStatusCode.NonAuthoritativeInformation);
            Proxy.Add(t);

            var response = Post("/Product", @"{""Kind"":""Dog""}");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);

            response = Get("/Product");
            Assert.AreEqual(System.Net.HttpStatusCode.NonAuthoritativeInformation, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Dog""}", response.Content);
        }

        [TestMethod]
        public void InMemoryForPostAndGetSingleBothWithImdbAndNoAdd()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Product").Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var t = Moksy.Common.SimulationFactory.When.I.Get().FromImdb("/Product").Then.Return.StatusCode(System.Net.HttpStatusCode.NonAuthoritativeInformation);
            Proxy.Add(t);

            var response = Post("/Product", "MyContent");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);

            response = Get("/Product");
            Assert.AreEqual(System.Net.HttpStatusCode.NonAuthoritativeInformation, response.StatusCode);
            Assert.AreEqual("", response.Content);
        }

        [TestMethod]
        public void InMemoryForPostAndGetSinglePostWithImdb()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Product").Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var t = Moksy.Common.SimulationFactory.When.I.Get().From("/Product").Then.Return.StatusCode(System.Net.HttpStatusCode.NotAcceptable);
            Proxy.Add(t);

            var response = Post("/Product", "MyContent");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);

            response = Get("/Product");
            Assert.AreEqual(System.Net.HttpStatusCode.NotAcceptable, response.StatusCode);
            Assert.AreEqual("", response.Content);
        }

        [TestMethod]
        public void InMemoryForPostWithoutImdbAndGetWithImdb()
        {
            var s = Moksy.Common.SimulationFactory.When.Post().To("/Product").Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var t = Moksy.Common.SimulationFactory.When.Get().FromImdb("/Product").Then.Return.StatusCode(System.Net.HttpStatusCode.NotAcceptable);
            Proxy.Add(t);

            var response = Post("/Product", "MyContent");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);

            response = Get("/Product");
            Assert.AreEqual(System.Net.HttpStatusCode.NotAcceptable, response.StatusCode);
            Assert.AreEqual("", response.Content);
        }

        [TestMethod]
        public void PostToImdbReturnsEmptyWithNoBodySpecified()
        {
            var s = Moksy.Common.SimulationFactory.When.Post().To("/Product").Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Product", "MyContent");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
            Assert.AreEqual("", response.Content);
        }

        [TestMethod]
        public void PostToImdbReturnsBodyWhenSpecified()
        {
            var s = Moksy.Common.SimulationFactory.When.Post().ToImdb("/Product").Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices).With.Body("Woo");
            Proxy.Add(s);

            var response = Post("/Product", "MyContent");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
            Assert.AreEqual("Woo", response.Content);
        }

        [TestMethod]
        public void GetAsJsonNoEntries()
        {
            var simulation = Moksy.Common.SimulationFactory.When.I.Get().FromImdb("/Product").AsJson().Return.StatusCode(System.Net.HttpStatusCode.PaymentRequired).With.Body("[{value}]");
            Proxy.Add(simulation);

            var response = Get("/Product");
            Assert.AreEqual(System.Net.HttpStatusCode.PaymentRequired, response.StatusCode);
            Assert.AreEqual("[]", response.Content);
        }

        [TestMethod]
        public void PostAndGetAsJsonSingleEntry()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Product").AsJson().Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired).AddToImdb("{SomeProperty}");
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.New("Second").I.Get().FromImdb("/Product").AsJson().Return.StatusCode(System.Net.HttpStatusCode.PaymentRequired);
            Proxy.Add(simulation2);

            var response = Post("/Product", "{ }");
            Assert.AreEqual(System.Net.HttpStatusCode.ProxyAuthenticationRequired, response.StatusCode);

            response = Get("/Product");
            Assert.AreEqual(System.Net.HttpStatusCode.PaymentRequired, response.StatusCode);
            Assert.AreEqual("{ }", response.Content);
        }

        [TestMethod]
        public void PostAndGetAsJsonTwoEntriesEntry()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Product").AsJson().Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired).AddToImdb("{a}");
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.New("Second").I.Get().FromImdb("/Product").AsJson().Return.StatusCode(System.Net.HttpStatusCode.PaymentRequired);
            Proxy.Add(simulation2);

            var response = Post("/Product", "{ }");
            Assert.AreEqual(System.Net.HttpStatusCode.ProxyAuthenticationRequired, response.StatusCode);
            response = Post("/Product", @"{ ""a"" : ""b"" }");
            Assert.AreEqual(System.Net.HttpStatusCode.ProxyAuthenticationRequired, response.StatusCode);

            response = Get("/Product");
            Assert.AreEqual(System.Net.HttpStatusCode.PaymentRequired, response.StatusCode);
            Assert.AreEqual(@"{ },{ ""a"" : ""b"" }", response.Content);
        }

        [TestMethod]
        public void PostAndGetAsJsonTwoEntriesWithValuePlaceholder()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Product").AsJson().Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired).AddToImdb("{a}");
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.New("Second").I.Get().FromImdb("/Product").AsJson().Return.Body("Abc{value}Def").StatusCode(System.Net.HttpStatusCode.PaymentRequired);
            Proxy.Add(simulation2);

            var response = Post("/Product", "{ }");
            response = Post("/Product", @"{ ""a"" : ""b"" }");

            response = Get("/Product");
            Assert.AreEqual(System.Net.HttpStatusCode.PaymentRequired, response.StatusCode);
            Assert.AreEqual(@"Abc{ },{ ""a"" : ""b"" }Def", response.Content);
        }



        [TestMethod]
        public void PostAsJsonWithUniqueKeyDoesNotExistSoAdds()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired).AddToImdb();
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.New("Second").I.Get().FromImdb("/Pet").AsJson().Return.StatusCode(System.Net.HttpStatusCode.PaymentRequired);
            Proxy.Add(simulation2);

            var response = Post("/Pet", @"{ ""Kind"" : ""Dog""  }");
            Assert.AreEqual(System.Net.HttpStatusCode.ProxyAuthenticationRequired, response.StatusCode);

            response = Get("/Pet");
            Assert.AreEqual(System.Net.HttpStatusCode.PaymentRequired, response.StatusCode);
            Assert.AreEqual(@"{ ""Kind"" : ""Dog""  }", response.Content);
        }

        [TestMethod]
        public void PostAsJsonWithUniqueKeyAlreadyExistsSoReturnsSeparateMessage()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired).AddToImdb();
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.New("Second").I.Post().ToImdb("/Pet").AsJson().Return.StatusCode(System.Net.HttpStatusCode.Forbidden);
            Proxy.Add(simulation2);

            var simulation3 = Moksy.Common.SimulationFactory.New("Third").I.Get().FromImdb("/Pet").AsJson().Return.StatusCode(System.Net.HttpStatusCode.PaymentRequired);
            Proxy.Add(simulation3);

            var response = Post("/Pet", @"{ ""Kind"" : ""Dog""  }");
            Assert.AreEqual(System.Net.HttpStatusCode.ProxyAuthenticationRequired, response.StatusCode);

            // Post again - it is NOT UNIQUE so we expect a different response - Ambiguous. 
            response = Post("/Pet", @"{ ""Kind"" : ""Dog""  }");
            Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, response.StatusCode);

            response = Get("/Pet");
            Assert.AreEqual(System.Net.HttpStatusCode.PaymentRequired, response.StatusCode);
            Assert.AreEqual(@"{ ""Kind"" : ""Dog""  }", response.Content);
        }



        [TestMethod]
        public void GetReturnsNotImplementedIfLookingUpByKeyAndDoesNotExist()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired).AddToImdb();
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.New("Third").I.Get().FromImdb("/Pet").AsJson().Return.StatusCode(System.Net.HttpStatusCode.PaymentRequired);
            Proxy.Add(simulation2);

            var response = Get("/Pet('Dog')");
            Assert.AreEqual(System.Net.HttpStatusCode.NotImplemented, response.StatusCode);
        }

        [TestMethod]
        public void GetReturnsFallthroughIfLookingUpByKeyAndDoesNotExist()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired).AddToImdb();
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.New("Second").I.Get().FromImdb("/Pet('{Kind}')").AsJson().And.Exists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.PaymentRequired);
            Proxy.Add(simulation2);

            var simulation3 = Moksy.Common.SimulationFactory.New("Third").I.Get().FromImdb("/Pet('{Kind}')").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(simulation3);

            
            var response = Get("/Pet('Dog')");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void GetReturnsFallthroughIfLookingUpByKeyAndDoesExist()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired).AddToImdb();
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.New("Second").I.Get().FromImdb("/Pet('{Kind}')").AsJson().And.Exists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.PaymentRequired);
            Proxy.Add(simulation2);

            var simulation3 = Moksy.Common.SimulationFactory.New("Third").I.Get().FromImdb("/Pet('{Kind}')").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(simulation3);

            var response = Post("/Pet", @"{ ""Kind"" : ""Dog""  }");
            Assert.AreEqual(System.Net.HttpStatusCode.ProxyAuthenticationRequired, response.StatusCode);

            response = Get("/Pet('Dog')");
            Assert.AreEqual(System.Net.HttpStatusCode.PaymentRequired, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Dog""}", response.Content);
        }


        [TestMethod]
        public void GetReturnsFallthroughIfLookingUpByKeyAndDoesExistWithCustomBody()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired).AddToImdb();
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.New("Second").I.Get().FromImdb("/Pet('{Kind}')").AsJson().And.Exists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.PaymentRequired).And.Body("Woo{value}Hoo");
            Proxy.Add(simulation2);

            var simulation3 = Moksy.Common.SimulationFactory.New("Third").I.Get().FromImdb("/Pet('{Kind}')").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(simulation3);

            var response = Post("/Pet", @"{ ""Kind"" : ""Dog""  }");
            Assert.AreEqual(System.Net.HttpStatusCode.ProxyAuthenticationRequired, response.StatusCode);

            response = Get("/Pet('Dog')");
            Assert.AreEqual(System.Net.HttpStatusCode.PaymentRequired, response.StatusCode);
            Assert.AreEqual(@"Woo{""Kind"":""Dog""}Hoo", response.Content);
        }

        [TestMethod]
        public void GetReturnsFallthroughIfLookingUpByKeyAndDoesExistWithConstantValue()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired).AddToImdb();
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.New("Second").I.Get().FromImdb("/Pet('{Kind}')").AsJson().And.Exists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.PaymentRequired).And.Body("MyConstantValue");
            Proxy.Add(simulation2);

            var simulation3 = Moksy.Common.SimulationFactory.New("Third").I.Get().FromImdb("/Pet('{Kind}')").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(simulation3);

            var response = Post("/Pet", @"{ ""Kind"" : ""Dog""  }");
            Assert.AreEqual(System.Net.HttpStatusCode.ProxyAuthenticationRequired, response.StatusCode);

            response = Get("/Pet('Dog')");
            Assert.AreEqual(System.Net.HttpStatusCode.PaymentRequired, response.StatusCode);
            Assert.AreEqual(@"MyConstantValue", response.Content);
        }



        [TestMethod]
        public void GetImplicitIndexPropertyMatchesWhenPropertyDoesExist()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired).AddToImdb();
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.New("Second").I.Get().FromImdb("/Pet('{Kind}')").AsJson().And.Exists().Return.StatusCode(System.Net.HttpStatusCode.PaymentRequired);
            Proxy.Add(simulation2);

            var simulation3 = Moksy.Common.SimulationFactory.New("Third").I.Get().FromImdb("/Pet('{Kind}')").AsJson().And.NotExists().Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(simulation3);

            var response = Post("/Pet", @"{ ""Kind"" : ""Dog""  }");
            Assert.AreEqual(System.Net.HttpStatusCode.ProxyAuthenticationRequired, response.StatusCode);

            response = Get("/Pet('Dog')");
            Assert.AreEqual(System.Net.HttpStatusCode.PaymentRequired, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Dog""}", response.Content);
        }

        [TestMethod]
        public void GetImplicitIndexPropertyDoesNotMatchWhenPropertyDoesNotExist()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired).AddToImdb();
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.New("Second").I.Get().FromImdb("/Pet('{Kind}')").AsJson().And.Exists().Return.StatusCode(System.Net.HttpStatusCode.PaymentRequired);
            Proxy.Add(simulation2);

            var simulation3 = Moksy.Common.SimulationFactory.New("Third").I.Get().FromImdb("/Pet('{Kind}')").AsJson().And.NotExists().Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(simulation3);

            var response = Get("/Pet('Dog')");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }



        [TestMethod]
        public void DeleteDoesNotMatchImdbWithNoEntries()
        {
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Delete().FromImdb("/Pet('{Kind}')").AsJson().And.Exists().Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(simulation1); 
            
            var simulation2 = Moksy.Common.SimulationFactory.When.I.Delete().FromImdb("/Pet('{Kind}')").AsJson().And.NotExists().Then.Return.StatusCode(System.Net.HttpStatusCode.PaymentRequired);
            Proxy.Add(simulation2);

            var response = Delete("/Pet('Dog')");
            Assert.AreEqual(System.Net.HttpStatusCode.PaymentRequired, response.StatusCode);
        }

        [TestMethod]
        public void DeleteDoesNotMatchImdbWithOneEntryEntries()
        {
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Delete().FromImdb("/Pet('{Kind}')").AsJson().And.Exists().Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.When.I.Delete().FromImdb("/Pet('{Kind}')").AsJson().And.NotExists().Then.Return.StatusCode(System.Net.HttpStatusCode.PaymentRequired);
            Proxy.Add(simulation2);

            var simulation3 = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired).AddToImdb();
            Proxy.Add(simulation3);

            var response = Post("/Pet", @"{ ""Kind"" : ""Cat""  }");

            response = Delete("/Pet('Dog')");
            Assert.AreEqual(System.Net.HttpStatusCode.PaymentRequired, response.StatusCode);
        }

        [TestMethod]
        public void DeleteMatchesButDoesNotRemoveTheEntryFromTheDatabase()
        {
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Delete().FromImdb("/Pet('{Kind}')").AsJson().And.Exists().Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.When.I.Delete().FromImdb("/Pet('{Kind}')").AsJson().And.NotExists().Then.Return.StatusCode(System.Net.HttpStatusCode.PaymentRequired);
            Proxy.Add(simulation2);

            var simulation3 = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired).AddToImdb();
            Proxy.Add(simulation3);

            var simulation4 = Moksy.Common.SimulationFactory.New("Second").I.Get().FromImdb("/Pet('{Kind}')").AsJson().And.Exists().Return.StatusCode(System.Net.HttpStatusCode.NotModified);
            Proxy.Add(simulation4);

            var simulation5 = Moksy.Common.SimulationFactory.New("Third").I.Get().FromImdb("/Pet('{Kind}')").AsJson().And.NotExists().Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired);
            Proxy.Add(simulation5);

            var response = Post("/Pet", @"{ ""Kind"" : ""Cat""  }");

            response = Delete("/Pet('Cat')");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);

            response = Get("/Pet('Cat')");
            Assert.AreEqual(System.Net.HttpStatusCode.NotModified, response.StatusCode);
        }

        [TestMethod]
        public void DeleteMatchesButDoesRemoveTheEntryFromTheDatabase()
        {
            var simulation2 = Moksy.Common.SimulationFactory.When.I.Delete().FromImdb("/Pet('{Kind}')").AsJson().And.NotExists().Then.Return.StatusCode(System.Net.HttpStatusCode.PaymentRequired);
            Proxy.Add(simulation2); 
            
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Delete().FromImdb("/Pet('{Kind}')").AsJson().And.Exists().Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices).RemoveFromImdb();
            Proxy.Add(simulation1);

            var simulation3 = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.Created).AddToImdb();
            Proxy.Add(simulation3);

            var simulation4 = Moksy.Common.SimulationFactory.New("Second").I.Get().FromImdb("/Pet('{Kind}')").AsJson().And.Exists().Return.StatusCode(System.Net.HttpStatusCode.NotModified);
            Proxy.Add(simulation4);

            var simulation5 = Moksy.Common.SimulationFactory.New("Third").I.Get().FromImdb("/Pet('{Kind}')").AsJson().And.NotExists().Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired);
            Proxy.Add(simulation5);

            var response = Post("/Pet", @"{ ""Kind"" : ""Cat""  }");
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);

            response = Delete("/Pet('Cat')");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);

            response = Get("/Pet('Cat')");
            Assert.AreEqual(System.Net.HttpStatusCode.ProxyAuthenticationRequired, response.StatusCode);
        }

        [TestMethod]
        public void DeleteWithNoMatchReturnsFixedBodyContent()
        {
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Delete().FromImdb("/Pet('{Kind}')").AsJson().And.NotExists().Then.Return.StatusCode(System.Net.HttpStatusCode.PaymentRequired).With.Body("FixedContent");
            Proxy.Add(simulation1);

            var response = Delete("/Pet('Cat')");
            Assert.AreEqual(System.Net.HttpStatusCode.PaymentRequired, response.StatusCode);
            Assert.AreEqual("FixedContent", response.Content);
        }

        [TestMethod]
        public void DeleteWithMatchReturnsDynamicContentNoRemove()
        {
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Delete().FromImdb("/Pet('{Kind}')").AsJson().And.Exists().Then.Return.StatusCode(System.Net.HttpStatusCode.PaymentRequired).With.Body("{value}");
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired).AddToImdb();
            Proxy.Add(simulation2);

            var response = Post("/Pet", @"{ ""Kind"" : ""Cat""  }");

            response = Delete("/Pet('Cat')");
            Assert.AreEqual(System.Net.HttpStatusCode.PaymentRequired, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Cat""}", response.Content);

            response = Delete("/Pet('Cat')");
            Assert.AreEqual(System.Net.HttpStatusCode.PaymentRequired, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Cat""}", response.Content);
        }

        [TestMethod]
        public void DeleteWithMatchReturnsDynamicContentWithRemove()
        {
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Delete().FromImdb("/Pet('{Kind}')").AsJson().And.Exists().Then.Return.StatusCode(System.Net.HttpStatusCode.PaymentRequired).With.Body("{value}").RemoveFromImdb();
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired).AddToImdb();
            Proxy.Add(simulation2);

            var response = Post("/Pet", @"{ ""Kind"" : ""Cat""  }");

            response = Delete("/Pet('Cat')");
            Assert.AreEqual(System.Net.HttpStatusCode.PaymentRequired, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Cat""}", response.Content);

            response = Delete("/Pet('Cat')");
            Assert.AreEqual(System.Net.HttpStatusCode.NotImplemented, response.StatusCode);
        }



        [TestMethod]
        public void PutNotExistsNotAddToDatabase()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Put().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.ExpectationFailed);
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.New("Second").I.Get().FromImdb("/Pet/{Kind}").AsJson().And.Exists().Return.StatusCode(System.Net.HttpStatusCode.NotModified);
            Proxy.Add(simulation2);

            var simulation3 = Moksy.Common.SimulationFactory.New("Third").I.Get().FromImdb("/Pet/{Kind}").AsJson().And.NotExists().Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired);
            Proxy.Add(simulation3);

            //var response = Post("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"" : ""Garfield""  }");

            var response = Put("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"" : ""Kitty""  }");
            Assert.AreEqual(System.Net.HttpStatusCode.ExpectationFailed, response.StatusCode);

            response = Get("/Pet/Cat");
            Assert.AreEqual(System.Net.HttpStatusCode.ProxyAuthenticationRequired, response.StatusCode);
        }

        [TestMethod]
        public void PutExistsNotAddToDatabase()
        {
            var add = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired).AddToImdb();
            Proxy.Add(add);

            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Put().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.ExpectationFailed);
            Proxy.Add(simulation1);

            var simulation4 = Moksy.Common.SimulationFactory.New("Fourth").I.Put().ToImdb("/Pet").AsJson().And.Exists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.LengthRequired);
            Proxy.Add(simulation4);

            var simulation2 = Moksy.Common.SimulationFactory.New("Second").I.Get().FromImdb("/Pet/{Kind}").AsJson().And.Exists().Return.StatusCode(System.Net.HttpStatusCode.NotModified);
            Proxy.Add(simulation2);

            var simulation3 = Moksy.Common.SimulationFactory.New("Third").I.Get().FromImdb("/Pet/{Kind}").AsJson().And.NotExists().Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired);
            Proxy.Add(simulation3);

            var response = Post("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"" : ""Garfield""  }");

            response = Put("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"" : ""Kitty""  }");
            Assert.AreEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);

            response = Get("/Pet/Cat");
            Assert.AreEqual(System.Net.HttpStatusCode.NotModified, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Cat"",""Name"":""Garfield""}", response.Content);
        }



        [TestMethod]
        public void PutNotExistsAddToDatabase()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Put().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.ExpectationFailed).And.AddToImdb();
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.New("Second").I.Get().FromImdb("/Pet/{Kind}").AsJson().And.Exists().Return.StatusCode(System.Net.HttpStatusCode.NotModified);
            Proxy.Add(simulation2);

            var simulation3 = Moksy.Common.SimulationFactory.New("Third").I.Get().FromImdb("/Pet/{Kind}").AsJson().And.NotExists().Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired);
            Proxy.Add(simulation3);

            //var response = Post("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"" : ""Garfield""  }");

            var response = Put("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"" : ""Kitty""  }");
            Assert.AreEqual(System.Net.HttpStatusCode.ExpectationFailed, response.StatusCode);

            response = Get("/Pet/Cat");
            Assert.AreEqual(System.Net.HttpStatusCode.NotModified, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Cat"",""Name"":""Kitty""}", response.Content);
        }

        [TestMethod]
        public void PutExistsAddToDatabase()
        {
            var add = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired).AddToImdb();
            Proxy.Add(add);

            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Put().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.ExpectationFailed);
            Proxy.Add(simulation1);

            var simulation4 = Moksy.Common.SimulationFactory.New("Fourth").I.Put().ToImdb("/Pet").AsJson().And.Exists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).Then.AddToImdb();
            Proxy.Add(simulation4);

            var simulation2 = Moksy.Common.SimulationFactory.New("Second").I.Get().FromImdb("/Pet/{Kind}").AsJson().And.Exists().Return.StatusCode(System.Net.HttpStatusCode.NotModified);
            Proxy.Add(simulation2);

            var simulation3 = Moksy.Common.SimulationFactory.New("Third").I.Get().FromImdb("/Pet/{Kind}").AsJson().And.NotExists().Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired);
            Proxy.Add(simulation3);

            var response = Post("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"" : ""Garfield""  }");

            response = Put("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"" : ""Kitty""  }");
            Assert.AreEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);

            response = Get("/Pet/Cat");
            Assert.AreEqual(System.Net.HttpStatusCode.NotModified, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Cat"",""Name"":""Kitty""}", response.Content);
        }



        [TestMethod]
        public void PutReturnsNoContent()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Put().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).And.AddToImdb();
            Proxy.Add(simulation1);

            var response = Put("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"" : ""Kitty""  }");
            Assert.AreEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);
            Assert.AreEqual("", response.Content);
        }

        [TestMethod]
        public void PutReturnsValue()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Put().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).And.AddToImdb().With.Body("{value}");
            Proxy.Add(simulation1);

            var response = Put("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"" : ""Kitty""  }");
            Assert.AreEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);
            Assert.AreEqual(@"{ ""Kind"" : ""Cat"", ""Name"" : ""Kitty""  }", response.Content);
        }



        [TestMethod]
        public void DynamicGuidInReturnBodyAlongWithValue()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Put().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).And.AddToImdb().With.Variable("TheVar").Body("{value}-{TheVar}THEEND");
            Proxy.Add(simulation1);

            var response = Put("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"" : ""Kitty""  }");
            Assert.AreEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);
            Assert.IsTrue(response.Content.StartsWith(@"{ ""Kind"" : ""Cat"", ""Name"" : ""Kitty""  }-"));
            Assert.IsTrue(response.Content.EndsWith(@"THEEND"));
        }

        [TestMethod]
        public void ConstantInReturnBodyAlongWithValue()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Put().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).And.AddToImdb().With.Variable("TheVar", "TheValue").Body("{value}-{TheVar}THEEND");
            Proxy.Add(simulation1);

            var response = Put("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"" : ""Kitty""  }");
            Assert.AreEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);
            Assert.IsTrue(response.Content.StartsWith(@"{ ""Kind"" : ""Cat"", ""Name"" : ""Kitty""  }-TheValueTHEEND"));
        }



        [TestMethod]
        public void ConstantVariableIncluded()
        {
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Put().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).And.AddToImdb().OverrideProperty("Name", "Garfield").Body("{value}");
            Proxy.Add(simulation1);

            var response = Put("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"" : ""Kitty""  }");
            Assert.AreEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Cat"",""Name"":""Garfield""}", response.Content);
        }

        [TestMethod]
        public void ConstantNullVariableIncluded()
        {
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Put().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).And.AddToImdb().OverrideProperty("Name", null).Body("{value}");
            Proxy.Add(simulation1);

            var response = Put("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"" : ""Kitty""  }");
            Assert.AreEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Cat"",""Name"":null}", response.Content);
        }


        [TestMethod]
        public void SubstitutedPropertyIsReturned()
        {
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Put().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).And.AddToImdb().With.Variable("thename","Garfield").OverrideProperty("Name", "{thename}").Body("{value}");
            Proxy.Add(simulation1);

            var response = Put("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"" : ""Kitty""  }");
            Assert.AreEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Cat"",""Name"":""Garfield""}", response.Content);
        }

        [TestMethod]
        public void SubstitutedPropertyIsReturnedWrapped()
        {
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Put().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).And.AddToImdb().With.Variable("thename", "Garfield").OverrideProperty("Name", "{{thename}}").Body("{{value}");
            Proxy.Add(simulation1);

            var response = Put("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"" : ""Kitty""  }");
            Assert.AreEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);
            Assert.AreEqual(@"{{""Kind"":""Cat"",""Name"":""{Garfield}""}", response.Content);
        }

        [TestMethod]
        public void SubstitutedGuidPropertyIsReturned()
        {
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Put().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).And.AddToImdb().With.Variable("theguid").OverrideProperty("Id", "{theguid}").Body("{value}");
            Proxy.Add(simulation1);

            var response = Put("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"" : ""Kitty""  }");
            Assert.AreEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);
            Assert.IsTrue(response.Content.Contains(@"{""Kind"":""Cat"",""Name"":""Kitty"",""Id"":"));

            var job = JsonConvert.DeserializeObject(response.Content) as JObject;
            var calculatedGuid = job["Id"];
            Guid parsedGuid = default(Guid);
            bool valid = System.Guid.TryParse(calculatedGuid.ToString(), out parsedGuid);
            Assert.IsTrue(valid);
            Assert.AreEqual(calculatedGuid.ToString(), parsedGuid.ToString());
        }



        [TestMethod]
        public void EndToEndSubstituteWithId()
        {
            // Will create a new Guid and return just the GUID in the Body. We will then use that GUID to retrieve the object we have stored. 
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).And.AddToImdb().With.Variable("theguid").OverrideProperty("Id", "{theguid}").Body("{theguid}");
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.When.I.Get().FromImdb("/Pet/{Id}").AsJson().And.Exists().Return.StatusCode(System.Net.HttpStatusCode.NotModified);
            Proxy.Add(simulation2);

            var response = Post("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"" : ""Kitty""  }");
            var guid = response.Content;

            var path = string.Format("/Pet/{0}", guid);
            response = Get(path);
            Assert.AreEqual(System.Net.HttpStatusCode.NotModified, response.StatusCode);
            Assert.IsTrue(response.Content.Contains(@"{""Kind"":""Cat"",""Name"":""Kitty"""));
        }



        // Tests built-in variables that are always returned. ie: {uriroot} contains the 
        [TestMethod]
        public void BuiltInVariablesPost()
        {
            // Some variables are available to all methods as placeholders. 
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).And.AddToImdb().With.Body("{requestroot}-{requestscheme}-{requesthost}-{requestport}");
            Proxy.Add(simulation1);

            var response = Post("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"" : ""Kitty""  }");
            Assert.AreEqual("http://localhost:10011-http-localhost-10011", response.Content);
        }

        [TestMethod]
        public void BuiltInVariablesGet()
        {
            // Some variables are available to all methods as placeholders. 
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Get().FromImdb("/Pet/{Kind}").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).With.Body("{requestroot}-{requestscheme}-{requesthost}-{requestport}");
            Proxy.Add(simulation1);

            var response = Get("/Pet/Dog");
            Assert.AreEqual("http://localhost:10011-http-localhost-10011", response.Content);
        }

        [TestMethod]
        public void BuiltInVariablesHeadNoImdb()
        {
            // Some variables are available to all methods as placeholders. 
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Options().From("/Pet").Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).With.Body("{requestroot}-{requestscheme}-{requesthost}-{requestport}");
            Proxy.Add(simulation1);

            var response = Execute("/Pet", RestSharp.Method.OPTIONS);
            Assert.AreEqual("http://localhost:10011-http-localhost-10011", response.Content);
        }



        [TestMethod]
        public void BuiltInVariablesInResponseHeader()
        {
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).And.AddToImdb().With.Variable("Identity").OverrideProperty("Id", "{Identity}").Header("Location", "{requestroot}/Pet/{Identity}");
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.When.I.Get().FromImdb("/Pet/{Id}").And.Exists("{Id}").Return.StatusCode(System.Net.HttpStatusCode.OK);
            Proxy.Add(simulation2);

            var response = Post("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"" : ""Kitty""  }");
            Assert.AreEqual("", response.Content);

            var header = response.Headers.FirstOrDefault(f => f.Name == "Location");
            Assert.IsNotNull(header);
            var location = header.Value.ToString();

            var guid = location.Substring(location.Length - 36);

            response = Get(location.Substring(location.IndexOf("/Pet/")));
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(response.Content.Contains(guid));
        }

        [TestMethod]
        public void BuiltInVariablesInResponseBody()
        {
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).And.AddToImdb().With.Variable("Identity").OverrideProperty("Id", "{Identity}").Header("Location", "{requestroot}/Pet/{Identity}").And.Body("{Identity}");
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.When.I.Get().FromImdb("/Pet/{Id}").And.Exists("{Id}").Return.StatusCode(System.Net.HttpStatusCode.OK);
            Proxy.Add(simulation2);

            var response = Post("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"" : ""Kitty""  }");
            Assert.AreNotEqual("", response.Content);

            var header = response.Headers.FirstOrDefault(f => f.Name == "Location");
            Assert.IsNotNull(header);
            var location = header.Value.ToString();

            var guid = location.Substring(location.Length - 36);

            // The content of the response should be the same as the Guid in the location. 
            Assert.AreEqual(guid, response.Content);

            response = Get(location.Substring(location.IndexOf("/Pet/")));
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(response.Content.Contains(guid));
        }



        #region Constraints

        [TestMethod]
        public void ConstraintIsMatched()
        {
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").With.Constraint(new LengthEquals("Kind", 3)).Then.Return.StatusCode(System.Net.HttpStatusCode.LengthRequired);
            Proxy.Add(simulation1);

            var response = Post("/Pet", @"{ ""Kind"" : ""Cat"" }");
            Assert.AreEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);
        }

        [TestMethod]
        public void BothConstraintsAreMatched()
        {
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").With.Constraint(new LengthEquals("Kind", 3)).And.Constraint(new LengthEquals("Name", 8)).Then.Return.StatusCode(System.Net.HttpStatusCode.LengthRequired);
            Proxy.Add(simulation1);

            var response = Post("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"":""Garfield"" }");
            Assert.AreEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);
        }

        [TestMethod]
        public void OneOfTwoConstraintsAreMatched()
        {
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").With.Constraint(new LengthEquals("Kind", 3)).And.Constraint(new LengthEquals("Name", 8)).Then.Return.StatusCode(System.Net.HttpStatusCode.LengthRequired);
            Proxy.Add(simulation1);

            var response = Post("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"":""Kitty"" }");
            Assert.AreNotEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);
        }



        [TestMethod]
        public void NoConstraintsAreMatchedReturnsEmptyArrayAsBody()
        {
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").Then.Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).With.Body("{constraintResponses}").And.AddToImdb();
            Proxy.Add(simulation1);

            var response = Post("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"":""Kitty"" }");
            Assert.AreEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);
            Assert.AreEqual("[]", response.Content);
        }

        [TestMethod]
        public void OneMatchingConstraintIsMetAndReturned()
        {
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").With.Constraint(new LengthEquals("Kind", 5)).Then.Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).With.Body("{constraintResponses}-{violationResponses}").And.AddToImdb();
            Proxy.Add(simulation1);

            var response = Post("/Pet", @"{ ""Kind"" : ""Horse"", ""Name"":""Sir Ed"" }");
            Assert.AreEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);
            Assert.AreEqual(@"[{""Name"":""Length"",""PropertyName"":""Kind"",""Kind"":""Equals"",""ExpectedLength"":5,""ActualLength"":5,""PropertyValue"":""Horse"",""PropertyHasValue"":true,""Description"":""The property 'Kind' was expected to be of length '5'.""}]-[]", response.Content);
        }

        [TestMethod]
        public void NoMatchingConstraintsAreMetOrReturned()
        {
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").With.Constraint(new LengthEquals("Kind", 5)).And.HasConstraintViolations().Then.Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).With.Body("{constraintResponses}-{violationResponses}").And.AddToImdb();
            Proxy.Add(simulation1);

            var response = Post("/Pet", @"{ ""Kind"" : ""Horse"", ""Name"":""Sir Ed"" }");
            Assert.AreEqual(System.Net.HttpStatusCode.NotImplemented, response.StatusCode);
        }

        [TestMethod]
        public void OneMatchingViolationIsReturned()
        {
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").With.Constraint(new LengthEquals("Kind", 5)).And.HasConstraintViolations().Then.Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).With.Body("{constraintResponses}-{violationResponses}").And.AddToImdb();
            Proxy.Add(simulation1);

            var response = Post("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"":""Sir Ed"" }");
            Assert.AreEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);
            Assert.AreEqual(@"[]-[{""Name"":""Length"",""PropertyName"":""Kind"",""Kind"":""Equals"",""ExpectedLength"":5,""ActualLength"":3,""PropertyValue"":""Cat"",""PropertyHasValue"":true,""Description"":""The property 'Kind' was expected to be of length '5'.""}]", response.Content);
        }

        [TestMethod]
        public void OneMatchingViolationBetweenIsReturned()
        {
            var simulation1 = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").With.Constraint(new LengthBetween("Kind", 5, 8)).And.HasConstraintViolations().Then.Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).With.Body("{constraintResponses}-{violationResponses}").And.AddToImdb();
            Proxy.Add(simulation1);

            var response = Post("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"":""Sir"" }");
            Assert.AreEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);
            Assert.AreEqual(@"[]-[{""Name"":""Length"",""PropertyName"":""Kind"",""Kind"":""Between"",""MinimumLength"":5,""MaximumLength"":8,""ActualLength"":3,""PropertyValue"":""Cat"",""PropertyHasValue"":true,""Description"":""The property 'Kind' was expected to be between '5' and '8' characters in length (inclusive).""}]", response.Content);
        }

        [TestMethod]
        public void OneMatchingViolationBetweenIsReturnedCustomResponse()
        {
            var between = new LengthBetween("Kind", 5, 8) { Response = @"{""Kind"":""OutOfRangeException"",""PropertyName"":""Kind"",""MinimumLength"":{MinimumLength}}" };

            var simulation1 = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").With.Constraint(between).And.HasConstraintViolations().Then.Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).With.Body("{violationResponses}").And.AddToImdb();
            Proxy.Add(simulation1);

            var response = Post("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"":""Sir"" }");
            Assert.AreEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);
            Assert.AreEqual(@"[{""Kind"":""OutOfRangeException"",""PropertyName"":""Kind"",""MinimumLength"":5}]", response.Content);
        }

        [TestMethod]
        public void OneMatchingViolationBetweenIsReturnedCustomResponsePut()
        {
            var between = new LengthBetween("Kind", 5, 8) { Response = @"{""Kind"":""OutOfRangeException"",""PropertyName"":""Kind"",""MinimumLength"":{MinimumLength}}" };

            var simulation1 = Moksy.Common.SimulationFactory.When.I.Put().ToImdb("/Pet").AsJson().And.NotExists("{Kind}").With.Constraint(between).And.HasConstraintViolations().Then.Return.StatusCode(System.Net.HttpStatusCode.LengthRequired).With.Body("{violationResponses}").And.AddToImdb();
            Proxy.Add(simulation1);

            var response = Put("/Pet", @"{ ""Kind"" : ""Cat"", ""Name"":""Sir"" }");
            Assert.AreEqual(System.Net.HttpStatusCode.LengthRequired, response.StatusCode);
            Assert.AreEqual(@"[{""Kind"":""OutOfRangeException"",""PropertyName"":""Kind"",""MinimumLength"":5}]", response.Content);
        }

        #endregion

        #region Start

        //[TestMethod]
        public void StartNotRunning()
        {
            Proxy proxy = new Proxy(20000);
            var result = proxy.Start();
            Assert.IsTrue(result);
        }

        #endregion

        #region Header Exists and Not Exists

        [TestMethod]
        public void HeaderAndValueExists()
        {
            var simulation = Moksy.Common.SimulationFactory.New("Pf").Get().From("/Something").With.Header("MyHeader", "MyValue").Return.StatusCode(System.Net.HttpStatusCode.NotModified);
            Proxy.Add(simulation);

            var response = Get("/Something", new List<Header>() { new Header("MyHeader", "MyValue") });
            Assert.AreEqual(System.Net.HttpStatusCode.NotModified, response.StatusCode);          
        }

        [TestMethod]
        public void HeaderAndValueBothNotExists()
        {
            var simulation = Moksy.Common.SimulationFactory.New("Pf").Get().From("/Something").With.Header("MyHeader", "MyValue", ComparisonType.NotExists).Return.StatusCode(System.Net.HttpStatusCode.NotModified);
            Proxy.Add(simulation);

            simulation = Moksy.Common.SimulationFactory.New("Pf").Get().From("/Something").With.Header("MyHeader", "MyValue").Return.StatusCode(System.Net.HttpStatusCode.NotModified);
            Proxy.Add(simulation);

            var response = Get("/Something", new List<Header>() { new Header("A", "B") });
            Assert.AreNotEqual(System.Net.HttpStatusCode.NotImplemented, response.StatusCode);
        }

        [TestMethod]
        public void HeaderDoesNotExist()
        {
            var simulation = Moksy.Common.SimulationFactory.New("Pf").Get().From("/Something").With.Header("MyHeader", ComparisonType.NotExists).Return.StatusCode(System.Net.HttpStatusCode.HttpVersionNotSupported).With.Body("MyHeader is missing");
            Proxy.Add(simulation);

            simulation = Moksy.Common.SimulationFactory.New("Pf").Get().From("/Something").With.Header("MyHeader", "MyValue", ComparisonType.NotExists).Return.StatusCode(System.Net.HttpStatusCode.NotModified);
            Proxy.Add(simulation);

            simulation = Moksy.Common.SimulationFactory.New("Pf").Get().From("/Something").With.Header("MyHeader", "MyValue").Return.StatusCode(System.Net.HttpStatusCode.NotModified);
            Proxy.Add(simulation);

            var response = Get("/Something");
            Assert.AreEqual(System.Net.HttpStatusCode.HttpVersionNotSupported, response.StatusCode);
            Assert.AreEqual("MyHeader is missing", response.Content);
        }

        [TestMethod]
        public void HeaderDoesExistDoesNotMatchMissingRule()
        {
            var simulation = Moksy.Common.SimulationFactory.New("Pf").Get().From("/Something").With.Header("MyHeader", ComparisonType.NotExists).Return.StatusCode(System.Net.HttpStatusCode.HttpVersionNotSupported).With.Body("MyHeader is missing");
            Proxy.Add(simulation);

            simulation = Moksy.Common.SimulationFactory.New("Pf").Get().From("/Something").With.Header("MyHeader", "MyValue", ComparisonType.NotExists).Return.StatusCode(System.Net.HttpStatusCode.NotModified);
            Proxy.Add(simulation);

            simulation = Moksy.Common.SimulationFactory.New("Pf").Get().From("/Something").With.Header("MyHeader", "MyValue").Return.StatusCode(System.Net.HttpStatusCode.NotModified);
            Proxy.Add(simulation);

            var response = Get("/Something", new List<Header>() { new Header("A", "B") });
            Assert.AreNotEqual(System.Net.HttpStatusCode.NotImplemented, response.StatusCode);
        }


        #endregion

        #region Body Parameters

        [TestMethod]
        public void NoParametersSpecifiedSoNoMatches()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet", @"a=b");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void OneParameterSpecifiedExactMatch()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a","b").Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet", @"a=b");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void OneParameterOfTwoMatches()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a", "b").Parameter("c", "d").Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet", @"a=b");
            Assert.AreEqual(System.Net.HttpStatusCode.NotImplemented, response.StatusCode);
        }

        [TestMethod]
        public void TwoExactMatches()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a", "b").Parameter("c", "d").Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet", @"a=b&c=d");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void SecondValueMatchesRule()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a", "b").Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a", "g").Then.Return.StatusCode(System.Net.HttpStatusCode.NotAcceptable);
            Proxy.Add(s);

            var response = Post("/Pet", @"a=g");
            Assert.AreEqual(System.Net.HttpStatusCode.NotAcceptable, response.StatusCode); 
        }

        [TestMethod]
        public void ParameterOnlyCaseSensitive()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a").Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a", "g").Then.Return.StatusCode(System.Net.HttpStatusCode.NotAcceptable);
            Proxy.Add(s);

            var response = Post("/Pet", @"a=g");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void ParameterOnlyCaseInsensitive()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a", ComparisonType.CaseInsensitive).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a", "g").Then.Return.StatusCode(System.Net.HttpStatusCode.NotAcceptable);
            Proxy.Add(s);

            var response = Post("/Pet", @"A=g");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void ParameterOnlyCaseSensitiveEncoded()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a/b", ComparisonType.UrlEncode).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a", "g").Then.Return.StatusCode(System.Net.HttpStatusCode.NotAcceptable);
            Proxy.Add(s);

            var response = Post("/Pet", @"a%2fb=g");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void ParameterOnlyCaseSensitiveEncodedNotExists()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a/b", ComparisonType.UrlEncode | ComparisonType.NotExists).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a", "g").Then.Return.StatusCode(System.Net.HttpStatusCode.NotAcceptable);
            Proxy.Add(s);

            var response = Post("/Pet", @"a%2fb=g");
            Assert.AreNotEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void ParameterAndValueCaseSensitive()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a", "g").Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet", @"a=g");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void ParameterAndValueCaseSensitive2()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("A", "g").Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet", @"a=g");
            Assert.AreNotEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void ParameterAndValueCaseInsensitive()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a", "G", ComparisonType.CaseInsensitive).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet", @"A=g");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void ParameterAndValueCaseInsensitiveAndEncoded()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a/B", "G/h", ComparisonType.UrlEncode | ComparisonType.CaseInsensitive).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet", @"A%2fb=g%2fH");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void ParameterAndValueCaseSensitiveAndEncoded()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a/B", "G/h", ComparisonType.UrlEncode).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet", @"A%2fb=g%2fH");
            Assert.AreNotEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        #endregion

        #region Content

        [TestMethod]
        public void NoContentRulesMatch()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet", @"something");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void OneContentRuleDoesNotMatch()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Contains("nothing").Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet", @"something");
            Assert.AreEqual(System.Net.HttpStatusCode.NotImplemented, response.StatusCode);
        }

        [TestMethod]
        public void OneContentRuleDoesMatch()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Contains("thing").Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet", @"something");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void ContentContainsButNotEncodedSoWillMatch()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Contains("t/hing").Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet", @"somet/hing");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void ContentContainsButEncodedSoWillNotMatch()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Contains("t/hing", ComparisonType.UrlEncode).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet", @"somet/hing");
            Assert.AreNotEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void ContentContainsButEncodedSoWillMatch()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Contains("t/hing", ComparisonType.UrlEncode).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet", @"somet%2fhing");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void ContentContainsButEncodedSoWillMatchCaseSensitive()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Contains("t/hing", ComparisonType.UrlEncode | ComparisonType.CaseSensitive).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet", @"somet%2fhing");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void ContentNotContainsButEncodedSoWillMatchCaseSensitive()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Contains("t/hing", ComparisonType.UrlEncode | ComparisonType.CaseSensitive | ComparisonType.NotContains).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet", @"somet%2fhing");
            Assert.AreNotEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void ContentContainsButEncodedSoWillNotMatchCaseSensitive()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Contains("t/hing", ComparisonType.UrlEncode | ComparisonType.CaseSensitive).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet", @"somet%2fHINg");
            Assert.AreNotEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        #endregion Content

        [TestMethod]
        public void ResponseHeaderWithContentType()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Contains("thing").Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices).With.Header("Content-Type", "type/xml").And.Body("something");
            Proxy.Add(s);

            var response = Post("/Pet", @"something");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);

            Assert.IsNotNull(response.Headers.FirstOrDefault(f => f.Name == "Content-Type"));
        }

        [TestMethod]
        public void ResponseHeaderWithContentTypeCaseSensitive()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Contains("thing").Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices).With.Header("Content-Type", "type/xml").And.Body("something");
            Proxy.Add(s);

            var response = Post("/Pet", @"something");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);

            Assert.IsNotNull(response.Headers.FirstOrDefault(f => f.Name == "Content-Type"));
        }

        [TestMethod]
        public void ResponseHeaderWithContentTypeCaseSensitive2()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Contains("thing", true).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices).With.Header("Content-Type", "type/xml").And.Body("something");
            Proxy.Add(s);

            var response = Post("/Pet", @"something");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);

            Assert.IsNotNull(response.Headers.FirstOrDefault(f => f.Name == "Content-Type"));
        }

        [TestMethod]
        public void ResponseHeaderWithContentTypeCaseInsensitive()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Contains("thing", false).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices).With.Header("Content-Type", "type/xml").And.Body("something");
            Proxy.Add(s);

            var response = Post("/Pet", @"someTHING");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);

            Assert.IsNotNull(response.Headers.FirstOrDefault(f => f.Name == "Content-Type"));
        }

        #region Query Parameters Match

        [TestMethod]
        public void QueryParameterDoesNotMatch()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a", "b", ParameterType.UrlParameter).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet?d=e", @"Hello");
            Assert.AreNotEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void QueryParameterMatch()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a", "b", ParameterType.UrlParameter).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet?a=b", @"Hello");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void TwoParametersNotEncoded()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a", "b", ParameterType.UrlParameter).Parameter("d", "e", ParameterType.UrlParameter).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet?a=b&d=e", @"Hello");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void TwoParametersOneEncodedOneNotWIllMatch()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("m/n", "o/p", ComparisonType.UrlEncode, ParameterType.UrlParameter).Parameter("d", "e", ParameterType.UrlParameter).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet?m%2fn=o%2fp&d=e", @"Hello");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void TwoParametersOneEncodedOneNotCaseSensitiveNoMatch()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("m/n", "o/p", ComparisonType.UrlEncode, ParameterType.UrlParameter).Parameter("d", "e", ParameterType.UrlParameter).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet?m%2fn=o%2Fp&d=E", @"Hello");
            Assert.AreNotEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void TwoParametersOneEncodedOneNotCaseInsensitiveNoMatch()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("m/n", "o/p", ComparisonType.UrlEncode | ComparisonType.CaseInsensitive, ParameterType.UrlParameter).Parameter("d", "e", ParameterType.UrlParameter).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet?m%2fN=o%2Fp&d=e", @"Hello");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void OneUrlParameterExistsMatches()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a", "b", ComparisonType.Exists, ParameterType.UrlParameter).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet?a=b", @"Hello");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void OneUrlParameterNotExistsMatches()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a", "b", ComparisonType.NotExists, ParameterType.UrlParameter).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet?a=b", @"Hello");
            Assert.AreNotEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void OneUrlParameterExistsMatches2()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a", "b", ComparisonType.NotExists, ParameterType.UrlParameter).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet?d=e", @"Hello");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }


        [TestMethod]
        public void OneUrlParameterNotExistsNoValue()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a", ComparisonType.NotExists, ParameterType.UrlParameter).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet?d=e", @"Hello");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode); 
        }

        [TestMethod]
        public void OneUrlParameterPartialMatchCaseSensitive()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a", "bc", ComparisonType.Exists | ComparisonType.PartialValue, ParameterType.UrlParameter).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet?a=bcd", @"Hello");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode); 
        }

        [TestMethod]
        public void OneUrlParameterPartialMatchCaseInsensitive()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a", "bc", ComparisonType.Exists | ComparisonType.PartialValue | ComparisonType.CaseInsensitive, ParameterType.UrlParameter).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet?a=bCd", @"Hello");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [TestMethod]
        public void OneUrlParameterNotExistsNoValueImplicitExists()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().To("/Pet").Parameter("a", ParameterType.UrlParameter).Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s);

            var response = Post("/Pet?d=e&a=b", @"Hello");
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        #endregion

        #region Octet Stream / Binary Content

        [TestMethod]
        public void GetEmptyBinaryContent()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body(new byte[0]).And.Header("Content-Type", "application/octet-stream");
            Proxy.Add(s);

            var response = Get("/Pet");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(0, response.RawBytes.Length);
        }

        [TestMethod]
        public void GetLength1BinaryContent()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body(new byte[1] { (byte)'a'} ).And.Header("Content-Type", "application/octet-stream");
            Proxy.Add(s);

            var response = Get("/Pet");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(1, response.RawBytes.Length);
            Assert.AreEqual((byte)'a', response.RawBytes[0]);
        }

        [TestMethod]
        public void GetFile()
        {
            var bytes = System.IO.File.ReadAllBytes(System.IO.Path.Combine(TestContext.DeploymentDirectory, "TestData", "ec2.png"));

            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet/ec2.png").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body(bytes).And.Header("Content-Type", "image/png");
            Proxy.Add(s);

            var response = Get("/Pet/ec2.png");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(bytes.Length, response.RawBytes.Length);

            Assert.IsTrue(bytes.SequenceEqual(response.RawBytes));
            Assert.IsNotNull(response.Headers.FirstOrDefault(f => f.Name == "Content-Type" && f.Value.ToString() == "image/png"));
        }

        #endregion
    }
}
