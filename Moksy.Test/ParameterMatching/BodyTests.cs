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
    /// Tests conditions where the Parameter is part of the Body content. 
    /// </summary>
    [TestClass]
    public class BodyTests
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public BodyTests()
        {
        }

        [TestInitialize]
        public void Initialize()
        {
            Evaluator = new SimulationConditionEvaluator();
        }

        SimulationConditionEvaluator Evaluator;



        // Body Parameters are passed as a=b&c=d in the Body of the request. 
        [TestMethod]
        public void NoBodyParametersMatches()
        {
            List<Parameter> ps = new List<Parameter>();

            SimulationCondition c = new SimulationCondition();

            Assert.IsTrue(Evaluator.Matches(c, ps));
        }

        [TestMethod]
        public void ExactlyOneParameterMatch()
        {
             List<Parameter> ps = new List<Parameter>();
             ps.Add(new Parameter("thename", "thevalue"));

            SimulationCondition c = new SimulationCondition();
            c.Parameter("thename", "thevalue");

            Assert.IsTrue(Evaluator.Matches(c, ps));
        }

        [TestMethod]
        public void OneParameterButNotAMatch()
        {
            List<Parameter> ps = new List<Parameter>();
            ps.Add(new Parameter("thename2", "thevalue2"));

            SimulationCondition c = new SimulationCondition();
            c.Parameter("thename", "thevalue");

            Assert.IsFalse(Evaluator.Matches(c, ps));
        }

        [TestMethod]
        public void OneConditionButNoParameters()
        {
            List<Parameter> ps = new List<Parameter>();

            SimulationCondition c = new SimulationCondition();
            c.Parameter("thename", "thevalue");

            Assert.IsFalse(Evaluator.Matches(c, ps));
        }

        [TestMethod]
        public void OneParameterButNoCondition()
        {
            List<Parameter> ps = new List<Parameter>();
            ps.Add(new Parameter("thename2", "thevalue2"));

            SimulationCondition c = new SimulationCondition();

            Assert.IsTrue(Evaluator.Matches(c, ps));
        }

        [TestMethod]
        public void ParameterExistsValueIgnored()
        {
            List<Parameter> ps = new List<Parameter>();
            ps.Add(new Parameter("thename2"));

            SimulationCondition c = new SimulationCondition();
            c.Parameter("thename", "thevalue");

            Assert.IsFalse(Evaluator.Matches(c, ps));
        }

        [TestMethod]
        public void ParameterExistsValueIgnored2()
        {
            List<Parameter> ps = new List<Parameter>();
            ps.Add(new Parameter("thename", "thevalue"));

            SimulationCondition c = new SimulationCondition();
            c.Parameter("thename");

            Assert.IsTrue(Evaluator.Matches(c, ps));
        }
    }
}
