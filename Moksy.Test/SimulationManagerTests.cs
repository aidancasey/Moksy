using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Common;
using Moksy.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Moksy.Test
{
    [TestClass]
    public class SimulationManagerTests
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public SimulationManagerTests()
        {
        }

        [TestMethod]
        public void CanAddIfEndpointDoesNotExist()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            Simulation s = new Simulation();
            s.Condition.Pattern = "/Pet";
            Assert.IsTrue(manager.CanAdd(s, "/Pet", "Kind", "Dog"));
        }

        // Obsoleted: Imdb must now be Json. Always. 
        // [TestMethod]
        public void CanAddIfEndpointContainsOneEntryThatIsNotJson()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            manager.AddToImdb(s, "/Pet", "NotValidJson");
            Assert.IsTrue(manager.CanAdd(s, "/Pet", "Something", "Else"));
        }

        [TestMethod]
        public void CanAddIfEndpointContainsOneEntryThatIsJsonButEmpty()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            manager.AddToImdb(s, "/Pet", "{ }");
            Assert.IsTrue(manager.CanAdd(s, "/Pet", "Kind", "Dog"));
        }

        [TestMethod]
        public void CanAddIfEndpointContainsOneEntryThatIsJsonAndDoesNotMatchEmpty()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            manager.AddToImdb(s, "/Pet", "{ }");
            Assert.IsTrue(manager.CanAdd(s, "/Pet", "Kind", "Dog"));
        }

        [TestMethod]
        public void CanAddIfEndpointContainsOneEntryThatIsJsonAndDOesNotMatchValue()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            manager.AddToImdb(s, "/Pet", @"{ ""Kind"" : ""Cat"" }");
            Assert.IsTrue(manager.CanAdd(s, "/Pet", "Kind", "Dog"));
        }

        [TestMethod]
        public void CanNotAddIfEndpointContainsOneEntryThatIsJsonAndDoesNotMatchEmpty()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            manager.AddToImdb(s, "/Pet", @"{ ""Kind"" : ""Dog"" }");
            Assert.IsFalse(manager.CanAdd(s, "/Pet", "Kind", "Dog"));
        }


        // Obsoleted: Imdb must now be Json. Always. 
        // [TestMethod]
        public void CanNotAddButAfterEmptyMismatchAndValueMismatch()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            manager.AddToImdb(s, "/Pet", @"{ }");
            manager.AddToImdb(s, "/Pet", @"NotJson");
            manager.AddToImdb(s, "/Pet", @"{ ""Kind"" : ""Cat"" }");
            manager.AddToImdb(s, "/Pet", @"{ ""Kind"" : ""Dog"" }");
            Assert.IsFalse(manager.CanAdd(s, "/Pet", "Kind", "Dog"));
        }



        [TestMethod]
        public void CanAddObjectWithPropertyNoneExistent()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            Assert.IsFalse(manager.CanAddObject(s, "/Pet", "TheProperty", "thisisnotjson"));
        }

        [TestMethod]
        public void CanAddObjectIfValidJsonAndPropertyIsNull()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            Assert.IsTrue(manager.CanAddObject(s, "/Pet", "TheProperty", "{ }"));
        }

        [TestMethod]
        public void CanAddObjectIfValidJsonAndPropertyIsExplicitlyNull()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            Assert.IsTrue(manager.CanAddObject(s, "/Pet", "TheProperty", @"{ ""TheProperty"" : null }"));
        }

        [TestMethod]
        public void CanAddObjectIfValidJsonAndPropertyIsSet()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            Assert.IsTrue(manager.CanAddObject(s, "/Pet", "TheProperty", @"{ ""TheProperty"" : ""TheValue"" }"));
        }

        [TestMethod]
        public void CanNotAddObjectIfPropertyIsImplicitlyAlreadyNull()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            manager.AddToImdb(s, "/Pet", @"{ }");
            Assert.IsFalse(manager.CanAddObject(s, "/Pet", "TheProperty", "{ }"));
        }

        [TestMethod]
        public void CanAddObjectIfPropertyNotExists()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            manager.AddToImdb(s, "/Pet", @"{ }");
            Assert.IsTrue(manager.CanAddObject(s, "/Pet", "TheProperty", @"{ ""TheProperty"" : ""TheValue"" }"));
        }




        [TestMethod]
        public void GetValueFromJsonIsNullIfNotJson()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var result = manager.GetPropertyValueFromJson(null, null);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetValueFromJsonIsNullIfNotJsonButPropertyNull()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var result = manager.GetPropertyValueFromJson("sdfsdf", null);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetValueFromJsonIsNullIfJsonButPropertyNull()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var result = manager.GetPropertyValueFromJson("{ }", "sdfs");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetValueFromJsonIfPropertIsNull()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var result = manager.GetPropertyValueFromJson(@"{ ""TheProperty"" : null }", "TheProperty");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetValueFromJsonIfPropertIsEmpty()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var result = manager.GetPropertyValueFromJson(@"{ ""TheProperty"" : """" }", "TheProperty");
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void GetValueFromJsonIfPropertIsSet()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var result = manager.GetPropertyValueFromJson(@"{ ""TheProperty"" : ""TheValue"" }", "TheProperty");
            Assert.AreEqual("TheValue", result);
        }



        [TestMethod]
        public void GetKeysNoEntriesIsNull()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var result = manager.GetKeysFor(null, null, null);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetKeysNoEntriesPathNotAdded()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var result = manager.GetKeysFor("/NoneExistent", "/NoneExistent", "Kind");
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetsNoKeysForNullObject()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            manager.AddToImdb(s, "/Pet", "{ }");
            var result = manager.GetKeysFor("/Pet", "/Pet", "Kind");
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetOneKeysForObject()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            manager.AddToImdb(s, "/Pet", @"{ ""Kind"" : ""Dog"" }");
            var result = manager.GetKeysFor("/Pet", "/Pet", "Kind");
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Dog", result.ToArray()[0]);
        }

        [TestMethod]
        public void GetTwoKeysForObject()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            manager.AddToImdb(s, "/Pet", @"{ ""Kind"" : ""Dog"" }");
            manager.AddToImdb(s, "/Pet", @"{ ""Kind"" : ""Cat"" }");
            var result = manager.GetKeysFor("/Pet", "/Pet", "Kind");
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Dog", result.ToArray()[0]);
            Assert.AreEqual("Cat", result.ToArray()[1]);
        }

        [TestMethod]
        public void StorageDoesNotDecrement()
        {
            SimulationManager mgr = new SimulationManager();
            var s = SimulationFactory.When.Delete().From("/ThePath").Once().Simulation;
            mgr.Add(s);

            var match = mgr.Match(System.Net.Http.HttpMethod.Delete, "/ThePath", null, false);
            Assert.IsNotNull(match);
            Assert.AreEqual(1, match.Condition.Repeat);

            var all = mgr.Get();
            Assert.AreEqual(1, all.Count);
            Assert.AreEqual(1, all[0].Condition.Repeat);
        }

        [TestMethod]
        public void StorageDoesDecrementAndRemoves()
        {
            SimulationManager mgr = new SimulationManager();
            var s = SimulationFactory.When.Delete().From("/ThePath").Once().Simulation;
            mgr.Add(s);

            var match = mgr.Match(System.Net.Http.HttpMethod.Delete, "/ThePath", null, true);
            Assert.IsNotNull(match);
            Assert.AreEqual(0, match.Condition.Repeat);

            var all = mgr.Get();
            Assert.AreEqual(0, all.Count);
        }

        [TestMethod]
        public void StorageDoesDecrementButDoesNotRemove()
        {
            SimulationManager mgr = new SimulationManager();
            var s = SimulationFactory.When.Delete().From("/ThePath").Times(4).Simulation;
            mgr.Add(s);

            var match = mgr.Match(System.Net.Http.HttpMethod.Delete, "/ThePath", null, true);
            Assert.IsNotNull(match);
            Assert.AreEqual(3, match.Condition.Repeat);

            var all = mgr.Get();
            Assert.AreEqual(1, all.Count);
        }



        [TestMethod]
        public void PathMatchesNoEntries()
        {
            SimulationManager mgr = new SimulationManager();
            var post = SimulationFactory.When.I.Post().ToImdb("/Pet").AsJson().And.NotExists("Kind").Then.AddToImdb();
            mgr.Add(post.Simulation);

            var add = SimulationFactory.When.I.Get().FromImdb("/Pet('{Kind}')").AsJson().Then.Return.StatusCode(System.Net.HttpStatusCode.OK);
            mgr.Add(add.Simulation);

            var match = mgr.Match(HttpMethod.Get, "/Pet('Dog')", new List<Header>(), false);
            Assert.IsNotNull(match);
            Assert.AreEqual("/Pet('{Kind}')", match.Condition.Pattern);
        }

        [TestMethod]
        public void PathDoesMatchWithIdentitySpecified()
        {
            SimulationManager mgr = new SimulationManager();
            var post = SimulationFactory.When.I.Post().ToImdb("/Pet").AsJson().And.NotExists("Kind").Then.AddToImdb();
            mgr.Add(post.Simulation);

            var add = SimulationFactory.When.I.Get().FromImdb("/Pet('{Kind}')").AsJson().Then.Return.StatusCode(System.Net.HttpStatusCode.OK);
            mgr.Add(add.Simulation);

            var match = mgr.Match(HttpMethod.Get, "/Pet('Dog')", new List<Header>(), false);
            Assert.IsNotNull(match);
            Assert.AreEqual("/Pet('{Kind}')", match.Condition.Pattern);
        }

        [TestMethod]
        public void PathDoesMatchWithoutIdentityButWithImdb()
        {
            SimulationManager mgr = new SimulationManager();
            var post = SimulationFactory.When.I.Post().ToImdb("/Pet").AsJson().And.NotExists("Kind").Then.AddToImdb();
            mgr.Add(post.Simulation);

            var get = SimulationFactory.When.I.Get().FromImdb("/Pet('{Kind}')").AsJson().Then.Return.StatusCode(System.Net.HttpStatusCode.OK);
            mgr.Add(get.Simulation);

            var get2 = SimulationFactory.When.I.Get().FromImdb("/Pet").AsJson().Then.Return.StatusCode(System.Net.HttpStatusCode.OK);
            mgr.Add(get2.Simulation);

            var match = mgr.Match(HttpMethod.Get, "/Pet", new List<Header>(), false);
            Assert.IsNotNull(match);
            Assert.AreEqual("/Pet", match.Condition.Pattern);
        }



        [TestMethod]
        public void GetFromImdbWhenNoPath()
        {
            SimulationManager mgr = new SimulationManager();
            //var get = SimulationFactory.When.I.Get().From("/Pet('{Kind}')").AsJson().With.Imdb().Then.Return.StatusCode(System.Net.HttpStatusCode.OK);
            //mgr.Add(get.Simulation);

            var match = mgr.GetFromImdb(HttpMethod.Get, "/Pet('Dog')", new List<Header>());
            Assert.IsNull(match);
        }

        [TestMethod]
        public void GetFromImdbWhenPathButDoesNotMatchPattern()
        {
            SimulationManager mgr = new SimulationManager();
            var get = SimulationFactory.When.I.Get().FromImdb("/Pet('{Kind}')").AsJson().Then.Return.StatusCode(System.Net.HttpStatusCode.OK);
            mgr.Add(get.Simulation);

            var match = mgr.GetFromImdb(HttpMethod.Get, "/Pet(Dog)", new List<Header>());
            Assert.IsNull(match);
        }


        [TestMethod]
        public void GetFromImdbWhenPathButEntryDoesNotExistImplicit()
        {
            SimulationManager mgr = new SimulationManager();
            var post = SimulationFactory.When.I.Post().ToImdb("/Pet").AsJson().And.NotExists("Kind").Then.AddToImdb();
            mgr.Add(post.Simulation);

            var get = SimulationFactory.When.I.Get().FromImdb("/Pet('{Kind}')").AsJson().Then.Return.StatusCode(System.Net.HttpStatusCode.OK);
            mgr.Add(get.Simulation);

            mgr.AddToImdb(post.Simulation, "/Pet", @"{ ""Kind"" : ""Cat"" }");

            var match = mgr.GetFromImdb(HttpMethod.Get, "/Pet('Dog')", new List<Header>());
            Assert.IsNull(match);
        }

        [TestMethod]
        public void GetFromImdbWhenPathAndEntryExists()
        {
            SimulationManager mgr = new SimulationManager();
            var post = SimulationFactory.When.I.Post().ToImdb("/Pet").AsJson().And.NotExists("Kind").Then.AddToImdb();
            mgr.Add(post.Simulation);

            var get = SimulationFactory.When.I.Get().FromImdb("/Pet('{Kind}')").AsJson().And.Exists("Kind").Then.Return.StatusCode(System.Net.HttpStatusCode.OK);
            mgr.Add(get.Simulation);

            mgr.AddToImdb(post.Simulation, "/Pet", @"{ ""Kind"" : ""Dog"" }");

            var match = mgr.GetFromImdb(HttpMethod.Get, "/Pet('Dog')", new List<Header>());
            Assert.IsNotNull(match);
        }
    }
}
