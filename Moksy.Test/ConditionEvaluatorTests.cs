using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Common;
using Moksy.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Test
{
    [TestClass]
    public class ConditionEvaluatorTests
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public ConditionEvaluatorTests()
        {
        }

        [TestInitialize]
        public void Initialize()
        {
            Evaluator = new SimulationConditionEvaluator();
        }

        SimulationConditionEvaluator Evaluator;



        [TestMethod]
        public void PathMatchesNullDefault()
        {
            SimulationCondition c = new SimulationCondition();
            
            Assert.IsTrue(Evaluator.Matches(c, (string) null));
        }

        [TestMethod]
        public void PathDoesNotMatchEmpty()
        {
            SimulationCondition c = new SimulationCondition();
            
            Assert.IsFalse(Evaluator.Matches(c, ""));
        }

        [TestMethod]
        public void PathDoesNotMatchNonsense()
        {
            SimulationCondition c = new SimulationCondition();

            Assert.IsFalse(Evaluator.Matches(c, "NonSense"));
        }

        [TestMethod]
        public void PathMatchesCaseSensitive()
        {
            SimulationCondition c = new SimulationCondition() { Path = "/Product" };

            Assert.IsTrue(Evaluator.Matches(c, "/Product"));
        }

        [TestMethod]
        public void PathDoesNotMatchCaseSensitive()
        {
            SimulationCondition c = new SimulationCondition() { Path = "/Product" };

            Assert.IsFalse(Evaluator.Matches(c, "/PRodUCt"));
        }


        [TestMethod]
        public void MethodDoesNotMatch()
        {
            SimulationCondition c = new SimulationCondition() { HttpMethod = System.Net.Http.HttpMethod.Post };
            Assert.IsFalse(Evaluator.Matches(c, System.Net.Http.HttpMethod.Get));
        }

        [TestMethod]
        public void MethodMatches()
        {
            SimulationCondition c = new SimulationCondition() { HttpMethod = System.Net.Http.HttpMethod.Get };
            Assert.IsTrue(Evaluator.Matches(c, System.Net.Http.HttpMethod.Get));
        }



        [TestMethod]
        public void NullHeadersAlwaysMatches()
        {
            SimulationCondition c = new SimulationCondition();
            Assert.IsTrue(Evaluator.Matches(c, (IEnumerable<Header>) null));

            c.Header("TheHeader", "Woo");
            Assert.IsTrue(Evaluator.Matches(c, (IEnumerable<Header>)null));
        }

        [TestMethod]
        public void EmptyHeadersAlwaysMatches()
        {
            List<Header> headers = new List<Header>();
            SimulationCondition c = new SimulationCondition();
            Assert.IsTrue(Evaluator.Matches(c, headers));

            c.Header("TheHeader", "Woo");
            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void OneHeaderDoesMatchIfThereAreNoHeadersInTheCondition()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));

            SimulationCondition c = new SimulationCondition();

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void OneHeaderMatchesCaseSensitive()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader", "TheValue");

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void OneHeaderDoesnotMatchCaseSensitive()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeadER", "TheValue");

            Assert.IsFalse(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void OneConditionMatchesAgainstTwo()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));
            headers.Add(new Header("TheHeader2", "TheValue2"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader", "TheValue");

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }




    }
}
