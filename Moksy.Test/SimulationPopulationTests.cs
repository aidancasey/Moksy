using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Newtonsoft.Json;
using Moksy.Common;

namespace Moksy.Test
{
    /// <summary>
    /// Tests to ensure that the various fluent methods correctly populate the Simulation class. 
    /// </summary>
    [TestClass]
    public class SimulationPopulationTests
    {
        [TestMethod]
        public void SanityNew()
        {
            var simulation = SimulationFactory.New();
            Assert.IsNotNull(simulation.Name);
            Assert.IsNotNull(simulation.Condition);
            Assert.AreEqual(null, simulation.Condition.SimulationConditionContent.Pattern);
            Assert.AreEqual(null, simulation.Condition.SimulationConditionContent.HttpMethod);
            Assert.AreEqual(0, simulation.Condition.RequestHeaders.Count);
            Assert.AreEqual(null, simulation.Response.SimulationResponseContent.Content);
            Assert.AreEqual(System.Net.HttpStatusCode.Unused, simulation.Response.SimulationResponseContent.HttpStatusCode);
        }

        [TestMethod]
        public void WithHeader()
        {
            var simulation = SimulationFactory.When.Get().From("/Product").With.Header("Content-Type", "application/json");
            Assert.AreEqual(1, simulation.RequestHeaders.Count);
            Assert.AreEqual("Content-Type", simulation.RequestHeaders[0].Name);
            Assert.AreEqual("application/json", simulation.RequestHeaders[0].Value);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void WithHeaderWithNullName()
        {
            var simulation = SimulationFactory.When.Get().From("/Product").With.Header(null, "application/json");
        }

        [TestMethod]
        public void WithHeaderWithNullValue()
        {
            var simulation = SimulationFactory.When.Get().From("/Product").With.Header("Content-Type", null);
            Assert.AreEqual(1, simulation.RequestHeaders.Count);
            Assert.AreEqual("Content-Type", simulation.RequestHeaders[0].Name);
            Assert.AreEqual(null, simulation.RequestHeaders[0].Value);
        }

        [TestMethod]
        public void WithHeaderWithEmptyValue()
        {
            var simulation = SimulationFactory.When.Get().From("/Product").With.Header("Content-Type", "");
            Assert.AreEqual(1, simulation.RequestHeaders.Count);
            Assert.AreEqual("Content-Type", simulation.RequestHeaders[0].Name);
            Assert.AreEqual("", simulation.RequestHeaders[0].Value);
        }

        [TestMethod]
        public void WithHeaderAlreadyPopulated()
        {
            Header header = new Header("MyHeader", "MyValue");
            var simulation = SimulationFactory.When.Get().From("/Product").With.Header(header);
            Assert.AreEqual(1, simulation.RequestHeaders.Count);
            Assert.AreEqual("MyHeader", simulation.RequestHeaders[0].Name);
            Assert.AreEqual("MyValue", simulation.RequestHeaders[0].Value);
        }

        [TestMethod]
        public void WithHeaderPopulatedAsNull()
        {
            var simulation = SimulationFactory.When.Get().From("/Product").With.Header( (string) null);
            Assert.IsNotNull(simulation);
        }

        [TestMethod]
        public void WithHeadersNull()
        {
            var simulation = SimulationFactory.When.Get().From("/Product").With.Headers(null);
            Assert.IsNotNull(simulation);
        }

        [TestMethod]
        public void WithHeadersOne()
        {
            Header header = new Header("MyHeader", "MyValue");
            var headers = new List<Header>() { header };
            var simulation = SimulationFactory.When.Get().From("/Product").With.Headers(headers);
            Assert.IsNotNull(simulation);
            Assert.AreEqual(1, simulation.RequestHeaders.Count);
        }

        [TestMethod]
        public void WithHeadersTwo()
        {
            Header header1 = new Header("MyHeader", "MyValue");
            Header header2 = new Header("MyOtherHeader", "MyOtherValue");
            var headers = new List<Header>() { header1, header2 };
            var simulation = SimulationFactory.When.Get().From("/Product").With.Headers(headers);
            Assert.IsNotNull(simulation);
            Assert.AreEqual(2, simulation.RequestHeaders.Count);
        }



        [TestMethod]
        public void ReturnBodyStringNull()
        {
            var simulation = SimulationFactory.When.Get().From("/Product").Return.Body((string)null);
            Assert.AreEqual(null, simulation.SimulationResponseContent.Content);
        }

        [TestMethod]
        public void ReturnBodyStringEmpty()
        {
            var simulation = SimulationFactory.When.Get().From("/Product").Return.Body("");
            Assert.AreEqual("", simulation.SimulationResponseContent.Content);
        }

        [TestMethod]
        public void ReturnBodyStringSomeText()
        {
            var simulation = SimulationFactory.When.Get().From("/Product").Return.Body("SomeText");
            Assert.AreEqual("SomeText", simulation.SimulationResponseContent.Content);
        }



        [TestMethod]
        public void StatusCodeIsSet()
        {
            var simulation = SimulationFactory.When.Get().From("/Product").Return.Body("SomeText").StatusCode(System.Net.HttpStatusCode.OK);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, simulation.SimulationResponseContent.HttpStatusCode);
        }



        [TestMethod]
        public void Serialization()
        {
            var simulation = SimulationFactory.When.Get().From("/Product").With.Header("TheHeader", "TheHeaderValoue").Then.Return.Body("SomeText").And.StatusCode(System.Net.HttpStatusCode.OK).Simulation;
            var json = JsonConvert.SerializeObject(simulation);
            var hydrated = JsonConvert.DeserializeObject<Simulation>(json);

            Assert.AreEqual(simulation.Name, hydrated.Name);
            Assert.AreEqual(simulation.Response.SimulationResponseContent.Content, hydrated.Response.SimulationResponseContent.Content);
            Assert.AreEqual(simulation.Condition.SimulationConditionContent.HttpMethod, hydrated.Condition.SimulationConditionContent.HttpMethod);
            Assert.AreEqual(simulation.Response.SimulationResponseContent.HttpStatusCode, hydrated.Response.SimulationResponseContent.HttpStatusCode);

            Assert.AreEqual(1, simulation.Condition.RequestHeaders.Count);
            Assert.AreEqual("TheHeader", simulation.Condition.RequestHeaders[0].Name);
            Assert.AreEqual("TheHeaderValoue", simulation.Condition.RequestHeaders[0].Value);
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void MustSpecifyNounFirstTo()
        {
            Simulation s = new Simulation();
            s.To("somewhere");
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void MustSpecifyNounFirstFrom()
        {
            Simulation s = new Simulation();
            s.From("somewhere");
        }

        [TestMethod]
        public void OnGet()
        {
            var s = SimulationFactory.When.Get().From("/ThePath");
            Assert.AreEqual(System.Net.Http.HttpMethod.Get, s.SimulationConditionContent.HttpMethod);
            Assert.AreEqual("/ThePath", s.SimulationConditionContent.Pattern);
        }

        [TestMethod]
        public void OnDelete()
        {
            var s = SimulationFactory.When.Delete().From("/ThePath");
            Assert.AreEqual(System.Net.Http.HttpMethod.Delete, s.SimulationConditionContent.HttpMethod);
            Assert.AreEqual("/ThePath", s.SimulationConditionContent.Pattern);
        }

        [TestMethod]
        public void OnPost()
        {
            var s = SimulationFactory.When.Post().To("/ThePath");
            Assert.AreEqual(System.Net.Http.HttpMethod.Post, s.SimulationConditionContent.HttpMethod);
            Assert.AreEqual("/ThePath", s.SimulationConditionContent.Pattern);
        }

        [TestMethod]
        public void OnTrace()
        {
            var s = SimulationFactory.When.Trace().To("/ThePath");
            Assert.AreEqual(System.Net.Http.HttpMethod.Trace, s.SimulationConditionContent.HttpMethod);
            Assert.AreEqual("/ThePath", s.SimulationConditionContent.Pattern);
        }

        [TestMethod]
        public void OnPut()
        {
            var s = SimulationFactory.When.Put().To("/ThePath");
            Assert.AreEqual(System.Net.Http.HttpMethod.Put, s.SimulationConditionContent.HttpMethod);
            Assert.AreEqual("/ThePath", s.SimulationConditionContent.Pattern);
        }

        [TestMethod]
        public void OnOptions()
        {
            var s = SimulationFactory.When.Options().To("/ThePath");
            Assert.AreEqual(System.Net.Http.HttpMethod.Options, s.SimulationConditionContent.HttpMethod);
            Assert.AreEqual("/ThePath", s.SimulationConditionContent.Pattern);
        }

        [TestMethod]
        public void OnHead()
        {
            var s = SimulationFactory.When.Head().To("/ThePath");
            Assert.AreEqual(System.Net.Http.HttpMethod.Head, s.SimulationConditionContent.HttpMethod);
            Assert.AreEqual("/ThePath", s.SimulationConditionContent.Pattern);
        }



        [TestMethod]
        public void Once()
        {
            var s = SimulationFactory.When.Delete().From("/ThePath").Once();
            Assert.AreEqual(1, s.SimulationConditionContent.Repeat);
        }

        [TestMethod]
        public void Twice()
        {
            var s = SimulationFactory.When.Delete().From("/ThePath").Twice();
            Assert.AreEqual(2, s.SimulationConditionContent.Repeat);
        }

        [TestMethod]
        public void Forever()
        {
            var s = SimulationFactory.When.Delete().From("/ThePath").Forever();
            Assert.AreEqual(Int64.MaxValue, s.SimulationConditionContent.Repeat);
        }

        [TestMethod]
        public void SixTimes()
        {
            var s = SimulationFactory.When.Delete().From("/ThePath").Times(6);
            Assert.AreEqual(6, s.SimulationConditionContent.Repeat);
        }



        [TestMethod]
        public void DefaultContentKindIsText()
        {
            var c = SimulationFactory.When.Post().To("/ThePath");
            Assert.AreEqual(ContentKind.Text, c.SimulationConditionContent.ContentKind);
        }

        [TestMethod]
        public void ContentKindCanBeSetToText()
        {
            var c = SimulationFactory.When.Post().To("/ThePath").AsText();
            Assert.AreEqual(ContentKind.Text, c.SimulationConditionContent.ContentKind);
        }

        [TestMethod]
        public void ContentKindCanBeSetToJson()
        {
            var c = SimulationFactory.When.Post().To("/ThePath").AsJson();
            Assert.AreEqual(ContentKind.Json, c.SimulationConditionContent.ContentKind);
        }


        [TestMethod]
        public void SupportsImdb()
        {
            var s = SimulationFactory.When.Delete().From("/ThePath");
            Assert.IsFalse(s.Simulation.Condition.SimulationConditionContent.IsImdb);

            var c = SimulationFactory.When.I.Delete().FromImdb("/ThePath");
            Assert.IsTrue(c.Simulation.Condition.SimulationConditionContent.IsImdb);
        }



        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void ExistsWillThrowIfNotJson()
        {
            var s = SimulationFactory.When.I.Get().From("/Endpoint").Exists().Return.StatusCode(System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void ExistsWillThrowIfNotJsonImplicitExistsWithoutPlaceholder()
        {
            var s = SimulationFactory.When.I.Get().From("/Endpoint").AsJson().Exists().Return.StatusCode(System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        public void ExistsWillThrowIfNotJsonImplicitExistsWithoutPlaceholderKeyIsIndexProperty()
        {
            var s = SimulationFactory.When.I.Get().From("/Endpoint({id})").AsJson().Exists().Return.StatusCode(System.Net.HttpStatusCode.OK);

            Assert.AreEqual("id", s.Simulation.Condition.SimulationConditionContent.IndexProperty);
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void ExistsWithPropertyWillThrowIfNotJson()
        {
            var s = SimulationFactory.When.I.Get().From("/Endpoint").Exists("{woo}").Return.StatusCode(System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void NotExistsWillThrowIfNotJson()
        {
            var s = SimulationFactory.When.I.Get().From("/Endpoint").NotExists().Return.StatusCode(System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void NotExistsWithPropertyWillThrowIfNotJson()
        {
            var s = SimulationFactory.When.I.Get().From("/Endpoint").NotExists("{woo}").Return.StatusCode(System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void NotExistsWillThrowIfNotJsonImplicitExistsWithoutPlaceholder()
        {
            var s = SimulationFactory.When.I.Get().From("/Endpoint").AsJson().NotExists().Return.StatusCode(System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        public void NotExistsWillThrowIfNotJsonImplicitExistsWithoutPlaceholderKeyIsIndexProperty()
        {
            var s = SimulationFactory.When.I.Get().From("/Endpoint({id})").AsJson().NotExists().Return.StatusCode(System.Net.HttpStatusCode.OK);

            Assert.AreEqual("id", s.Simulation.Condition.SimulationConditionContent.IndexProperty);
        }



        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void ExistsWillThrowIfNotJsonAsTest()
        {
            var s = SimulationFactory.When.I.Get().From("/Endpoint").AsText().Exists().Return.StatusCode(System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void ExistsWithPropertyWillThrowIfNotJsonAsTest()
        {
            var s = SimulationFactory.When.I.Get().From("/Endpoint").AsText().Exists("{woo}").Return.StatusCode(System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void NotExistsWillThrowIfNotJsonAsText()
        {
            var s = SimulationFactory.When.I.Get().From("/Endpoint").AsText().NotExists().Return.StatusCode(System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void NotExistsWithPropertyWillThrowIfNotJsonAsText()
        {
            var s = SimulationFactory.When.I.Get().From("/Endpoint").AsText().NotExists("{woo}").Return.StatusCode(System.Net.HttpStatusCode.OK);
        }



        [TestMethod]
        public void FromImdbImpliesJson()
        {
            var s = SimulationFactory.When.I.Get().FromImdb("/Endpoint").And.NotExists("woo").Return.StatusCode(System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        public void ToImdbImpliesJson()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Endpoint").And.NotExists("woo").Return.StatusCode(System.Net.HttpStatusCode.OK);
        }




        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void FromImdbImpliesJsonAndCannotBeSetToText()
        {
            var s = SimulationFactory.When.I.Get().FromImdb("/Endpoint").AsText().Return.StatusCode(System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void ToImdbImpliesJsonAndCannotBeSetToText()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Endpoint").AsText().Return.StatusCode(System.Net.HttpStatusCode.OK);
        }



        [TestMethod]
        public void NullBodyIsOk()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Endpoint").AsJson().Then.Return.Body((string)null);
            Assert.AreEqual(null, s.SimulationResponseContent.Content);
        }



        public class Pet
        {
            public string Kind { get; set; }
        }

        [TestMethod]
        public void ObjectAsJsonWorks()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Endpoint").AsJson().Then.Return.Body(new Pet() { Kind = "Dog" });
            Assert.AreEqual(@"{""Kind"":""Dog""}", s.SimulationResponseContent.Content);
        }



        [TestMethod]
        public void EmptyVariables()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/").Then.Return;
            Assert.AreEqual(0, s.Variables.Count);
        }

        [TestMethod]
        public void AddVariableGuid()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/").Then.Return.With.Variable("TheVar");
            Assert.AreEqual(1, s.Variables.Count);
            Assert.AreEqual("TheVar", s.Variables[0].Name);
            Assert.AreEqual(VariableKind.Guid, s.Variables[0].Kind);
        }

        [TestMethod]
        public void AddVariableConstant()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/").Then.Return.With.Variable("TheVar", "TheValue");
            Assert.AreEqual(1, s.Variables.Count);
            Assert.AreEqual("TheVar", s.Variables[0].Name);
            Assert.AreEqual("TheValue", s.Variables[0].Value);
            Assert.AreEqual(VariableKind.Constant, s.Variables[0].Kind);
        }

        [TestMethod]
        public void CalculateVariablesEmpty()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/").Then.Return;
            var result = s.CalculateVariables();
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void CalculateVariablesGuid()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/").Then.Return.With.Variable("TheVar");
            var result = s.CalculateVariables();
            Assert.AreEqual(1, result.Count);
            Guid guid = default(Guid);
            var valid = System.Guid.TryParse(result["TheVar"], out guid);
            Assert.IsTrue(valid);

            // Now calculate the variables again - a new Guid should be created. 
            var result2 = s.CalculateVariables();
            Assert.AreNotEqual(result["TheVar"], result2["TheVar"]);
        }

        [TestMethod]
        public void CalculateVariablesConstant()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/").Then.Return.With.Variable("TheVar", "TheValue");
            var result = s.CalculateVariables();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("TheValue", result["TheVar"]);
        }



        [TestMethod]
        public void EmptyProperties()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/").Then.Return;
            Assert.AreEqual(0, s.Properties.Count);
        }

        [TestMethod]
        public void OneProperty()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/").Then.Return.OverrideProperty("A", "B");
            Assert.AreEqual(1, s.Properties.Count);
            Assert.AreEqual("A", s.Properties[0].Name);
            Assert.AreEqual("B", s.Properties[0].Value);
        }



        [TestMethod]
        public void ImdbDiscriminatorTo()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet");
            Assert.IsFalse(s.IsGroupedByImdbHeaderDiscriminator);
            Assert.AreEqual(null, s.SimulationConditionContent.ImdbHeaderDiscriminator);
        }

        [TestMethod]
        public void ImdbDiscriminatorToSet()
        {
            var s = SimulationFactory.When.I.Post().ToImdb("/Pet", "clinic");
            Assert.IsTrue(s.IsGroupedByImdbHeaderDiscriminator);
            Assert.AreEqual("clinic", s.SimulationConditionContent.ImdbHeaderDiscriminator);
        }

        [TestMethod]
        public void ImdbDiscriminatorFrom()
        {
            var s = SimulationFactory.When.I.Get().FromImdb("/Pet");
            Assert.IsFalse(s.IsGroupedByImdbHeaderDiscriminator);
            Assert.AreEqual(null, s.SimulationConditionContent.ImdbHeaderDiscriminator);
        }

        [TestMethod]
        public void ImdbDiscriminatorFromSet()
        {
            var s = SimulationFactory.When.I.Get().FromImdb("/Pet", "clinic");
            Assert.IsTrue(s.IsGroupedByImdbHeaderDiscriminator);
            Assert.AreEqual("clinic", s.SimulationConditionContent.ImdbHeaderDiscriminator);
        }



        [TestMethod]
        public void NoParameters()
        {
            var s = SimulationFactory.When.I.Get().From("/Pet").Return.StatusCode(System.Net.HttpStatusCode.OK);
            Assert.AreEqual(0, s.Simulation.Condition.SimulationConditionContent.ParametersForJson.Length);
        }

        [TestMethod]
        public void OneParameter()
        {
            var s = SimulationFactory.When.I.Get().From("/Pet").Parameter("theparam", "thevalue").Return.StatusCode(System.Net.HttpStatusCode.OK);
            Assert.AreEqual(1, s.Simulation.Condition.SimulationConditionContent.ParametersForJson.Length);
            var t = s.Simulation.Condition.SimulationConditionContent.ParametersForJson[0];
            Assert.AreEqual("theparam", t.Name);
            Assert.AreEqual("thevalue", t.Value);
        }



        [TestMethod]
        public void NoContent()
        {
            var s = SimulationFactory.When.I.Get().From("/Pet").Return.StatusCode(System.Net.HttpStatusCode.OK);
            Assert.AreEqual(0, s.Simulation.Condition.ContentRules.Count);
        }

        [TestMethod]
        public void OneContent()
        {
            var s = SimulationFactory.When.I.Get().From("/Pet").Contains("something").Return.StatusCode(System.Net.HttpStatusCode.OK);
            Assert.AreEqual(1, s.Simulation.Condition.ContentRules.Count);
            Assert.AreEqual("something", s.Simulation.Condition.ContentRules[0].Content);
        }
    }
}
