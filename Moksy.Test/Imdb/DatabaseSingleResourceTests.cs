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
    /// Tests the Imdb works for the most simple kind of resource storage. ie: /Pet/{id}
    /// </summary>
    [TestClass]
    public class DatabaseSingleResourceTests
    {
        public DatabaseSingleResourceTests()
        {
        }

        [TestInitialize]
        public void Init()
        {
            Database = new Database();
        }

        internal Database Database;

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
            Assert.AreEqual(1, Database.GetResources()[0].Resources.Count);

            var dogResource = Database.GetResources()[0].Resources[0];
            Assert.AreEqual("Dog", dogResource.Name);

            Assert.AreEqual(1, dogResource.Data().Count);
            Assert.AreEqual(@"{""Kind"":""Dog""}", dogResource.Data()[0].Json);
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
            Assert.AreEqual(1, Database.GetResources()[0].Resources[0].Data().Count);
            Assert.AreEqual(1, Database.GetResources()[0].Resources[1].Data().Count);
            Assert.AreEqual(@"{""Kind"":""Dog""}", Database.GetResources()[0].Resources[0].Data()[0].Json);
            Assert.AreEqual(@"{""Kind"":""Cat""}", Database.GetResources()[0].Resources[1].Data()[0].Json);
        }

        [TestMethod]
        public void AddMissingPropertyEntry()
        {
            var result = Database.AddJson("/Pet", "/Pet", "NoneExistentProperty", @"{""Kind"":""Dog""}");
            Assert.AreEqual(1, Database.GetResources().Count);
            Assert.AreEqual(1, Database.GetResources()[0].Resources[0].Data().Count);
            Assert.AreEqual(@"{""Kind"":""Dog""}", Database.GetResources()[0].Resources[0].Data()[0].Json);
        }

        [TestMethod]
        public void AddMissingPropertyEntryTwice()
        {
            var result = Database.AddJson("/Pet", "/Pet", "NoneExistentProperty", @"{""Kind"":""Dog""}");
            result = Database.AddJson("/Pet", "/Pet", "NoneExistentProperty", @"{""Kind"":""Cat""}");
            Assert.AreEqual(1, Database.GetResources().Count);
            Assert.AreEqual(1, Database.GetResources()[0].Resources[0].Data().Count);
            Assert.AreEqual(@"{""Kind"":""Cat""}", Database.GetResources()[0].Resources[0].Data()[0].Json);
        }

        [TestMethod]
        public void AddMissingPropertyEntryAndEmptyProperty()
        {
            var result = Database.AddJson("/Pet", "/Pet", "NoneExistentProperty", @"{""Kind"":""Dog""}");
            result = Database.AddJson("/Pet", "/Pet", "NoneExistentProperty", @"{""NoneExistentProperty"":"""",""Kind"":""Cat""}");
            Assert.AreEqual(1, Database.GetResources().Count);
            Assert.AreEqual(2, Database.GetResources()[0].Resources.Count);
            Assert.AreEqual(1, Database.GetResources()[0].Resources[0].Data().Count);
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
            Assert.AreEqual(1, Database.GetResources()[0].Resources[0].Data().Count);
            Assert.AreEqual(1, Database.GetResources()[0].Resources[1].Data().Count);
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
            Assert.AreEqual(0, Database.Resources.Count);
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
