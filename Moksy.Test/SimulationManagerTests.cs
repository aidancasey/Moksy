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
            s.Condition.SimulationConditionContent.Pattern = "/Pet";
            Assert.IsTrue(manager.CanAdd(s, "/Pet", "Kind", "Dog", null));
        }

        // Obsoleted: Imdb must now be Json. Always. 
        // [TestMethod]
        public void CanAddIfEndpointContainsOneEntryThatIsNotJson()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            manager.AddToImdb(s, "/Pet", "/Pet", "NotValidJson", null);
            Assert.IsTrue(manager.CanAdd(s, "/Pet", "Something", "Else", null));
        }

        [TestMethod]
        public void CanAddIfEndpointContainsOneEntryThatIsJsonButEmpty()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            manager.AddToImdb(s, "/Pet", "/Pet", "{ }", null);
            Assert.IsTrue(manager.CanAdd(s, "/Pet", "Kind", "Dog", null));
        }

        [TestMethod]
        public void CanAddIfEndpointContainsOneEntryThatIsJsonAndDoesNotMatchEmpty()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            manager.AddToImdb(s, "/Pet", "/Pet", "{ }", null);
            Assert.IsTrue(manager.CanAdd(s, "/Pet", "Kind", "Dog", null));
        }

        [TestMethod]
        public void CanAddIfEndpointContainsOneEntryThatIsJsonAndDOesNotMatchValue()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            manager.AddToImdb(s, "/Pet", "/Pet", @"{ ""Kind"" : ""Cat"" }", null);
            Assert.IsTrue(manager.CanAdd(s, "/Pet", "Kind", "Dog", null));
        }

        [TestMethod]
        public void CanNotAddIfEndpointContainsOneEntryThatIsJsonAndDoesNotMatchEmpty()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            manager.AddToImdb(s, "/Pet", "/Pet", @"{ ""Kind"" : ""Dog"" }", null);
            Assert.IsFalse(manager.CanAdd(s, "/Pet", "Kind", "Dog", null));
        }


        // Obsoleted: Imdb must now be Json. Always. 
        // [TestMethod]
        public void CanNotAddButAfterEmptyMismatchAndValueMismatch()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            manager.AddToImdb(s, "/Pet", "/Pet", @"{ }", null);
            manager.AddToImdb(s, "/Pet", "/Pet", @"NotJson", null);
            manager.AddToImdb(s, "/Pet", "/Pet", @"{ ""Kind"" : ""Cat"" }", null);
            manager.AddToImdb(s, "/Pet", "/Pet", @"{ ""Kind"" : ""Dog"" }", null);
            Assert.IsFalse(manager.CanAdd(s, "/Pet", "Kind", "Dog", null));
        }



        [TestMethod]
        public void CanAddObjectWithPropertyNoneExistent()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            Assert.IsFalse(manager.CanAddObject(s, "/Pet", "TheProperty", "thisisnotjson", null));
        }

        [TestMethod]
        public void CanAddObjectIfValidJsonAndPropertyIsNull()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            Assert.IsTrue(manager.CanAddObject(s, "/Pet", "TheProperty", "{ }", null));
        }

        [TestMethod]
        public void CanAddObjectIfValidJsonAndPropertyIsExplicitlyNull()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            Assert.IsTrue(manager.CanAddObject(s, "/Pet", "TheProperty", @"{ ""TheProperty"" : null }", null));
        }

        [TestMethod]
        public void CanAddObjectIfValidJsonAndPropertyIsSet()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            Assert.IsTrue(manager.CanAddObject(s, "/Pet", "TheProperty", @"{ ""TheProperty"" : ""TheValue"" }", null));
        }

        [TestMethod]
        public void CanNotAddObjectIfPropertyIsImplicitlyAlreadyNull()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            manager.AddToImdb(s, "/Pet", "/Pet", @"{ }", null);
            Assert.IsFalse(manager.CanAddObject(s, "/Pet", "TheProperty", "{ }", null));
        }

        [TestMethod]
        public void CanAddObjectIfPropertyNotExists()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            manager.AddToImdb(s, "/Pet", "/Pet", @"{ }", null);
            Assert.IsTrue(manager.CanAddObject(s, "/Pet", "TheProperty", @"{ ""TheProperty"" : ""TheValue"" }", null));
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
            var result = manager.GetKeysFor(null, null, null, null);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetKeysNoEntriesPathNotAdded()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var result = manager.GetKeysFor("/NoneExistent", "/NoneExistent", "Kind", null);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetsNoKeysForNullObject()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            manager.AddToImdb(s, "/Pet", "/Pet", "{ }", null);
            var result = manager.GetKeysFor("/Pet", "/Pet", "Kind", null);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetOneKeysForObject()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            manager.AddToImdb(s, "/Pet", "/Pet", @"{ ""Kind"" : ""Dog"" }", null);
            var result = manager.GetKeysFor("/Pet", "/Pet", "Kind", null);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Dog", result.ToArray()[0]);
        }

        [TestMethod]
        public void GetTwoKeysForObject()
        {
            Moksy.Storage.SimulationManager manager = new Storage.SimulationManager();
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet").Simulation;
            s.Condition.SimulationConditionContent.IndexProperty = "Kind";
            manager.AddToImdb(s, "/Pet", "/Pet", @"{ ""Kind"" : ""Dog"" }", null);
            manager.AddToImdb(s, "/Pet", "/Pet", @"{ ""Kind"" : ""Cat"" }", null);
            var result = manager.GetKeysFor("/Pet", "/Pet", "Kind", null);
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

            var match = mgr.Match(System.Net.Http.HttpMethod.Delete, "/ThePath", null, null, false, null);
            Assert.IsNotNull(match);
            Assert.AreEqual(1, match.Condition.SimulationConditionContent.Repeat);

            var all = mgr.Get();
            Assert.AreEqual(1, all.Count);
            Assert.AreEqual(1, all[0].Condition.SimulationConditionContent.Repeat);
        }

        [TestMethod]
        public void StorageDoesDecrementAndRemoves()
        {
            SimulationManager mgr = new SimulationManager();
            var s = SimulationFactory.When.Delete().From("/ThePath").Once().Simulation;
            mgr.Add(s);

            var match = mgr.Match(System.Net.Http.HttpMethod.Delete, "/ThePath", null, null, true, null);
            Assert.IsNotNull(match);
            Assert.AreEqual(0, match.Condition.SimulationConditionContent.Repeat);

            var all = mgr.Get();
            Assert.AreEqual(0, all.Count);
        }

        [TestMethod]
        public void StorageDoesDecrementButDoesNotRemove()
        {
            SimulationManager mgr = new SimulationManager();
            var s = SimulationFactory.When.Delete().From("/ThePath").Times(4).Simulation;
            mgr.Add(s);

            var match = mgr.Match(System.Net.Http.HttpMethod.Delete, "/ThePath", null, null, true, null);
            Assert.IsNotNull(match);
            Assert.AreEqual(3, match.Condition.SimulationConditionContent.Repeat);

            var all = mgr.Get();
            Assert.AreEqual(1, all.Count);
        }



        [TestMethod]
        public void PathMatchesNoEntries()
        {
            SimulationManager mgr = new SimulationManager();
            var post = SimulationFactory.When.I.Post().ToImdb("/Pet").AsJson().And.NotExists("Kind").Then.AddToImdb();
            mgr.Add(post.Simulation);

            var add = SimulationFactory.When.I.Get().FromImdb("/Pet('{Kind}')").AsJson().And.NotExists("{Kind}").Then.Return.StatusCode(System.Net.HttpStatusCode.OK);
            mgr.Add(add.Simulation);

            var match = mgr.Match(HttpMethod.Get, "/Pet('Dog')", null, new List<Header>(), false, null);
            Assert.IsNotNull(match);
            Assert.AreEqual("/Pet('{Kind}')", match.Condition.SimulationConditionContent.Pattern);
        }

        [TestMethod]
        public void PathDoesMatchWithIdentitySpecified()
        {
            SimulationManager mgr = new SimulationManager();
            var post = SimulationFactory.When.I.Post().ToImdb("/Pet").AsJson().And.NotExists("Kind").Then.AddToImdb();
            mgr.Add(post.Simulation);

            var add = SimulationFactory.When.I.Get().FromImdb("/Pet('{Kind}')").AsJson().And.NotExists("{Kind}").Then.Return.StatusCode(System.Net.HttpStatusCode.OK);
            mgr.Add(add.Simulation);

            var match = mgr.Match(HttpMethod.Get, "/Pet('Dog')", "", new List<Header>(), false, null);
            Assert.IsNotNull(match);
            Assert.AreEqual("/Pet('{Kind}')", match.Condition.SimulationConditionContent.Pattern);
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

            var match = mgr.Match(HttpMethod.Get, "/Pet", "", new List<Header>(), false, null);
            Assert.IsNotNull(match);
            Assert.AreEqual("/Pet", match.Condition.SimulationConditionContent.Pattern);
        }



        [TestMethod]
        public void GetFromImdbWhenNoPath()
        {
            SimulationManager mgr = new SimulationManager();
            //var get = SimulationFactory.When.I.Get().From("/Pet('{Kind}')").AsJson().With.Imdb().Then.Return.StatusCode(System.Net.HttpStatusCode.OK);
            //mgr.Add(get.Simulation);

            var match = mgr.GetFromImdb(HttpMethod.Get, "/Pet('Dog')", "", new List<Header>(), null);
            Assert.IsNull(match);
        }

        [TestMethod]
        public void GetFromImdbWhenPathButDoesNotMatchPattern()
        {
            SimulationManager mgr = new SimulationManager();
            var get = SimulationFactory.When.I.Get().FromImdb("/Pet('{Kind}')").AsJson().Then.Return.StatusCode(System.Net.HttpStatusCode.OK);
            mgr.Add(get.Simulation);

            var match = mgr.GetFromImdb(HttpMethod.Get, "/Pet(Dog)", "", new List<Header>(), null);
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

            mgr.AddToImdb(post.Simulation, "/Pet", "/Pet", @"{ ""Kind"" : ""Cat"" }", null);

            var match = mgr.GetFromImdb(HttpMethod.Get, "/Pet('Dog')", "", new List<Header>(), null);
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

            mgr.AddToImdb(post.Simulation, "/Pet", "/Pet", @"{ ""Kind"" : ""Dog"" }", null);

            var match = mgr.GetFromImdb(HttpMethod.Get, "/Pet('Dog')", "", new List<Header>(), null);
            Assert.IsNotNull(match);
        }

    }
}
