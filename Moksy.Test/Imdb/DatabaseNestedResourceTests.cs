using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moksy.Storage;

namespace Moksy.Test.Imdb
{
    /// <summary>
    /// Tests for complex (nested) resource Imdb's. ie: /Pet/{kind}/Toy/{name} or /Pet/{kind}/{toyname} (without explicit resources)
    /// </summary>
    [TestClass]
    public class DatabaseNestedResourceTests
    {
        public DatabaseNestedResourceTests()
        {
        }

        [TestInitialize]
        public void Init()
        {
            Database = new Database();
        }

        internal Database Database;


        #region Database - Add/Delete where multiple resources are in use 

        [TestMethod]
        public void AddResourcePropertyResourceObject()
        {
            var result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""Dog""}");
            result = Database.AddJson("/Pet/Dog/Toy", "/Pet/{Kind}/Toy", "Name", @"{""Name"":""Bone""}");
            Assert.IsTrue(result);

            // /Pet
            Assert.AreEqual(1, Database.GetResources().Count);
            Assert.AreEqual("Pet", Database.GetResources()[0].Name);

            // /Pet/{Kind} (/Pet/Dog)
            Assert.AreEqual(1, Database.GetResources()[0].Resources.Count);
            Assert.AreEqual("Dog", Database.GetResources()[0].Resources[0].Name);
            Assert.AreEqual(@"{""Kind"":""Dog""}", Database.GetResources()[0].Resources[0].Data()[0].Json);

            // /Pet/{Kind}/Toy
            Assert.AreEqual(1, Database.GetResources()[0].Resources[0].Resources.Count);
            Assert.AreEqual("Toy", Database.GetResources()[0].Resources[0].Resources[0].Name);
            Assert.AreEqual("Bone", Database.GetResources()[0].Resources[0].Resources[0].Resources[0].Name);
            Assert.AreEqual(@"{""Name"":""Bone""}", Database.GetResources()[0].Resources[0].Resources[0].Resources[0].Data()[0].Json);
            
            //
            // Exists
            // 
            result = Database.Exists("/Pet", "/Pet", "Kind", "Dog");
            Assert.IsTrue(result);
            
            result = Database.Exists("/Pet", "/Pet", "Kind", "Cat");
            Assert.IsFalse(result);

            result = Database.Exists("/Pet/Dog/Toy", "/Pet/{Kind}/Toy", "Name", "Bone");
            Assert.IsTrue(result);

            result = Database.Exists("/Pet/Dog/Toy", "/Pet/{Kind}/Toy", "Name", "Wheel");
            Assert.IsFalse(result);

            //
            // Remove
            //
            Database.Remove("/Pet/Dog/Toy", "/Pet/{Kind}/Toy", "Name", "Bone");

            result = Database.Exists("/Pet/Dog/Toy", "/Pet/{Kind}/Toy", "Name", "Bone");
            Assert.IsFalse(result);

            // NOTE: This is 1 because the Discriminator/Entry pair is NOT purged when we remove the last entry. 
            Assert.AreEqual(1, Database.GetResources()[0].Resources[0].Resources.Count);

        }

        [TestMethod]
        public void AddResourcePropertyResourceObject2()
        {
            // In this case, the Toy is created without a Dog record; the Dog record should be created. 
            var result = Database.AddJson("/Pet/Dog/Toy", "/Pet/{Kind}/Toy", "Name", @"{""Name"":""Bone""}");
            Assert.IsTrue(result);

            // /Pet
            Assert.AreEqual(1, Database.GetResources().Count);

            // /Pet/{Kind} (/Pet/Dog)
            Assert.AreEqual(1, Database.GetResources()[0].Resources.Count);
            Assert.AreEqual(@"{""Kind"":""Dog""}", Database.GetResources()[0].Resources[0].Data()[0].Json);

            // /Pet/{Kind}/Toy
            Assert.AreEqual(1, Database.GetResources()[0].Resources[0].Resources.Count);
            Assert.AreEqual("Toy", Database.GetResources()[0].Resources[0].Resources[0].Name);
            Assert.AreEqual(@"{""Name"":""Bone""}", Database.GetResources()[0].Resources[0].Resources[0].Resources[0].Data()[0].Json);
        }

        [TestMethod]
        public void AddProperty()
        {
            var result = Database.AddJson("/Dog", "/{Kind}", "Kind", @"{""Kind"":""Dog""}");
            Assert.IsTrue(result);

            // /Dog
            // NOTE: An "EMPTY" Resource will be created first. 
            Assert.AreEqual(1, Database.GetResources().Count);
            Assert.AreEqual("", Database.GetResources()[0].Name);

            Assert.AreEqual(1, Database.GetResources()[0].Resources.Count);
            Assert.AreEqual(@"{""Kind"":""Dog""}", Database.GetResources()[0].Resources[0].Data()[0].Json);
            Assert.AreEqual(0, Database.GetResources()[0].Resources[0].Resources.Count);
        }

        [TestMethod]
        public void AddPropertyProperty()
        {
            // In this case, the "parent" resource is created first. 
            var result = Database.AddJson("/Dog", "/{Kind}", "Kind", @"{""Kind"":""Dog""}");
            result = Database.AddJson("/Dog/Bone", "/{Kind}/{Name}", "Name", @"{""Name"":""Bone""}");
            result = Database.AddJson("/Dog/Chew", "/{Kind}/{Name}", "Name", @"{""Name"":""Chew""}");
            Assert.IsTrue(result);

            // Blank
            // Dog
            // Blank
            // Two resources
            //     Each of the two resources contains a single entry - Name/Bone; and Name/Chew

            // /Dog
            // NOTE: An "EMPTY" Resource will be created first. 
            Assert.AreEqual(1, Database.GetResources().Count);
            Assert.AreEqual("", Database.GetResources()[0].Name);

            Assert.AreEqual(1, Database.GetResources()[0].Resources.Count);
            Assert.AreEqual(@"{""Kind"":""Dog""}", Database.GetResources()[0].Resources[0].Data()[0].Json);

            // Dog/Bone
            Assert.AreEqual(1, Database.GetResources()[0].Resources[0].Resources.Count);
            Assert.AreEqual("", Database.GetResources()[0].Resources[0].Resources[0].Name);

            Assert.AreEqual("Bone", Database.GetResources()[0].Resources[0].Resources[0].Resources[0].Name);
            Assert.AreEqual("Chew", Database.GetResources()[0].Resources[0].Resources[0].Resources[1].Name);
            Assert.AreEqual(@"{""Name"":""Bone""}", Database.GetResources()[0].Resources[0].Resources[0].Resources[0].Data()[0].Json);
            Assert.AreEqual(@"{""Name"":""Chew""}", Database.GetResources()[0].Resources[0].Resources[0].Resources[1].Data()[0].Json);
        }

        [TestMethod]
        public void AddPropertyProperty2()
        {
            // In this case, the "parent" resource is NOT created first. 
            // var result = Database.AddJson("/Dog", "/{Kind}", "Kind", @"{""Kind"":""Dog""}");
            var result = Database.AddJson("/Dog/Bone", "/{Kind}/{Name}", "Name", @"{""Name"":""Bone""}");
            Assert.IsTrue(result);

            // /Dog
            // NOTE: An "EMPTY" Resource will be created first. 
            Assert.AreEqual(1, Database.GetResources().Count);
            Assert.AreEqual("", Database.GetResources()[0].Name);

            Assert.AreEqual(1, Database.GetResources()[0].Resources.Count);
            Assert.AreEqual(@"{""Kind"":""Dog""}", Database.GetResources()[0].Resources[0].Data()[0].Json);

            // Dog/Bone
            Assert.AreEqual(1, Database.GetResources()[0].Resources[0].Resources[0].Resources.Count);
            Assert.AreEqual("", Database.GetResources()[0].Resources[0].Resources[0].Name);
            Assert.AreEqual("Bone", Database.GetResources()[0].Resources[0].Resources[0].Resources[0].Name);
            Assert.AreEqual(@"{""Name"":""Bone""}", Database.GetResources()[0].Resources[0].Resources[0].Resources[0].Data()[0].Json);
        }

        [TestMethod]
        public void AddMega()
        {
            // In this case, the "parent" resource is created first. 
            var result = Database.AddJson("/Dog", "/{Kind}", "Kind", @"{""Kind"":""Dog""}");
            result = Database.AddJson("/Cat", "/{Kind}", "Kind", @"{""Kind"":""Cat""}");

            result = Database.AddJson("/Dog/Bone", "/{Kind}/{Name}", "Name", @"{""Name"":""Bone""}");
            result = Database.AddJson("/Dog/Thing", "/{Kind}/{Name}", "Name", @"{""Name"":""Thing""}");

            result = Database.AddJson("/Cat/Ball", "/{Kind}/{Name}", "Name", @"{""Name"":""Ball""}");

            result = Database.AddJson("/Gerbil/Wheel", "/{Kind}/{Name}", "Name", @"{""Name"":""Wheel""}");

            // /Dog
            // NOTE: An "EMPTY" Resource will be created first. 
            Assert.AreEqual(1, Database.GetResources().Count);
            Assert.AreEqual("", Database.GetResources()[0].Name);

            Assert.AreEqual(3, Database.GetResources()[0].Resources.Count);
            Assert.AreEqual(@"{""Kind"":""Dog""}", Database.GetResources()[0].Resources[0].Data()[0].Json);
            Assert.AreEqual(@"{""Kind"":""Cat""}", Database.GetResources()[0].Resources[1].Data()[0].Json);
            Assert.AreEqual(@"{""Kind"":""Gerbil""}", Database.GetResources()[0].Resources[2].Data()[0].Json);

            // Dog/Bone
            Assert.AreEqual(1, Database.GetResources()[0].Resources[0].Resources.Count);
            Assert.AreEqual("", Database.GetResources()[0].Resources[0].Resources[0].Name);
            Assert.AreEqual(2, Database.GetResources()[0].Resources[0].Resources[0].Resources.Count);
            Assert.AreEqual(@"{""Name"":""Bone""}", Database.GetResources()[0].Resources[0].Resources[0].Resources[0].Data()[0].Json);
            Assert.AreEqual(@"{""Name"":""Thing""}", Database.GetResources()[0].Resources[0].Resources[0].Resources[1].Data()[0].Json);
        }


        /*
        [TestMethod]
        public void AddEntryFirstEntryPresent()
        {
            Assert.Fail("This test is redundant - and wrong. Update this to test nested resources");

            var result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""Dog""}");
            result = Database.AddJson("/Pet/Dog/Toy", "/Pet/{Kind}/Toy", "Name", @"{""Name"":""Bone""}");
            Assert.IsTrue(result);

            Assert.AreEqual(1, Database.Resources.Count);
            Assert.AreEqual("Pet", Database.Resources[0].Name);
            Assert.AreEqual("Toy", Database.Resources[1].Name);

            Assert.AreEqual(1, Database.Resources[0].Data().Count);
            Assert.AreEqual(@"{""Kind"":""Dog""}", Database.Resources[0].Data()[0].Json);

            Assert.AreEqual(1, Database.Resources[0].Data()[0].Resources[0].Data().Count);
            Assert.AreEqual(@"{""Name"":""Bone""}", Database.Resources[0].Data()[0].Json);
        }

        [TestMethod]
        public void TopLevelAddedFirst()
        {
            // Add /Pet
            // Then add /Pet/Toy/Bone
            var result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""Dog""}");
            result = Database.AddJson("/Pet/Dog/Toy", "/Pet/{Kind}/Toy", "Name", @"{""Name"":""Bone""}");
            Assert.IsTrue(result);

            // One top level resources. 
            Assert.AreEqual(1, Database.Resources.Count);
            Assert.AreEqual("Pet", Database.Resources[0].Name);

            // Pet has one nested resource - Toy
            Assert.AreEqual(1, Database.Resources[0].Data()[0].Resources.Count);
            Assert.AreEqual("Toy", Database.Resources[1].Data()[0].Resources[0].Name);

            // Pet has a single entry - Dog. 
            Assert.AreEqual(1, Database.Resources[0].Data().Count);
            Assert.AreEqual(@"{""Kind"":""Dog""}", Database.Resources[0].Data()[0].Json);

            // Pet/Dog/Toy has a single entry - bone. 
            Assert.AreEqual(1, Database.Resources[0].Data()[0].Resources[0].Data()[0].Resources[0].Data()[0].Json);
            Assert.AreEqual(@"{""Name"":""Bone""}", Database.Resources[0].Data()[0].Json);
        }*/
        #endregion


        #region Resources

        [TestMethod]
        public void ResourceSanity()
        {
            var resource = new Resource(null);
            Assert.AreEqual(0, resource.Data().Count);
        }

        [TestMethod]
        public void ResourceSanityNull()
        {
            var resource = new Resource(null);
            Assert.IsNull(resource.Name);
        }

        [TestMethod]
        public void ResourceSanityEmpty()
        {
            var resource = new Resource("");
            Assert.AreEqual("", resource.Name);
        }

        public void ResourceSanityValid()
        {
            var resource = new Resource("Pet");
            Assert.AreEqual("Pet", resource.Name);
        }

        #endregion

        [TestMethod]
        public void DatabaseSanity()
        {
            var db = new Database();
            Assert.AreEqual(0, db.GetResources().Count);
        }

        #region Database - Add/Delete where only one resource is in use

        [TestMethod]
        public void AddEntry()
        {
            var result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""Dog""}");
            Assert.IsTrue(result);

            Assert.AreEqual(1, Database.GetResources().Count);
            Assert.AreEqual("Pet", Database.GetResources()[0].Name);
            Assert.AreEqual(1, Database.GetResources()[0].Resources[0].Data().Count);
            Assert.AreEqual(@"{""Kind"":""Dog""}", Database.GetResources()[0].Resources[0].Data()[0].Json);

            Assert.AreEqual(1, Database.GetResources()[0].Resources.Count);
        }

        [TestMethod]
        public void AddTwoEntries()
        {
            var result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""Dog""}");
            Assert.IsTrue(result);
            result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""Cat""}");
            Assert.IsTrue(result);

            Assert.AreEqual(1, Database.GetResources().Count);
            Assert.AreEqual("Pet", Database.GetResources()[0].Name);
            Assert.AreEqual(2, Database.GetResources()[0].Resources.Count);
            Assert.AreEqual(@"{""Kind"":""Dog""}", Database.GetResources()[0].Resources[0].Data()[0].Json);
            Assert.AreEqual(@"{""Kind"":""Cat""}", Database.GetResources()[0].Resources[1].Data()[0].Json);
        }

        [TestMethod]
        public void AddMissingPropertyEntry()
        {
            var result = Database.AddJson("/Pet", "/Pet", "NoneExistentProperty", @"{""Kind"":""Dog""}");
            Assert.AreEqual(1, Database.GetResources().Count);
            Assert.AreEqual(1, Database.GetResources()[0].Resources.Count);
            Assert.AreEqual(@"{""Kind"":""Dog""}", Database.GetResources()[0].Resources[0].Data()[0].Json);
        }

        [TestMethod]
        public void AddMissingPropertyEntryTwice()
        {
            var result = Database.AddJson("/Pet", "/Pet", "NoneExistentProperty", @"{""Kind"":""Dog""}");
            result = Database.AddJson("/Pet", "/Pet", "NoneExistentProperty", @"{""Kind"":""Cat""}");
            Assert.AreEqual(1, Database.GetResources().Count);
            Assert.AreEqual(1, Database.GetResources()[0].Resources.Count);
            Assert.AreEqual(@"{""Kind"":""Cat""}", Database.GetResources()[0].Resources[0].Data()[0].Json);
        }

        [TestMethod]
        public void AddMissingPropertyEntryAndEmptyProperty()
        {
            var result = Database.AddJson("/Pet", "/Pet", "NoneExistentProperty", @"{""Kind"":""Dog""}");
            result = Database.AddJson("/Pet", "/Pet", "NoneExistentProperty", @"{""NoneExistentProperty"":"""",""Kind"":""Cat""}");
            Assert.AreEqual(1, Database.GetResources().Count);
            Assert.AreEqual(2, Database.GetResources()[0].Resources.Count);
            Assert.AreEqual(@"{""Kind"":""Dog""}", Database.GetResources()[0].Resources[0].Data()[0].Json);
            Assert.AreEqual(@"{""NoneExistentProperty"":"""",""Kind"":""Cat""}", Database.GetResources()[0].Resources[1].Data()[0].Json);
        }

        [TestMethod]
        [ExpectedException(typeof(Newtonsoft.Json.JsonReaderException))]
        public void AddThrowsInvalidArgumentExceptionWithInvalidJson()
        {
            var result = Database.AddJson("/Pet", "/Pet", "Kind", @"{ this is not } json }");
        }



        [TestMethod]
        public void AddEntryAlreadyExistsButCaseSensitive()
        {
            var result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""Dog""}");
            result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""DOG""}");
            Assert.IsTrue(result);

            Assert.AreEqual(1, Database.GetResources().Count);
            Assert.AreEqual("Pet", Database.GetResources()[0].Name);
            Assert.AreEqual(2, Database.GetResources()[0].Resources.Count);
            Assert.AreEqual(@"{""Kind"":""Dog""}", Database.GetResources()[0].Resources[0].Data()[0].Json);
            Assert.AreEqual(@"{""Kind"":""DOG""}", Database.GetResources()[0].Resources[1].Data()[0].Json);
        }

        [TestMethod]
        public void AddEntryAlreadyExistsReplaces()
        {
            var result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""Dog"",""A"":""B""}");
            result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""Dog"",""C"":""D""}");
            Assert.IsTrue(result);

            Assert.AreEqual(1, Database.GetResources().Count);
            Assert.AreEqual("Pet", Database.GetResources()[0].Name);
            Assert.AreEqual(1, Database.GetResources()[0].Resources[0].Data().Count);
            Assert.AreEqual(@"{""Kind"":""Dog"",""C"":""D""}", Database.GetResources()[0].Resources[0].Data()[0].Json);
        }



        #endregion

        #region Lookup

        [TestMethod]
        public void LookupNoneExistentProperty()
        {
            var result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""Dog"",""A"":""B""}");
            var data = Database.Lookup("/Pet", "/Pet", "NoneExistent", "theValue");
            Assert.IsNull(data);
        }

        [TestMethod]
        public void LookupNoneExistentPropertyThatIsNullWillMatch()
        {
            var result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""Dog""}");
            var data = Database.Lookup("/Pet", "/Pet", "NoneExistent", null);
            Assert.AreEqual(@"{""Kind"":""Dog""}", data);
        }

        [TestMethod]
        public void LookupIsNullInEmptyDatabase()
        {
            var data = Database.Lookup("/Pet", "/Pet", "NoneExistent", "theValue");
            Assert.IsNull(data);
        }

        [TestMethod]
        public void LookupWithOneEntryAndExistingProperty()
        {
            var result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""Dog""}");
            result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""Cat""}");

            var dog = Database.Lookup("/Pet", "/Pet", "Kind", "Dog");
            Assert.AreEqual(@"{""Kind"":""Dog""}", dog);

            var cat = Database.Lookup("/Pet", "/Pet", "Kind", "Cat");
            Assert.AreEqual(@"{""Kind"":""Cat""}", cat);

            var snake = Database.Lookup("/Pet", "/Pet", "Kind", "Snake");
            Assert.IsNull(snake);
        }

        [TestMethod]
        public void LookupHeavilyNested()
        {
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
            Assert.AreEqual(@"{""Kind"":""Dog""}", Database.GetResources("me")[0].Resources[0].Data()[0].Json);
            Assert.AreEqual(@"{""Kind"":""GerbilMe""}", Database.GetResources("me")[0].Resources[1].Data()[0].Json);
            Assert.AreEqual(@"{""Kind"":""Cat""}", Database.GetResources("me")[0].Resources[2].Data()[0].Json);

            // /Pet/{Kind}/Toy
            Assert.AreEqual(1, Database.GetResources("me")[0].Resources[0].Resources.Count);
            Assert.AreEqual("Toy", Database.GetResources("me")[0].Resources[0].Resources[0].Name);
            Assert.AreEqual("Bone", Database.GetResources("me")[0].Resources[0].Resources[0].Resources[0].Name);
            Assert.AreEqual(@"{""Name"":""Bone""}", Database.GetResources("me")[0].Resources[0].Resources[0].Resources[0].Data()[0].Json);

            // /Pet/{Kind}/Toy/{Name}/Price
            Assert.AreEqual(2, Database.GetResources("me")[0].Resources[0].Resources[0].Resources[0].Resources[0].Resources.Count);
            Assert.AreEqual("Price", Database.GetResources("me")[0].Resources[0].Resources[0].Resources[0].Resources[0].Name);
            Assert.AreEqual("CheapMe", Database.GetResources("me")[0].Resources[0].Resources[0].Resources[0].Resources[0].Resources[0].Name);
            Assert.AreEqual("ExpensiveMe", Database.GetResources("me")[0].Resources[0].Resources[0].Resources[0].Resources[0].Resources[1].Name);
            Assert.AreEqual(@"{""Ranking"":""CheapMe""}", Database.GetResources("me")[0].Resources[0].Resources[0].Resources[0].Resources[0].Resources[0].Data()[0].Json);
            Assert.AreEqual(@"{""Ranking"":""ExpensiveMe""}", Database.GetResources("me")[0].Resources[0].Resources[0].Resources[0].Resources[0].Resources[1].Data()[0].Json);  
   
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
            Assert.AreEqual(@"{""Kind"":""Dog""}", Database.GetResources("you")[0].Resources[0].Data()[0].Json);
            Assert.AreEqual(@"{""Kind"":""GerbilYou""}", Database.GetResources("you")[0].Resources[1].Data()[0].Json);
            Assert.AreEqual(@"{""Kind"":""Cat""}", Database.GetResources("you")[0].Resources[2].Data()[0].Json);

            // /Pet/{Kind}/Toy
            Assert.AreEqual(1, Database.GetResources("you")[0].Resources[0].Resources.Count);
            Assert.AreEqual("Toy", Database.GetResources("you")[0].Resources[0].Resources[0].Name);
            Assert.AreEqual("Bone", Database.GetResources("you")[0].Resources[0].Resources[0].Resources[0].Name);
            Assert.AreEqual(@"{""Name"":""Bone""}", Database.GetResources("you")[0].Resources[0].Resources[0].Resources[0].Data()[0].Json);

            // /Pet/{Kind}/Toy/{Name}/Price
            Assert.AreEqual(2, Database.GetResources("you")[0].Resources[0].Resources[0].Resources[0].Resources[0].Resources.Count);
            Assert.AreEqual("Price", Database.GetResources("you")[0].Resources[0].Resources[0].Resources[0].Resources[0].Name);
            Assert.AreEqual("CheapYou", Database.GetResources("you")[0].Resources[0].Resources[0].Resources[0].Resources[0].Resources[0].Name);
            Assert.AreEqual("ExpensiveYou", Database.GetResources("you")[0].Resources[0].Resources[0].Resources[0].Resources[0].Resources[1].Name);
            Assert.AreEqual(@"{""Ranking"":""CheapYou""}", Database.GetResources("you")[0].Resources[0].Resources[0].Resources[0].Resources[0].Resources[0].Data()[0].Json);
            Assert.AreEqual(@"{""Ranking"":""ExpensiveYou""}", Database.GetResources("you")[0].Resources[0].Resources[0].Resources[0].Resources[0].Resources[1].Data()[0].Json);  

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
            // Remove item
            //
            var removed = Database.Remove("/Pet/Dog/Toy/Bone/Price", "/Pet/{Kind}/Toy/{Name}/Price", "Ranking", "CheapYou", "you");
            Assert.IsTrue(removed);

            lookup = Database.Lookup("/Pet/Dog/Toy/Bone/Price", "/Pet/{Kind}/Toy/{Name}/Price", "Ranking", "CheapYou", "you");
            Assert.IsNull(lookup);

            lookup = Database.Lookup("/Pet/Dog/Toy/Bone/Price", "/Pet/{Kind}/Toy/{Name}/Price", "Ranking", "CheapMe", "you");
            Assert.IsNull(lookup);

            lookup = Database.Lookup("/Pet/Dog/Toy/Bone/Price", "/Pet/{Kind}/Toy/{Name}/Price", "Ranking", "CheapMe", "me");
            Assert.IsNotNull(lookup);
        }

        [TestMethod]
        public void LookupResource()
        {
            var result = Database.AddJson("/Pet/Dog/Toy/Bone/Price", "/Pet/{Kind}/Toy/{Name}/Price", "Ranking", @"{""Ranking"":""CheapMe""}", "me");
            Assert.IsTrue(result);

            result = Database.AddJson("/Pet/Dog/Toy/Bone/Price", "/Pet/{Kind}/Toy/{Name}/Price", "Ranking", @"{""Ranking"":""ExpensiveMe""}", "me");
            Assert.IsTrue(result);

            // /Pet
            var resource = Database.LookupResource("/Pet", "/Pet", "me");
            Assert.AreEqual("Pet", resource.Name);
            Assert.IsFalse(resource.IsPropertyResource);

            // /Pet/Dog - /Pet/{Kind}
            resource = Database.LookupResource("/Pet/Dog", "/Pet/{Kind}", "me");
            Assert.AreEqual("Dog", resource.Name);
            Assert.IsTrue(resource.IsPropertyResource);

            resource = Database.LookupResource("/Pet/Cat", "/Pet/{Kind}", "me");
            Assert.IsNull(resource);

            // "/Pet/Dog/Toy/Bone/Price", "/Pet/{Kind}/Toy/{Name}/Price"
            resource = Database.LookupResource("/Pet/Dog/Toy/Bone/Price", "/Pet/{Kind}/Toy/{Name}/Price", "me");
            Assert.AreEqual("Price", resource.Name);
            Assert.AreEqual(2, resource.Resources.Count);
            Assert.IsFalse(resource.IsPropertyResource);

            resource = Database.LookupResource("/Pet/Dog/Toy/Bone/Price/ExpensiveMe", "/Pet/{Kind}/Toy/{Name}/Price/{Ranking}", "me");
            Assert.AreEqual("ExpensiveMe", resource.Name);
            Assert.IsTrue(resource.IsPropertyResource);

            resource = Database.LookupResource("/Pet/Dog/Toy/Bone/Price/CheapMe", "/Pet/{Kind}/Toy/{Name}/Price/{Ranking}", "me");
            Assert.AreEqual("CheapMe", resource.Name);
            Assert.IsTrue(resource.IsPropertyResource);
        }

        #endregion

        #region Exists

        // These are simple because Exists wraps Lookup
        [TestMethod]
        public void ExistsIsTrueIfNoneExistentProperty()
        {
            var result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""Dog""}");
            var exists = Database.Exists("/Pet", "/Pet", "NoneExistent", null);
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void ExistsIsTrueIfEmptyPropertyExists()
        {
            var result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""TheProperty"":"""",""Kind"":""Dog""}");
            var exists = Database.Exists("/Pet", "/Pet", "TheProperty", "");
            Assert.IsTrue(exists);
        }


        #endregion 

        #region Clear

        [TestMethod]
        public void RemoveAll()
        {
            var result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""TheProperty"":"""",""Kind"":""Dog""}");
            Database.RemoveAll();
            Assert.AreEqual(0, Database.GetResources().Count);
        }

        #endregion

        #region Remove

        [TestMethod]
        public void RemoveObjectThatExists()
        {
            var result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""Dog""}");
            result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""Cat""}");

            var removed = Database.Remove("/Pet", "/Pet", "Kind", "Dog");
            Assert.IsTrue(removed);

            Assert.AreEqual(1, Database.GetResources().Count);
            Assert.AreEqual(1, Database.GetResources()[0].Resources.Count);
            Assert.AreEqual(@"{""Kind"":""Cat""}", Database.GetResources()[0].Resources[0].Data()[0].Json);
        }

        [TestMethod]
        public void RemoveObjectThatDoesNotExists()
        {
            var result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""Dog""}");
            result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""Cat""}");

            var removed = Database.Remove("/Pet", "/Pet", "Kind", "Gerbil");
            Assert.IsFalse(removed);

            Assert.AreEqual(1, Database.GetResources().Count);
            Assert.AreEqual(2, Database.GetResources()[0].Resources.Count);
            Assert.AreEqual(@"{""Kind"":""Dog""}", Database.GetResources()[0].Resources[0].Data()[0].Json);
            Assert.AreEqual(@"{""Kind"":""Cat""}", Database.GetResources()[0].Resources[1].Data()[0].Json);
        }

        [TestMethod]
        public void RemoveObjectWithNullProperty()
        {
            var result = Database.AddJson("/Pet", "/Pet", "Kind", @"{""Kind"":""Dog""}");
            var removed = Database.Remove("/Pet", "/Pet", "NoneExistent", null);
            Assert.IsFalse(removed);
            Assert.AreEqual(1, Database.GetResources().Count);
            Assert.AreEqual(1, Database.GetResources()[0].Resources[0].Data().Count);
        }

        #endregion
    }
}
