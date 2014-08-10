using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.IntegrationTest
{
    /// <summary>
    /// Contains a number of samples to show how to use the API. 
    /// </summary>
    /// <remarks>The easiest way to get started is to:
    /// 1. Launch Visual Studio as Administrator. 
    /// 2. Run Moksy.Host (right click project and add the port number to the Debug / Command Line Arguments. ie: 10011). 
    /// 3. Provide the same Port number to the Proxy class in these tests. 
    /// 4. Step through the tests in this class to see how it works. 
    /// </remarks>
    [TestClass]
    public class DocumentationTests : TestBase
    {
        public DocumentationTests()
        {
            var processes = System.Diagnostics.Process.GetProcessesByName("Moksy.Host");
            Assert.AreEqual(1, processes.Length, "Moksy.Host is not running. Right click the Moksy.Host project, add 10011 to the Debug / Command Line Arguments and then run Moksy.Host without a debugger. Then try to run this test again. ");

            Proxy = new Proxy(PortNumber);
            Assert.IsTrue(Proxy.Start(), string.Format("Moksy could not start. Try to manually launch 'Moksy.Host.exe {0}' from the Command Line. ", PortNumber));
            Proxy.DeleteAll();
        }

        Proxy Proxy;
        int PortNumber = 10011;

        #region Simple

        /// <summary>
        /// Set up a canned response so that GET /Pet is called, a single object is returned. 
        /// </summary>
        [TestMethod]
        public void SimpleGet()
        {
            var pet = new Pet() { Kind = "Dog" };

            var simulation = SimulationFactory.When.I.Get().From("/Pet").Then.Return.Body(pet).And.StatusCode(System.Net.HttpStatusCode.OK);
            Proxy.Add(simulation);

            // Now manually navigate to http://localhost:[PortNumber]/Pet to retrieve that response. 
        }

        #endregion // Manual

        #region Imdb

        /// <summary>
        /// Set up a POST, GET, DELETE Imdb to store and retrieve objects from an in memory database using the 'Kind' property as an index. 
        /// </summary>
        /// <remarks>This is more or else the pattern you follow 
        /// </remarks>
        [TestMethod]
        public void EndToEndSimulation()
        {
            // 1. Set up our simulations. We will use an in-memory database and ensure uniqueness on the 'Kind' property of all submnitted Json objects. 
            // 2. Add some test data. 
            // 3. Now hit the end-point directly by submitting DELETE, GET and POST requests to exercise the simulations. 
            //    In practice: you set up the simulations and then invoke another service which hits the simulation. This allows you to fake third party end-points. 

            // 1.
            Proxy.Add(SimulationFactory.When.I.Post().ToImdb("/Pet").And.Exists("Kind").Then.Return.StatusCode(System.Net.HttpStatusCode.BadRequest).With.Body("The Pet already exists. "));
            Proxy.Add(SimulationFactory.When.I.Post().ToImdb("/Pet").And.NotExists("Kind").Then.Return.StatusCode(System.Net.HttpStatusCode.Created).With.Body("{value}").And.AddToImdb());

            Proxy.Add(SimulationFactory.When.I.Get().FromImdb("/Pet").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body("[{value}]"));
            Proxy.Add(SimulationFactory.When.I.Get().FromImdb("/Pet/{Kind}").And.Exists("Kind").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body("{value}"));
            Proxy.Add(SimulationFactory.When.I.Get().FromImdb("/Pet/{Kind}").And.NotExists("Kind").Then.Return.StatusCode(System.Net.HttpStatusCode.NotFound).With.Body("Pet Not Found."));

            Proxy.Add(SimulationFactory.When.I.Delete().FromImdb("/Pet/{Kind}").With.Header("YourHeader", "YourHeaderValue").And.Exists("Kind").Then.Return.StatusCode(System.Net.HttpStatusCode.NoContent).And.RemoveFromImdb());
            Proxy.Add(SimulationFactory.When.I.Delete().FromImdb("/Pet/{Kind}").And.Exists("Kind").Then.Return.StatusCode(System.Net.HttpStatusCode.NoContent).And.RemoveFromImdb());
            Proxy.Add(SimulationFactory.When.I.Delete().FromImdb("/Pet/{Kind}").And.NotExists("Kind").Then.Return.StatusCode(System.Net.HttpStatusCode.NoContent));

            // 2. 
            var response = Post("/Pet", new Pet() { Kind = "Dog" });
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
            Post("/Pet", new Pet() { Kind = "Cat" });
            Post("/Pet", new Pet() { Kind = "Rabbit" });
            Post("/Pet", new Pet() { Kind = "Gerbil" });
            Post("/Pet", new Pet() { Kind = "Snake" });



            // 3.
            // Get calls to exercise the above (navigate to the same Url from a Browser to check the response). 
            response = Get("/Pet/Dog");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(@"{""Kind"":""Dog""}", response.Content);

            response = Get("/Pet/Lion");
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);

            // Try a post something that already exists
            response = Post("/Pet", new Pet() { Kind = "Dog" });
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual(response.Content, "The Pet already exists. ");

            // Try to delete an item that does not exist. 
            response = Delete("/Pet/Lion");
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response.StatusCode);
            Assert.AreEqual("", response.Content);

            // ... and one that does. 
            response = Delete("/Pet/Gerbil");
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response.StatusCode);
            Assert.AreEqual("", response.Content);
            response = Get("/Pet/Gerbil");
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);

            // ... and delete using Headers. 
            List<Header> headers = new List<Header>();
            headers.Add(new Header() { Name = "YourHeader", Value = "YourHeaderValue" });
            response = Delete("/Pet/Snake", headers);
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response.StatusCode);

            // Hit /Pet with a GET (no parameters). This will return all of the values. Notice that GET /Pet will return a Body of [{value}]
            // {value} is the comma-separated list of all entries in the database provided by Moksy; you typically want to wrap collections depending on what end-point
            // you are simulation. ie: raw collections, OData and so forth. 
            response = Get("/Pet");
            Assert.IsTrue(response.Content.StartsWith("["));
            Assert.IsTrue(response.Content.Contains("Dog"));
            Assert.IsTrue(response.Content.EndsWith("]"));
        }

        #endregion
    }
}
