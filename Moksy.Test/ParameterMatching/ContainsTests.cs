using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Common;
using Moksy.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Test.ParameterMatching
{
    /// <summary>
    /// Tests conditions where the body contains certain content. 
    /// </summary>
    [TestClass]
    public class ContainsTests
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public ContainsTests()
        {
        }

        [TestInitialize]
        public void Initialize()
        {
            Evaluator = new SimulationConditionEvaluator();
        }

        SimulationConditionEvaluator Evaluator;



        [TestMethod]
        public void NoContentRulesAlwaysEvaluatesAsTrue()
        {
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("something", Encoding.UTF8, "application/json");

            Assert.IsTrue(e.MatchesContentRules(c, content));
        }

        [TestMethod]
        public void EmptyContentRulesAlwaysEvaluatesAsTrue()
        {
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Contains("");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("something", Encoding.UTF8, "application/json");

            Assert.IsTrue(e.MatchesContentRules(c, content));
        }

        [TestMethod]
        public void OneContentRulesMatches()
        {
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Contains("thin");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("something", Encoding.UTF8, "application/json");

            Assert.IsTrue(e.MatchesContentRules(c, content));
        }

        [TestMethod]
        public void OneContentRuleDoesNotMatch()
        {
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Contains("nothing");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("something", Encoding.UTF8, "application/json");

            Assert.IsFalse(e.MatchesContentRules(c, content));
        }

        [TestMethod]
        public void OneContentRuleMatchesOneDoesNotMatch()
        {
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Contains("nothing");
            c.Contains("something");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("some", Encoding.UTF8, "application/json");

            Assert.IsFalse(e.MatchesContentRules(c, content));
        }

        [TestMethod]
        public void TwoMatches()
        {
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Contains("some");
            c.Contains("thing");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("something", Encoding.UTF8, "application/json");

            Assert.IsTrue(e.MatchesContentRules(c, content));
        }

        [TestMethod]
        public void ContainsCaseSensitiveMatch()
        {
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Contains("some", true);

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("SOMEthing", Encoding.UTF8, "application/json");

            Assert.IsFalse(e.MatchesContentRules(c, content)); 
        }

        [TestMethod]
        public void ContainsCaseSensitiveMatch2()
        {
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Contains("some", false);

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("SOMEthing", Encoding.UTF8, "application/json");

            Assert.IsTrue(e.MatchesContentRules(c, content));
        }
    }
}
