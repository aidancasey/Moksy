using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.IntegrationTest
{
    [TestClass]
    public class ImdbHeaderTests : TestBase
    {
        public ImdbHeaderTests()
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
        public void PostWithoutHeaderDoesNotMatch()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet", "owner").Then.AddToImdb().With.StatusCode(System.Net.HttpStatusCode.Created);
            Proxy.Add(s);

            var response = Post("/Pet", "{}");
            Assert.AreEqual(System.Net.HttpStatusCode.NotImplemented, response.StatusCode);
        }

        [TestMethod]
        public void PostWithHeaderDoesMatch()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet", "owner").Then.AddToImdb().With.StatusCode(System.Net.HttpStatusCode.Created);
            Proxy.Add(s);

            var headers = new List<Header>() { new Header("owner", "me") };
            var response = Post("/Pet", @"{""Kind"":""Dog""}", headers);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
        }

        [TestMethod]
        public void PostWithHeaderAndGetWithHeaderExactMatch()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet", "owner").Then.AddToImdb().With.StatusCode(System.Net.HttpStatusCode.Created);
            Proxy.Add(s);

            s = SimulationFactory.When.I.Get().FromImdb("/Pet/{Kind}", "owner").Then.Return.Body("{value}").With.StatusCode(System.Net.HttpStatusCode.PreconditionFailed);
            Proxy.Add(s);

            var headers = new List<Header>() { new Header("owner", "me") };
            var response = Post("/Pet", @"{""Kind"":""Dog""}", headers);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);

            response = Get("/Pet/Dog", headers);
            Assert.AreEqual(System.Net.HttpStatusCode.PreconditionFailed, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Dog""}", response.Content);
        }

        [TestMethod]
        public void PostWithHeaderAndGetWithoutHeaderDoesNotMatch()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet", "owner").Then.AddToImdb().With.StatusCode(System.Net.HttpStatusCode.Created);
            Proxy.Add(s);

            s = SimulationFactory.When.I.Get().FromImdb("/Pet/{Kind}", "owner").Then.Return.Body("{value}").With.StatusCode(System.Net.HttpStatusCode.PreconditionFailed);
            Proxy.Add(s);

            var headers = new List<Header>() { new Header("owner", "me") };
            var response = Post("/Pet", @"{""Kind"":""Dog""}", headers);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);

            response = Get("/Pet/Dog");
            Assert.AreEqual(System.Net.HttpStatusCode.NotImplemented, response.StatusCode);
        }



        [TestMethod]
        public void StoreValueWithHeaderDiscriminator()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet", "owner").Then.AddToImdb().With.StatusCode(System.Net.HttpStatusCode.Created);
            Proxy.Add(s);

            s = SimulationFactory.When.I.Get().FromImdb("/Pet/{Kind}", "owner").Then.Return.Body("{value}").With.StatusCode(System.Net.HttpStatusCode.PreconditionFailed);
            Proxy.Add(s);

            var me = new List<Header>() { new Header("owner", "me") };
            var you = new List<Header>() { new Header("owner", "you") };
            var response = Post("/Pet", @"{""Kind"":""Dog"",""Name"":""K9""}", me);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
            response = Post("/Pet", @"{""Kind"":""Dog"",""Name"":""Rover""}", you);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);

            response = Get("/Pet/Dog", me);
            Assert.AreEqual(System.Net.HttpStatusCode.PreconditionFailed, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Dog"",""Name"":""K9""}", response.Content);

            response = Get("/Pet/Dog", you);
            Assert.AreEqual(System.Net.HttpStatusCode.PreconditionFailed, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Dog"",""Name"":""Rover""}", response.Content);
        }



        [TestMethod]
        public void PutValueWithHeaderDiscriminator()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet", "owner").Then.AddToImdb().With.StatusCode(System.Net.HttpStatusCode.Created);
            Proxy.Add(s);

            s = SimulationFactory.When.I.Get().FromImdb("/Pet/{Kind}", "owner").Then.Return.Body("{value}").With.StatusCode(System.Net.HttpStatusCode.PreconditionFailed);
            Proxy.Add(s);

            s = Moksy.Common.SimulationFactory.When.I.Put().ToImdb("/Pet/{Kind}", "owner").AsJson().And.Exists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.ExpectationFailed).And.AddToImdb();
            Proxy.Add(s);

            var me = new List<Header>() { new Header("owner", "me") };
            var you = new List<Header>() { new Header("owner", "you") };

            var response = Post("/Pet", @"{""Kind"":""Dog"",""Name"":""K9""}", me);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);

            response = Post("/Pet", @"{""Kind"":""Dog"",""Name"":""Rover""}", you);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);

            response = Put("/Pet/Dog", @"{""Kind"":""Dog"",""Name"":""Lassie""}", you);
            Assert.AreEqual(System.Net.HttpStatusCode.ExpectationFailed, response.StatusCode);

            response = Get("/Pet/Dog", me);
            Assert.AreEqual(System.Net.HttpStatusCode.PreconditionFailed, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Dog"",""Name"":""K9""}", response.Content);

            response = Get("/Pet/Dog", you);
            Assert.AreEqual(System.Net.HttpStatusCode.PreconditionFailed, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Dog"",""Name"":""Lassie""}", response.Content);
        }

        [TestMethod]
        public void PutValueRouteDifferentFromObjectWithHeaderDiscriminator()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet", "owner").Then.AddToImdb().With.StatusCode(System.Net.HttpStatusCode.Created);
            Proxy.Add(s);

            s = SimulationFactory.When.I.Get().FromImdb("/Pet/{Kind}", "owner").Then.Return.Body("{value}").With.StatusCode(System.Net.HttpStatusCode.PreconditionFailed);
            Proxy.Add(s);

            s = Moksy.Common.SimulationFactory.When.I.Put().ToImdb("/Pet/{Kind}", "owner").AsJson().And.Exists("{Kind}").Then.OverrideProperty("Kind", "{Kind}").And.Return.StatusCode(System.Net.HttpStatusCode.ExpectationFailed).And.AddToImdb();
            Proxy.Add(s);

            var me = new List<Header>() { new Header("owner", "me") };
            var you = new List<Header>() { new Header("owner", "you") };

            var response = Post("/Pet", @"{""Kind"":""Dog"",""Name"":""K9""}", me);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);

            response = Post("/Pet", @"{""Kind"":""Dog"",""Name"":""Rover""}", you);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);

            // By setitng this to Lizard, we are expecting the value in the path to override the value in the object. 
            response = Put("/Pet/Dog", @"{""Kind"":""Lizard"",""Name"":""Lassie""}", you);
            Assert.AreEqual(System.Net.HttpStatusCode.ExpectationFailed, response.StatusCode);

            response = Get("/Pet/Dog", me);
            Assert.AreEqual(System.Net.HttpStatusCode.PreconditionFailed, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Dog"",""Name"":""K9""}", response.Content);

            response = Get("/Pet/Dog", you);
            Assert.AreEqual(System.Net.HttpStatusCode.PreconditionFailed, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Dog"",""Name"":""Lassie""}", response.Content);
        }



        [TestMethod]
        public void PutValueNotInPath()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet", "owner").Then.AddToImdb().With.StatusCode(System.Net.HttpStatusCode.Created);
            Proxy.Add(s);

            s = SimulationFactory.When.I.Get().FromImdb("/Pet/{Kind}", "owner").Then.Return.Body("{value}").With.StatusCode(System.Net.HttpStatusCode.PreconditionFailed);
            Proxy.Add(s);

            s = Moksy.Common.SimulationFactory.When.I.Put().ToImdb("/Pet", "owner").AsJson().And.Exists("{Kind}").Then.Return.StatusCode(System.Net.HttpStatusCode.ExpectationFailed).And.AddToImdb();
            Proxy.Add(s);

            var me = new List<Header>() { new Header("owner", "me") };
            var you = new List<Header>() { new Header("owner", "you") };

            var response = Post("/Pet", @"{""Kind"":""Dog"",""Name"":""K9""}", me);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);

            response = Post("/Pet", @"{""Kind"":""Dog"",""Name"":""Rover""}", you);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);

            // By setitng this to Lizard, we are expecting the value in the path to override the value in the object. 
            response = Put("/Pet", @"{""Kind"":""Dog"",""Name"":""Lassie""}", you);
            Assert.AreEqual(System.Net.HttpStatusCode.ExpectationFailed, response.StatusCode);

            response = Get("/Pet/Dog", me);
            Assert.AreEqual(System.Net.HttpStatusCode.PreconditionFailed, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Dog"",""Name"":""K9""}", response.Content);

            response = Get("/Pet/Dog", you);
            Assert.AreEqual(System.Net.HttpStatusCode.PreconditionFailed, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Dog"",""Name"":""Lassie""}", response.Content);
        }



        [TestMethod]
        public void DeleteByKindHeaderDiscriminator()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet", "owner").Then.AddToImdb().With.StatusCode(System.Net.HttpStatusCode.Created);
            Proxy.Add(s);

            s = SimulationFactory.When.I.Get().FromImdb("/Pet/{Kind}", "owner").And.NotExists("{Kind}").Then.Return.Body("{value}").With.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
            Proxy.Add(s); 
            
            s = SimulationFactory.When.I.Get().FromImdb("/Pet/{Kind}", "owner").And.Exists("{Kind}").Then.Return.Body("{value}").With.StatusCode(System.Net.HttpStatusCode.PreconditionFailed);
            Proxy.Add(s);

            s = SimulationFactory.When.I.Delete().FromImdb("/Pet/{Kind}", "owner").And.Exists("{Kind}").Then.Return.Body("{value}").With.StatusCode(System.Net.HttpStatusCode.NotAcceptable).And.RemoveFromImdb();
            Proxy.Add(s);

            var me = new List<Header>() { new Header("owner", "me") };
            var you = new List<Header>() { new Header("owner", "you") };

            var response = Post("/Pet", @"{""Kind"":""Dog"",""Name"":""K9""}", me);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);

            response = Post("/Pet", @"{""Kind"":""Dog"",""Name"":""Rover""}", you);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);

            // By setitng this to Lizard, we are expecting the value in the path to override the value in the object. 
            response = Delete("/Pet/Dog", you);
            Assert.AreEqual(System.Net.HttpStatusCode.NotAcceptable, response.StatusCode);

            response = Get("/Pet/Dog", me);
            Assert.AreEqual(System.Net.HttpStatusCode.PreconditionFailed, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Dog"",""Name"":""K9""}", response.Content);

            response = Get("/Pet/Dog", you);
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);
        }
    }
}
