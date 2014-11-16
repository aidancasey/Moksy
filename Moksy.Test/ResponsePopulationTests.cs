using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Test
{
    /// <summary>
    /// Tests to ensure that the HttpResponseMessage is correctly populated with the Headers in the SimulationResponse. 
    /// </summary>
    [TestClass]
    public class ResponsePopulationTests
    {
        public ResponsePopulationTests()
        {
        }



        [TestMethod]
        public void Null()
        {
            var message = HttpResponseMessageFactory.New(null);
            Assert.IsNotNull(message);
            Assert.AreEqual(null, message.Content);
            Assert.AreEqual(0, message.Headers.Count());
            Assert.AreEqual(System.Net.HttpStatusCode.OK, message.StatusCode);
        }

        [TestMethod]
        public void Uninitialized()
        {
            SimulationResponse response = new SimulationResponse();

            var message = HttpResponseMessageFactory.New(response);
            Assert.AreEqual(System.Net.HttpStatusCode.Unused, message.StatusCode);
            Assert.AreEqual(null, message.Content);
            Assert.AreEqual(0, message.Headers.Count());
        }



        [TestMethod]
        public void StatusCodeIs404()
        {
            SimulationResponse response = new SimulationResponse();
            response.With.StatusCode(System.Net.HttpStatusCode.NotFound);

            var message = HttpResponseMessageFactory.New(response);
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, message.StatusCode);
        }

        [TestMethod]
        public void ContentIsFixedString()
        {
            SimulationResponse response = new SimulationResponse();
            response.Body("This is the content that is returned.");

            var message = HttpResponseMessageFactory.New(response);
            StringContent stringContent = message.Content as StringContent;
            Assert.IsNotNull(stringContent);
            var task = stringContent.ReadAsStringAsync();
            task.Wait();
            Assert.AreEqual("This is the content that is returned.", task.Result);
        }


        [TestMethod]
        public void SingleHeaderInResponse()
        {
            SimulationResponse response = new SimulationResponse();
            response.Header("H1", "H2");

            var message = HttpResponseMessageFactory.New(response);
            Assert.AreEqual(1, message.Headers.Count());
            var headers = message.Headers.ToArray();
            Assert.AreEqual("H1", headers[0].Key);
            Assert.AreEqual("H2", headers[0].Value.ToArray()[0]);
        }



        [TestMethod]
        public void Mega()
        {
            SimulationResponse response = new SimulationResponse();
            response.Body("Some content.").StatusCode(System.Net.HttpStatusCode.MultipleChoices).Header("Woo", "Hoo");

            var message = HttpResponseMessageFactory.New(response);
            StringContent stringContent = message.Content as StringContent;
            var task = stringContent.ReadAsStringAsync();
            task.Wait();
            Assert.AreEqual("Some content.", task.Result);

            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, message.StatusCode);

            Assert.AreEqual(1, message.Headers.Count());
            var headers = message.Headers.ToArray();
            Assert.AreEqual("Woo", headers[0].Key);
            Assert.AreEqual("Hoo", headers[0].Value.ToArray()[0]);
        }


        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void AddToImdbWithoutSpecifyingImdbUsage()
        {
            var s = SimulationFactory.When.I.Post().To("/Endpoint").Then.Return.And.AddToImdb();
        }

        [TestMethod]
        public void AddToImdbWithUsageSpecifiedTrue()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Endpoint").Then.Return.And.AddToImdb();
            Assert.IsTrue(s.AddImdb);
        }

        [TestMethod]
        public void AddToImdbWithUsageSpecifiedFalse()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Endpoint").Then.Return.And.AddToImdb(false);
            Assert.IsFalse(s.AddImdb);
        }



        [TestMethod]
        public void ContentIsStringImplicit()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Endpoint").Then.Return.StatusCode(System.Net.HttpStatusCode.OK);
            Assert.AreEqual(ContentKind.Text, s.ContentKind);
        }

        [TestMethod]
        public void ContentIsStringExplicit()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Endpoint").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body("theString");
            Assert.AreEqual(ContentKind.Text, s.ContentKind);
        }

        [TestMethod]
        public void ContentIsString()
        {
            var g = new { A = "aValue" };
            var s = SimulationFactory.When.I.Post().ToImdb("/Endpoint").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body(g);
            Assert.AreEqual(ContentKind.Text, s.ContentKind);
        }



        [TestMethod]
        public void IsOctet()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Endpoint").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body(new byte[0]);
            Assert.AreEqual(ContentKind.Octet, s.ContentKind);
        }
    }
}
