using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Common;
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
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Product").Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices).With.AddToImdb();
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
        public void PostAndGetAsJsonSingleEntry()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Product").AsJson().Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired).AddToImdb();
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
            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Product").AsJson().Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired).AddToImdb();
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
            var simulation1 = Moksy.Common.SimulationFactory.New("First").I.Post().ToImdb("/Product").AsJson().Return.StatusCode(System.Net.HttpStatusCode.ProxyAuthenticationRequired).AddToImdb();
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
    }
}
