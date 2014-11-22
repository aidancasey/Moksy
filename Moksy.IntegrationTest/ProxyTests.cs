using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.IntegrationTest
{
    [TestClass]
    public class ProxyTests : TestBase
    {
        public ProxyTests()
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
        public void SanityNoSimulations()
        {
            var all = Proxy.GetAll();
            Assert.AreEqual(0, all.Count());
        }

        [TestMethod]
        public void AddOneSimulationAndGetAll()
        {
            var simulation = Moksy.Common.SimulationFactory.When.Get().From("/Product").With.Header("MyHeader", "HeaderValue").Return.Body("This content").With.StatusCode(System.Net.HttpStatusCode.PreconditionFailed);
            Proxy.Add(simulation);

            var all = Proxy.GetAll();
            Assert.AreEqual(1, all.Count());

            var first = all.First();
            Assert.AreEqual("/Product", first.Condition.SimulationConditionContent.Pattern);

            Assert.AreEqual(HttpMethod.Get, first.Condition.SimulationConditionContent.HttpMethod);

            Assert.AreEqual(1, first.Condition.RequestHeaders.Count);
            Assert.AreEqual("MyHeader", first.Condition.RequestHeaders[0].Name);
            Assert.AreEqual("HeaderValue", first.Condition.RequestHeaders[0].Value);

            Assert.AreEqual(System.Net.HttpStatusCode.PreconditionFailed, first.Response.SimulationResponseContent.HttpStatusCode);

            Assert.AreEqual("This content", first.Response.SimulationResponseContent.Content);
        }

        [TestMethod]
        public void AddOneSimulationAndGetByName()
        {
            var simulation = Moksy.Common.SimulationFactory.New("FirstOne").Get().From("/Product").With.Header("MyHeader", "HeaderValue").Return.Body("This content").With.StatusCode(System.Net.HttpStatusCode.PreconditionFailed);
            Proxy.Add(simulation);

            var existing = Proxy.GetByName("FirstOne");
            Assert.IsNotNull(existing);
        }

        [TestMethod]
        public void AddOneSimulationAndGetByNameFailsBecauseCaseSensitive()
        {
            var simulation = Moksy.Common.SimulationFactory.New("FirstOne").Get().From("/Product").With.Header("MyHeader", "HeaderValue").Return.Body("This content").With.StatusCode(System.Net.HttpStatusCode.PreconditionFailed);
            Proxy.Add(simulation);

            var existing = Proxy.GetByName("FirstONE");
            Assert.IsNull(existing);
        }

        [TestMethod]
        public void AddOneSimulationAndGetByNameNotExist()
        {
            var simulation = Moksy.Common.SimulationFactory.New("FirstOne").Get().From("/Product").With.Header("MyHeader", "HeaderValue").Return.Body("This content").With.StatusCode(System.Net.HttpStatusCode.PreconditionFailed);
            Proxy.Add(simulation);

            var existing = Proxy.GetByName("FirstOneNotExist");
            Assert.IsNull(existing);
        }

        [TestMethod]
        public void AddOnSimulationAndDeleteIt()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").Get().From("/Product").With.Header("MyHeader", "HeaderValue").Return.Body("This content").With.StatusCode(System.Net.HttpStatusCode.PreconditionFailed);
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.New("Second").Get().From("/Product").With.Header("MyHeader", "HeaderValue").Return.Body("This content").With.StatusCode(System.Net.HttpStatusCode.PreconditionFailed);
            Proxy.Add(simulation2);

            var all = Proxy.GetAll();
            Assert.AreEqual(2, all.Count());

            var response = Proxy.DeleteByName("Second");
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response);

            all = Proxy.GetAll();
            Assert.AreEqual(1, all.Count());

            var match = all.FirstOrDefault(f => f.Name == "First");
            Assert.IsNotNull(match);

            match = all.FirstOrDefault(f => f.Name == "Second");
            Assert.IsNull(match);
        }



        [TestMethod]
        public void AddOnSimulationAndDeleteItButLeaveData()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").Get().FromImdb("/Pet").Return.StatusCode(System.Net.HttpStatusCode.OK);
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.New("Second").Post().ToImdb("/Pet").Then.Return.StatusCode(System.Net.HttpStatusCode.Created).And.AddToImdb();
            Proxy.Add(simulation2);

            var response = Post("/Pet", new Pet() { Kind = "Dog" });

            var code = Proxy.DeleteByName("Second");
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, code);

            response = Get("/Pet");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Dog""}", response.Content);
        }

        [TestMethod]
        public void AddOnSimulationAndDeleteItButRemoveData()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").Get().FromImdb("/Pet").Return.StatusCode(System.Net.HttpStatusCode.OK);
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.New("Second").Post().ToImdb("/Pet").Then.Return.StatusCode(System.Net.HttpStatusCode.Created).And.AddToImdb();
            Proxy.Add(simulation2);

            var response = Post("/Pet", new Pet() { Kind = "Dog" });

            var code = Proxy.DeleteByName("Second", true);
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, code);

            response = Get("/Pet");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(@"", response.Content);
        }



        [TestMethod]
        public void DeleteSimulationDataRetainRule()
        {
            var simulation1 = Moksy.Common.SimulationFactory.New("First").Get().FromImdb("/Pet").Return.StatusCode(System.Net.HttpStatusCode.OK).Return.Body("StillExists");
            Proxy.Add(simulation1);

            var simulation2 = Moksy.Common.SimulationFactory.New("Second").Post().ToImdb("/Pet").Then.Return.StatusCode(System.Net.HttpStatusCode.Created).And.AddToImdb();
            Proxy.Add(simulation2);

            var response = Post("/Pet", new Pet() { Kind = "Dog" });

            var code = Proxy.DeleteByName("First", true, true);
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, code);

            response = Get("/Pet");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(@"StillExists", response.Content);
        }



        [TestMethod]
        public void ConfigureRemoteMachine()
        {
            Proxy proxy = new Proxy(10011);
            proxy.Start();


        }


        /*
        [TestMethod]
        public void ProxyExit()
        {
            var all = Proxy.GetAll();
            Assert.IsNotNull(all);

            Proxy.Exit();

            try
            {
                all = Proxy.GetAll();
                Assert.Fail("It was expected the service would be finished by now. ");
            }
            catch (System.Net.WebException wex)
            {

            }
        }
        

        [TestMethod]
        public void StartRemoteMachine()
        {
            // Start it on the local host - we will treat this instance as a remote connection. 
            Proxy localProxy = new Moksy.Proxy(30011);
            Assert.IsTrue(localProxy.Start());
            Assert.IsTrue(localProxy.IsLocalhost);

            // This simulates a remove machine by passing in the local machine name as the Proxy Host. 
            var hostname = System.Environment.ExpandEnvironmentVariables("%COMPUTERNAME%");

            var proxy = new Proxy(hostname, 30011);
            Assert.IsTrue(proxy.Start());
            Assert.IsFalse(proxy.IsLocalhost);

            var s = Moksy.Common.SimulationFactory.New("First").Get().From("/Pet").Return.StatusCode(System.Net.HttpStatusCode.OK).Body("Dog");
            proxy.Add(s);

            RestSharp.IRestClient client = new RestSharp.RestClient(proxy.Root);
            RestSharp.IRestRequest request = new RestSharp.RestRequest("/Pet", RestSharp.Method.GET);
            request.RequestFormat = RestSharp.DataFormat.Json;
            var response = client.Execute(request);
            Assert.AreEqual("Dog", response.Content);
        }
        */
        public TestContext TestContext { get; set; }
    }
}
