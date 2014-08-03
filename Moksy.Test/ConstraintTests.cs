using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Common;
using Moksy.Common.Constraints;
using Moksy.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Test
{
    [TestClass]
    public class ConstraintTests
    {
        public ConstraintTests()
        {
        }

        [TestMethod]
        public void IsNullIsFalse()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""TheValue"" }");
            IsNullConstraint isnull = new IsNullConstraint("TheProperty");
            Assert.IsFalse(isnull.Evaluate(o));
        }



        [TestMethod]
        public void IsNullIsTrue()
        {
            var o = GetJ(@"{ ""TheProperty"" : null }");
            IsNullConstraint isnull = new IsNullConstraint("TheProperty");
            Assert.IsTrue(isnull.Evaluate(o));
        }

        [TestMethod]
        public void IsNullPropertyNull()
        {
            var o = GetJ(@"{ ""TheProperty"" : null }");
            IsNullConstraint isnull = new IsNullConstraint(null);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void IsNullJobNull()
        {
            IsNullConstraint isnull = new IsNullConstraint("TheProperty");
            Assert.IsFalse(isnull.Evaluate(null)); 
        }




        [TestMethod]
        public void IsMissingIsTrue()
        {
            var o = GetJ(@"{ }");
            IsEmptyConstraint isnull = new IsEmptyConstraint("TheProperty");
            Assert.IsTrue(isnull.Evaluate(o));
        }

        [TestMethod]
        public void IsMissingPropertyNull()
        {
            var o = GetJ(@"{ ""TheProperty"" : null }");
            IsEmptyConstraint isnull = new IsEmptyConstraint("TheProperty");
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void IsMissingPropertySet()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""TheValue"" }");
            IsEmptyConstraint isnull = new IsEmptyConstraint("TheProperty");
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void IsMissingPropertyNameNull()
        {
            var o = GetJ(@"{ ""TheProperty"" : null }");
            IsEmptyConstraint isnull = new IsEmptyConstraint(null);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void IsMissingJobNull()
        {
            IsEmptyConstraint isnull = new IsEmptyConstraint("TheProperty");
            Assert.IsFalse(isnull.Evaluate(null));
        }



        [TestMethod]
        public void LengthEqualsMissing()
        {
            var o = GetJ(@"{ }");
            LengthConstraint isnull = new LengthConstraint("TheProperty", 0);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthPropertyNull()
        {
            var o = GetJ(@"{ ""TheProperty"" : null }");
            LengthConstraint isnull = new LengthConstraint("TheProperty", 0);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthPropertyNameNull()
        {
            var o = GetJ(@"{ ""TheProperty"" : null }");
            LengthConstraint isnull = new LengthConstraint(null, 0);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthJobNull()
        {
            LengthConstraint isnull = new LengthConstraint("TheProperty", 0);
            Assert.IsFalse(isnull.Evaluate(null));
            Assert.AreEqual(0, isnull.ActualLength);
        }



        [TestMethod]
        public void LengthEqualsPropertyEqual0()
        {
            var o = GetJ(@"{ ""TheProperty"" : """" }");
            LengthConstraint isnull = new LengthConstraint("TheProperty", 0);
            Assert.IsTrue(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthEqualsPropertyEqual4()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCD"" }");
            LengthConstraint isnull = new LengthConstraint("TheProperty", 0);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthNotEqualsPropertyEqual45()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCDE"" }");
            LengthConstraint isnull = new LengthConstraint("TheProperty", 4, LengthKind.NotEquals);
            Assert.IsTrue(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthNotEqualsPropertyEqual44()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCD"" }");
            LengthConstraint isnull = new LengthConstraint("TheProperty", 4, LengthKind.NotEquals);
            Assert.IsFalse(isnull.Evaluate(o));
        }



        [TestMethod]
        public void LengthLessThan4PropertyEqual3()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABC"" }");
            LengthConstraint isnull = new LengthConstraint("TheProperty", 4, LengthKind.LessThan);
            Assert.IsTrue(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthLess4ThanPropertyEqual4()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCD"" }");
            LengthConstraint isnull = new LengthConstraint("TheProperty", 4, LengthKind.LessThan);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthLess4ThanPropertyEqual5()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCDE"" }");
            LengthConstraint isnull = new LengthConstraint("TheProperty", 4, LengthKind.LessThan);
            Assert.IsFalse(isnull.Evaluate(o));
        }



        [TestMethod]
        public void LengthGreaterThan4PropertyEqual3()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABC"" }");
            LengthConstraint isnull = new LengthConstraint("TheProperty", 4, LengthKind.GreaterThan);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthGreater4ThanPropertyEqual4()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCD"" }");
            LengthConstraint isnull = new LengthConstraint("TheProperty", 4, LengthKind.GreaterThan);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthLGreater4ThanPropertyEqual5()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCDE"" }");
            LengthConstraint isnull = new LengthConstraint("TheProperty", 4, LengthKind.GreaterThan);
            Assert.IsTrue(isnull.Evaluate(o));
        }



        [TestMethod]
        public void LengthLtgt47PropertyEqual3()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABC"" }");
            LengthConstraint isnull = new LengthConstraint("TheProperty", 4, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(3, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt47PropertyEqual4()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCD"" }");
            LengthConstraint isnull = new LengthConstraint("TheProperty", 4, 7);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(4, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt47PropertyEqual7()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCDEFG"" }");
            LengthConstraint isnull = new LengthConstraint("TheProperty", 4, 7);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(7, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt47PropertyEqual8()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCDEFGH"" }");
            LengthConstraint isnull = new LengthConstraint("TheProperty", 4, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(8, isnull.ActualLength);
        }



        protected JObject GetJ(string json)
        {
            JObject j = JsonConvert.DeserializeObject(json) as JObject;
            return j;
        }



        #region Condition with Constraints

        [TestMethod]
        public void EmptyConstraints()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet");
            Assert.AreEqual(0, s.Constraints.Count);
        }

        [TestMethod]
        public void NullConstraint()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet");
            Assert.AreEqual(s, s.Constraint( (ConstraintBase) null));
        }

        [TestMethod]
        public void OneConstraint()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet");
            
            LengthConstraint c = new LengthConstraint("TheProperty", 10);
            s.Constraint(c);

            Assert.AreEqual(1, s.Constraints.Count);
        }

        [TestMethod]
        public void TwoConstraintsWithRehydration()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet");

            LengthConstraint c = new LengthConstraint("TheProperty", 10);
            s.Constraint(c);

            IsNullConstraint n = new IsNullConstraint("TheOtherProperty");
            s.Constraint(n);

            Assert.AreEqual(2, s.Constraints.Count);

            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
            var json = JsonConvert.SerializeObject(s, settings);

            var h = JsonConvert.DeserializeObject<SimulationCondition>(json, settings);
            Assert.AreEqual(2, h.Constraints.Count);
        }

        [TestMethod]
        public void ConstraintsIsEmpty()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet");

            List<ConstraintBase> b = new List<ConstraintBase>();
            s.Constraint(b);

            Assert.AreEqual(0, s.Constraints.Count);
        }

        [TestMethod]
        public void OneConstraintEnum()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet");

            List<ConstraintBase> b = new List<ConstraintBase>();
            b.Add(new IsNullConstraint("TheProperty"));
            s.Constraint(b);

            Assert.AreEqual(1, s.Constraints.Count);            
        }



        [TestMethod]
        public void NullNoneMatchingConstraints()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.Constraint(new LengthConstraint("TheProperty", 4));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            var result = mgr.FindMatchingConstraints(null, null);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void NullContentonstraints()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.Constraint(new LengthConstraint("TheProperty", 4));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            var result = mgr.FindMatchingConstraints(new List<ConstraintBase>(), null);
            Assert.AreEqual(0, result.Count());
        }






        [TestMethod]
        public void MatchesJsonNoViolation()
        {
            var v = new LengthConstraint("TheProperty", 4);
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.Constraint(v);
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);

            var matches = mgr.FindMatchingConstraints(new List<ConstraintBase>() { v }, "{ }");
            Assert.AreEqual(0, matches.Count());
        }

        [TestMethod]
        public void MatchesJsonOneViolation()
        {
            var v = new LengthConstraint("TheProperty", 4);
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.Constraint(v);
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);

            var matches = mgr.FindMatchingConstraints(new List<ConstraintBase>() { v }, @"{ ""TheProperty"":""ABCD"" }");
            Assert.AreEqual(1, matches.Count());
        }




        [TestMethod]
        public void MatchesNoViolationsNoIndex()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.Constraint(new LengthConstraint("TheProperty", 4));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            StringContent content = new StringContent(@"{""TheProperty"":""ABCDE""}");
            var match = mgr.Match(HttpMethod.Post, content, "/Pet", new List<Header>(), false);
            Assert.IsNull(match);
        }

        [TestMethod]
        public void MatchesOneViolationsNoIndex()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.Constraint(new LengthConstraint("TheProperty", 4));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            StringContent content = new StringContent(@"{""TheProperty"":""ABCD""}");
            var match = mgr.Match(HttpMethod.Post, content, "/Pet", new List<Header>(), false);
            Assert.IsNotNull(match);
            Assert.AreEqual(1, match.Violations.Count);
        }

        [TestMethod]
        public void MatchesNoViolationsIndex()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.NotExists("Kind").Constraint(new LengthConstraint("TheProperty", 4));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            StringContent content = new StringContent(@"{""TheProperty"":""ABCDE""}");
            var match = mgr.Match(HttpMethod.Post, content, "/Pet", new List<Header>(), false);
            Assert.IsNull(match);
        }

        [TestMethod]
        public void MatchesOneViolationsIndex()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.NotExists("Kind").Constraint(new LengthConstraint("TheProperty", 4));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            StringContent content = new StringContent(@"{""TheProperty"":""ABCD""}");
            var match = mgr.Match(HttpMethod.Post, content, "/Pet", new List<Header>(), false);
            Assert.IsNotNull(match);
            Assert.AreEqual(1, match.Violations.Count);
        }

        [TestMethod]
        public void MatchesTwoViolationsIndex()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.NotExists("Kind").Constraint(new LengthConstraint("TheProperty", 4)).Constraint(new IsEmptyConstraint("Other"));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            StringContent content = new StringContent(@"{""TheProperty"":""ABCD""}");
            var match = mgr.Match(HttpMethod.Post, content, "/Pet", new List<Header>(), false);
            Assert.IsNotNull(match);
            Assert.AreEqual(2, match.Violations.Count);
        }

        [TestMethod]
        public void MatchesTwoViolationsOnlyOneIndex()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.NotExists("Kind").Constraint(new LengthConstraint("TheProperty", 4)).Constraint(new IsNullConstraint("Other"));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            StringContent content = new StringContent(@"{""TheProperty"":""ABCD""}");
            var match = mgr.Match(HttpMethod.Post, content, "/Pet", new List<Header>(), false);
            Assert.IsNull(match);
        }

        [TestMethod]
        public void MatchesTwoViolationsIndexExists()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.NotExists("Kind").Constraint(new LengthConstraint("TheProperty", 4)).Constraint(new IsEmptyConstraint("Other"));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            StringContent content = new StringContent(@"{""TheProperty"":""ABCD""}");
            var match = mgr.Match(HttpMethod.Post, content, "/Pet", new List<Header>(), false);
            Assert.IsNotNull(match);
            Assert.AreEqual(2, match.Violations.Count);
        }


        #endregion
    }
}
