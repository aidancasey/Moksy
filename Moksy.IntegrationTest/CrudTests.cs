using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Storage;
using Newtonsoft.Json;
using Moksy.Common;

namespace Moksy.IntegrationTest
{
    [TestClass]
    public class SanityCrudTests : TestBase
    {
        [TestInitialize]
        public void Initialize()
        {
            Proxy = new Proxy(10011);
            Proxy.DeleteAll();

            var all = Proxy.GetAll();
            Assert.AreEqual(0, all.Count);
        }

        Proxy Proxy = new Proxy(10011);


        [TestMethod]
        public void GetAllIsEmpty()
        {
            var all = Proxy.GetAll();
            Assert.AreEqual(0, all.Count);
        }

        [TestMethod]
        public void GetAllByNameNullNotExists()
        {
            var response = Proxy.GetByName(null);
            Assert.IsNull(response);
        }

        [TestMethod]
        public void GetAllByNameEmptyNotExists()
        {
            var response = Proxy.GetByName("");
            Assert.IsNull(response);
        }



        [TestMethod]
        public void CreateWithName()
        {
            Simulation s = new Simulation() { Name = "Trial1" };
            var response = Proxy.Add(s);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response);

            var all = Proxy.GetAll();
            Assert.AreEqual(1, all.Count);
            Assert.AreEqual("Trial1", all[0].Name);
        }

        [TestMethod]
        public void CreateWithNameThatAlreadyExists()
        {
            Simulation s = new Simulation() { Name = "Trial1" };
            var response = Proxy.Add(s);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response);

            s = new Simulation() { Name = "Trial1" };
            response = Proxy.Add(s);
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response);

            var all = Proxy.GetAll();
            Assert.AreEqual(1, all.Count);
            Assert.AreEqual("Trial1", all[0].Name);
        }



        [TestMethod]
        public void DeleteByNameNotExist()
        {
            var response = Proxy.DeleteByName("ASimulationThatDoesNotExist");
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response);

            var all = Proxy.GetAll();
            Assert.AreEqual(0, all.Count);
        }

        [TestMethod]
        public void DeleteByNameNotExistNull()
        {
            var response = Proxy.DeleteByName(null);
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response);

            var all = Proxy.GetAll();
            Assert.AreEqual(0, all.Count);
        }

        [TestMethod]
        public void DeleteByNameThatDoesExist()
        {
            Simulation s = new Simulation() { Name = "Trial1" };
            var response = Proxy.Add(s);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response);

            var all = Proxy.GetAll();
            Assert.AreEqual(1, all.Count);

            response = Proxy.DeleteByName("Trial1");
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response);

            all = Proxy.GetAll();
            Assert.AreEqual(0, all.Count);
        }



        [TestMethod]
        public void DeleteAll()
        {
            Simulation s = new Simulation() { Name = "Trial1" };
            var response = Proxy.Add(s);

            response = Proxy.DeleteByName("*");
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response);

            var all = Proxy.GetAll();
            Assert.AreEqual(0, all.Count);
        }



    }
}
