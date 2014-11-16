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
    /// Tests conditions where the Body Parameters are matched against the raw body content. 
    /// </summary>
    [TestClass]
    public class ParameterBodyContentTests
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public ParameterBodyContentTests()
        {
        }

        [TestInitialize]
        public void Initialize()
        {
            Evaluator = new SimulationConditionEvaluator();
        }

        SimulationConditionEvaluator Evaluator;



        [TestMethod]
        public void NullContentAlwaysMatches()
        {
            // Will always match
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();

            Assert.IsTrue(e.MatchesBodyParameters(c, (System.Net.Http.HttpContent) null));
        }

        [TestMethod]
        public void EmptyContentAlwaysMatches()
        {
            // Will always match
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("", Encoding.UTF8, "application/json");

            Assert.IsTrue(e.MatchesBodyParameters(c, content));
        }

        [TestMethod]
        public void ContentWithPairMatchesBecauseNoParameters()
        {
            // Will always match
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("a=b", Encoding.UTF8, "application/json");

            Assert.IsTrue(e.MatchesBodyParameters(c, content));
        }

        [TestMethod]
        public void ContentWithMatchingPairMatches()
        {
            // Will always match
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Parameters.Add(new Parameter("a", "b"));

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("a=b", Encoding.UTF8, "application/json");

            Assert.IsTrue(e.MatchesBodyParameters(c, content));
        }

        [TestMethod]
        public void ContentWithNoMatchingPairMatches()
        {
            // Will always match
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Parameter("c", "d");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("a=b", Encoding.UTF8, "application/json");

            Assert.IsFalse(e.MatchesBodyParameters(c, content));
        }

        [TestMethod]
        public void ContentWithOneOfTwoMatchingPairMatches()
        {
            // Will always match
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Parameter("c", "d");
            c.Parameter("a", "b");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("a=b", Encoding.UTF8, "application/json");

            Assert.IsFalse(e.MatchesBodyParameters(c, content));
        }

        [TestMethod]
        public void ContentWithTwoExactMatchingPairMatches()
        {
            // Will always match
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Parameter("c", "d");
            c.Parameter("a", "b");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("a=b&c=d", Encoding.UTF8, "application/json");

            Assert.IsTrue(e.MatchesBodyParameters(c, content));
        }

        [TestMethod]
        public void ContentWithTwoOfThreeMatchingPairMatches()
        {
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Parameter("c", "d");
            c.Parameter("a", "b");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("a=b&c=d&e=f", Encoding.UTF8, "application/json");

            Assert.IsTrue(e.MatchesBodyParameters(c, content));
        }

        [TestMethod]
        public void TwoConditionsSecondOneMatches()
        {
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c1 = new SimulationCondition();
            c1.Parameter("c", "d");
            c1.Parameter("g", "h");

            SimulationCondition c2 = new SimulationCondition();
            c2.Parameter("c", "d");
            c2.Parameter("e", "f");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("a=b&c=d&e=f", Encoding.UTF8, "application/json");

            Assert.IsFalse(e.MatchesBodyParameters(c1, content));
            Assert.IsTrue(e.MatchesBodyParameters(c2, content)); 
        }

        [TestMethod]
        public void ContentWithTwoMatchesButOneNotMatching()
        {
            // Will always match
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Parameter("c", "d");
            c.Parameter("a", "b");
            c.Parameter("m", "n");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("a=b&c=d&e=f", Encoding.UTF8, "application/json");

            Assert.IsFalse(e.MatchesBodyParameters(c, content));
        }
    }
}
