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
    /// Tests for checking whether UrlParameter conditions are matched. 
    /// </summary>
    [TestClass]
    public class UrlTests
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public UrlTests()
        {
        }

        [TestInitialize]
        public void Initialize()
        {
            Evaluator = new SimulationConditionEvaluator();
        }

        SimulationConditionEvaluator Evaluator;



        [TestMethod]
        public void NoUrlParametersNullQuery()
        {
            SimulationCondition c = new SimulationCondition();

            Assert.IsTrue(Evaluator.MatchesUrlParameters(c, null));
        }

        [TestMethod]
        public void NoUrlParametersEmptyQuery()
        {
            SimulationCondition c = new SimulationCondition();

            Assert.IsTrue(Evaluator.MatchesUrlParameters(c, null));
        }

        [TestMethod]
        public void NoConditionParametersOneInQuery()
        {
            SimulationCondition c = new SimulationCondition();

            Assert.IsTrue(Evaluator.MatchesUrlParameters(c, "a=b"));
        }

        [TestMethod]
        public void OneConditionMatchesCaseSensitive()
        {
            SimulationCondition c = new SimulationCondition();
            c.Parameter("a", "b", ParameterType.UrlParameter);

            Assert.IsTrue(Evaluator.MatchesUrlParameters(c, "a=b"));
        }

        [TestMethod]
        public void OneConditionMatchesCaseSensitiveFails()
        {
            SimulationCondition c = new SimulationCondition();
            c.Parameter("A", "b", ParameterType.UrlParameter);

            Assert.IsFalse(Evaluator.MatchesUrlParameters(c, "a=b"));
        }

        [TestMethod]
        public void OneConditionMatchesCaseInsensitive()
        {
            SimulationCondition c = new SimulationCondition();
            c.Parameter("A", "b", ComparisonType.CaseInsensitive, ParameterType.UrlParameter);

            Assert.IsTrue(Evaluator.MatchesUrlParameters(c, "a=b"));
        }

        [TestMethod]
        public void OneConditionNotMatchesCaseSensitive()
        {
            SimulationCondition c = new SimulationCondition();
            c.Parameter("d", "e", ParameterType.UrlParameter);

            Assert.IsFalse(Evaluator.MatchesUrlParameters(c, "a=b"));
        }

        [TestMethod]
        public void OneConditionWithEncoding()
        {
            SimulationCondition c = new SimulationCondition();
            c.Parameter("d/e", "f/g", ComparisonType.UrlEncoded, ParameterType.UrlParameter);

            Assert.IsTrue(Evaluator.MatchesUrlParameters(c, "d%2fe=f%2fg"));
        }

        [TestMethod]
        public void OneConditionWithoutEncoding()
        {
            SimulationCondition c = new SimulationCondition();
            c.Parameter("d/e", "f/g", ParameterType.UrlParameter);

            Assert.IsFalse(Evaluator.MatchesUrlParameters(c, "d%2fe=f%2fg"));
        }

        [TestMethod]
        public void OneConditionWithBodyParameterDoesNotMatch()
        {
            SimulationCondition c = new SimulationCondition();
            c.Parameter("d", "e", ParameterType.BodyParameter);
            c.Parameter("g", "h", ParameterType.UrlParameter);

            Assert.IsFalse(Evaluator.MatchesUrlParameters(c, "d=e"));
        }
    }
}
