using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moksy.Common;
namespace Moksy.IntegrationTest.Imdb
{
    /// <summary>
    /// Tests that files can be uploaded / downloaded to an Imdb. 
    /// </summary>
    [TestClass]
    [DeploymentItem("TestData", "TestData")]
    public class FileTests : TestBase 
    {
        public FileTests()
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
        public void PostAndGetRoundTrip()
        {
            // When posting a file, the system will create an internal variable called BinaryContentIdentifier. Use this variable as part of a header or Body response to be able to locate the binary
            // content after it was uploaded. 
            var s = SimulationFactory.When.I.Post().ToImdb("/Storage").AsBinary().Then.AddToImdb().And.Return.StatusCode(System.Net.HttpStatusCode.Created).With.Header("Location", "{requestroot}/Storage/{BinaryContentIdentity}");
            Proxy.Add(s);

            s = SimulationFactory.When.I.Get().FromImdb("/Storage/{BinaryContentIdentity}").AsBinary().And.Exists("{BinaryContentIdentity}").Then.Return.StatusCode(System.Net.HttpStatusCode.OK);
            Proxy.Add(s);

            var path = System.IO.Path.Combine(TestContext.DeploymentDirectory, "TestData", "ec2.png");

            var response = PostFile("/Storage", path);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
            var header = response.Headers.FirstOrDefault(f => f.Name == "Location");
            Assert.IsNotNull(header, "There was no Location header returned. ");

            var location = System.Convert.ToString(header.Value);

            // Now GET from that location. 
            Uri uri = new Uri(location);
            response = Get(uri.PathAndQuery);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);

            // The raw bytes should be equal to the original. 
            var originalBytes = System.IO.File.ReadAllBytes(path);
            Assert.IsTrue(originalBytes.SequenceEqual(response.RawBytes));
        }

        [TestMethod]
        public void DeleteFile()
        {
            // When posting a file, the system will create an internal variable called BinaryContentIdentifier. Use this variable as part of a header or Body response to be able to locate the binary
            // content after it was uploaded. 
            var s = SimulationFactory.When.I.Post().ToImdb("/Storage").AsBinary().Then.AddToImdb().And.Return.StatusCode(System.Net.HttpStatusCode.Created).With.Header("Location", "{requestroot}/Storage/{BinaryContentIdentity}");
            Proxy.Add(s);

            s = SimulationFactory.When.I.Get().FromImdb("/Storage/{BinaryContentIdentity}").AsBinary().And.Exists("{BinaryContentIdentity}").Then.Return.StatusCode(System.Net.HttpStatusCode.OK);
            Proxy.Add(s);

            s = SimulationFactory.When.I.Get().FromImdb("/Storage/{BinaryContentIdentity}").AsBinary().And.NotExists("{BinaryContentIdentity}").Then.Return.StatusCode(System.Net.HttpStatusCode.NotFound);
            Proxy.Add(s);

            s = SimulationFactory.When.I.Delete().FromImdb("/Storage/{BinaryContentIdentity}").AsBinary().And.Exists("{BinaryContentIdentity}").Then.Return.StatusCode(System.Net.HttpStatusCode.NoContent).And.RemoveFromImdb();
            Proxy.Add(s);

            s = SimulationFactory.When.I.Delete().FromImdb("/Storage/{BinaryContentIdentity}").AsBinary().And.NotExists("{BinaryContentIdentity}").Then.Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices).And.RemoveFromImdb();
            Proxy.Add(s);

            var path = System.IO.Path.Combine(TestContext.DeploymentDirectory, "TestData", "ec2.png");

            var response = PostFile("/Storage", path);
            var header = response.Headers.FirstOrDefault(f => f.Name == "Location");
            var location = System.Convert.ToString(header.Value);
            
            Uri uri = new Uri(location);

            response = Delete(uri.PathAndQuery);
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response.StatusCode);
            response = Delete(uri.PathAndQuery);
            Assert.AreEqual(System.Net.HttpStatusCode.MultipleChoices, response.StatusCode);

            response = Get(uri.PathAndQuery);
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }



        [TestMethod]
        public void PutRoundTrip()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Storage").AsBinary().Then.AddToImdb().And.Return.StatusCode(System.Net.HttpStatusCode.Created).With.Header("Location", "{requestroot}/Storage/{BinaryContentIdentity}");
            Proxy.Add(s);

            s = SimulationFactory.When.I.Put().ToImdb("/Storage/{BinaryContentIdentity}").AsBinary().And.Exists("{BinaryContentIdentity}").Then.AddToImdb().And.Return.StatusCode(System.Net.HttpStatusCode.NoContent);
            Proxy.Add(s);

            s = SimulationFactory.When.I.Put().ToImdb("/Storage/{BinaryContentIdentity}").AsBinary().And.NotExists("{BinaryContentIdentity}").Then.AddToImdb().And.Return.StatusCode(System.Net.HttpStatusCode.NotFound);
            Proxy.Add(s);

            s = SimulationFactory.When.I.Get().FromImdb("/Storage/{BinaryContentIdentity}").AsBinary().And.Exists("{BinaryContentIdentity}").Then.Return.StatusCode(System.Net.HttpStatusCode.OK);
            Proxy.Add(s);

            var path = System.IO.Path.Combine(TestContext.DeploymentDirectory, "TestData", "ec2.png");
            var response = PostFile("/Storage", path);
            var header = response.Headers.FirstOrDefault(f => f.Name == "Location");
            var location = System.Convert.ToString(header.Value);

            Uri uri = new Uri(location);

            // Now update the File. 
            path = System.IO.Path.Combine(TestContext.DeploymentDirectory, "TestData", "SimpleSample.json");
            response = PutFile(uri.PathAndQuery, path);
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response.StatusCode);

            // Now get the file and make sure it is equal to the updated one; not the original. 
            response = Get(uri.PathAndQuery);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);

            // The raw bytes should be equal to the original. 
            var originalBytes = System.IO.File.ReadAllBytes(path);
            Assert.IsTrue(originalBytes.SequenceEqual(response.RawBytes));
        }
    }
}
