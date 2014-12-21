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
    public class ImdbNestedTests : TestBase 
    {
        public ImdbNestedTests()
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
        public void LookupHeavilyNested()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet/{Kind}/Toy/{Name}/Price", "owner").And.NotExists("{Ranking}").Then.AddToImdb().And.Return.StatusCode(System.Net.HttpStatusCode.Created);
            Proxy.Add(s);

            s = SimulationFactory.When.I.Post().ToImdb("/Pet/{Kind}/Toy/{Name}/Price", "owner").And.Exists("{Ranking}").Then.Return.StatusCode(System.Net.HttpStatusCode.Conflict);
            Proxy.Add(s);

            var response = Post("/Pet/Dog/Toy/Bone/Price", @"{""Ranking"":""CheapMe""}", new Header[] { new Header("owner", "me") });
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);

            response = Post("/Pet/Dog/Toy/Bone/Price", @"{""Ranking"":""CheapMe""}", new Header[] { new Header("owner", "me") });
            Assert.AreEqual(System.Net.HttpStatusCode.Conflict, response.StatusCode);
            /*
            //
            // To start with: just use one discriminator to create the objects. 
            //
            var result = Database.AddJson("/Pet/Dog/Toy/Bone/Price", "/Pet/{Kind}/Toy/{Name}/Price", "Ranking", @"{""Ranking"":""CheapMe""}", "me");
            Assert.IsTrue(result);

            result = Database.AddJson("/Pet/Dog/Toy/Bone/Price", "/Pet/{Kind}/Toy/{Name}/Price", "Ranking", @"{""Ranking"":""ExpensiveMe""}", "me");
            Assert.IsTrue(result);

            result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""GerbilMe""}", "me");
            Assert.IsTrue(result);

            result = Database.AddJson("/Pet/Cat/Toy", "/Pet/{Kind}/Toy", "Name", @"{""Name"":""WheelMe""}", "me");
            Assert.IsTrue(result);

            // Pet
            Assert.AreEqual(1, Database.GetResources("me").Count);
            Assert.AreEqual("Pet", Database.GetResources("me")[0].Name);

            // /Pet/{Kind} (/Pet/Dog)
            Assert.AreEqual(3, Database.GetResources("me")[0].Resources.Count);
            Assert.AreEqual(@"{""Kind"":""Dog""}", Database.GetResources("me")[0].Data()[0].Json);
            Assert.AreEqual(@"{""Kind"":""GerbilMe""}", Database.GetResources("me")[0].Data()[1].Json);
            Assert.AreEqual(@"{""Kind"":""Cat""}", Database.GetResources("me")[0].Data()[2].Json);

            // /Pet/{Kind}/Toy
            Assert.AreEqual(1, Database.GetResources("me")[0].Resources[0].Resources.Count);
            Assert.AreEqual("Toy", Database.GetResources("me")[0].Resources[0].Resources[0].Name);
            Assert.AreEqual(@"{""Name"":""Bone""}", Database.GetResources("me")[0].Resources[0].Resources[0].Data()[0].Json);

            // /Pet/{Kind}/Toy/{Name}/Price
            Assert.AreEqual(2, Database.GetResources("me")[0].Resources[0].Resources[0].Resources[0].Resources[0].Resources.Count);
            Assert.AreEqual("Price", Database.GetResources("me")[0].Resources[0].Resources[0].Resources[0].Resources[0].Name);
            Assert.AreEqual(@"{""Ranking"":""CheapMe""}", Database.GetResources("me")[0].Resources[0].Resources[0].Resources[0].Resources[0].Data()[0].Json);
            Assert.AreEqual(@"{""Ranking"":""ExpensiveMe""}", Database.GetResources("me")[0].Resources[0].Resources[0].Resources[0].Resources[0].Data()[1].Json);

            //
            // Now add nested (discriminated) items. 
            //
            result = Database.AddJson("/Pet/Dog/Toy/Bone/Price", "/Pet/{Kind}/Toy/{Name}/Price", "Ranking", @"{""Ranking"":""CheapYou""}", "you");
            Assert.IsTrue(result);

            result = Database.AddJson("/Pet/Dog/Toy/Bone/Price", "/Pet/{Kind}/Toy/{Name}/Price", "Ranking", @"{""Ranking"":""ExpensiveYou""}", "you");
            Assert.IsTrue(result);

            result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""GerbilYou""}", "you");
            Assert.IsTrue(result);

            result = Database.AddJson("/Pet/Cat/Toy", "/Pet/{Kind}/Toy", "Name", @"{""Name"":""WheelYou""}", "you");
            Assert.IsTrue(result);

            // Pet
            Assert.AreEqual(1, Database.GetResources("you").Count);
            Assert.AreEqual("Pet", Database.GetResources("you")[0].Name);

            // /Pet/{Kind} (/Pet/Dog)
            Assert.AreEqual(3, Database.GetResources("you")[0].Resources.Count);
            Assert.AreEqual(@"{""Kind"":""Dog""}", Database.GetResources("you")[0].Data()[0].Json);
            Assert.AreEqual(@"{""Kind"":""GerbilYou""}", Database.GetResources("you")[0].Data()[1].Json);
            Assert.AreEqual(@"{""Kind"":""Cat""}", Database.GetResources("you")[0].Data()[2].Json);

            // /Pet/{Kind}/Toy
            Assert.AreEqual(1, Database.GetResources("you")[0].Resources[0].Resources.Count);
            Assert.AreEqual("Toy", Database.GetResources("you")[0].Resources[0].Resources[0].Name);
            Assert.AreEqual(@"{""Name"":""Bone""}", Database.GetResources("you")[0].Resources[0].Resources[0].Data()[0].Json);

            // /Pet/{Kind}/Toy/{Name}/Price
            Assert.AreEqual(2, Database.GetResources("you")[0].Resources[0].Resources[0].Resources[0].Resources[0].Resources.Count);
            Assert.AreEqual("Price", Database.GetResources("you")[0].Resources[0].Resources[0].Resources[0].Resources[0].Name);
            Assert.AreEqual(@"{""Ranking"":""CheapYou""}", Database.GetResources("you")[0].Resources[0].Resources[0].Resources[0].Resources[0].Data()[0].Json);
            Assert.AreEqual(@"{""Ranking"":""ExpensiveYou""}", Database.GetResources("you")[0].Resources[0].Resources[0].Resources[0].Resources[0].Data()[1].Json);

            //
            // Exists: 
            //
            var lookup = Database.Lookup("/Pet/Dog/Toy/Bone/Price", "/Pet/{Kind}/Toy/{Name}/Price", "Ranking", "CheapYou", "you");
            Assert.IsNotNull(lookup);

            lookup = Database.Lookup("/Pet/Dog/Toy/Bone/Price", "/Pet/{Kind}/Toy/{Name}/Price", "Ranking", "CheapMe", "you");
            Assert.IsNull(lookup);

            lookup = Database.Lookup("/Pet/Dog/Toy/Bone/Price", "/Pet/{Kind}/Toy/{Name}/Price", "Ranking", "CheapMe", "me");
            Assert.IsNotNull(lookup);

            //
            // Remove
            //
            var removed = Database.Remove("/Pet/Dog/Toy/Bone/Price", "/Pet/{Kind}/Toy/{Name}/Price", "Ranking", "CheapYou", "you");
            Assert.IsTrue(removed);

            lookup = Database.Lookup("/Pet/Dog/Toy/Bone/Price", "/Pet/{Kind}/Toy/{Name}/Price", "Ranking", "CheapYou", "you");
            Assert.IsNull(lookup);

            lookup = Database.Lookup("/Pet/Dog/Toy/Bone/Price", "/Pet/{Kind}/Toy/{Name}/Price", "Ranking", "CheapMe", "you");
            Assert.IsNull(lookup);

            lookup = Database.Lookup("/Pet/Dog/Toy/Bone/Price", "/Pet/{Kind}/Toy/{Name}/Price", "Ranking", "CheapMe", "me");
            Assert.IsNotNull(lookup);*/
        }

    }
}
