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
            IsMissingConstraint isnull = new IsMissingConstraint("TheProperty");
            Assert.IsTrue(isnull.Evaluate(o));
        }

        [TestMethod]
        public void IsMissingPropertyNull()
        {
            var o = GetJ(@"{ ""TheProperty"" : null }");
            IsMissingConstraint isnull = new IsMissingConstraint("TheProperty");
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void IsMissingPropertySet()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""TheValue"" }");
            IsMissingConstraint isnull = new IsMissingConstraint("TheProperty");
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void IsMissingPropertyNameNull()
        {
            var o = GetJ(@"{ ""TheProperty"" : null }");
            IsMissingConstraint isnull = new IsMissingConstraint(null);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void IsMissingJobNull()
        {
            IsMissingConstraint isnull = new IsMissingConstraint("TheProperty");
            Assert.IsFalse(isnull.Evaluate(null));
        }



        [TestMethod]
        public void LengthEqualsMissingImplicit()
        {
            var o = GetJ(@"{ }");
            LengthEqualsConstraint isnull = new LengthEqualsConstraint("TheProperty", 0);
            Assert.IsTrue(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthEqualsMissingNotImplicit()
        {
            var o = GetJ(@"{ }");
            LengthEqualsConstraint isnull = new LengthEqualsConstraint("TheProperty", 0, false, true);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthPropertyNullImplicit()
        {
            var o = GetJ(@"{ ""TheProperty"" : null }");
            LengthEqualsConstraint isnull = new LengthEqualsConstraint("TheProperty", 0);
            Assert.IsTrue(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthPropertyNullNotImplicit()
        {
            var o = GetJ(@"{ ""TheProperty"" : null }");
            LengthEqualsConstraint isnull = new LengthEqualsConstraint("TheProperty", 0, false, false);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthPropertyNameNull()
        {
            var o = GetJ(@"{ ""TheProperty"" : null }");
            LengthEqualsConstraint isnull = new LengthEqualsConstraint(null, 0);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthJobNull()
        {
            LengthEqualsConstraint isnull = new LengthEqualsConstraint("TheProperty", 0);
            Assert.IsFalse(isnull.Evaluate(null));
            Assert.AreEqual(0, isnull.ActualLength);
        }



        [TestMethod]
        public void LengthEqualsPropertyEqual0()
        {
            var o = GetJ(@"{ ""TheProperty"" : """" }");
            LengthEqualsConstraint isnull = new LengthEqualsConstraint("TheProperty", 0);
            Assert.IsTrue(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthEqualsPropertyEqual4()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCD"" }");
            LengthEqualsConstraint isnull = new LengthEqualsConstraint("TheProperty", 0);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthNotEqualsPropertyEqual45()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCDE"" }");
            LengthLessThanOrGreaterThanConstraint isnull = new LengthNotEqualsConstraint("TheProperty", 4);
            Assert.IsTrue(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthNotEqualsPropertyEqual44()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCD"" }");
            LengthLessThanOrGreaterThanConstraint isnull = new LengthNotEqualsConstraint("TheProperty", 4);
            Assert.IsFalse(isnull.Evaluate(o));
        }


        
        [TestMethod]
        public void LengthLessThan0MissingImplicit()
        {
            var o = GetJ(@"{}");
            LengthLessThanConstraint isnull = new LengthLessThanConstraint("TheProperty", 0);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthLessThan0MissingNotImplicit()
        {
            var o = GetJ(@"{}");
            LengthLessThanConstraint isnull = new LengthLessThanConstraint("TheProperty", 0, false, true);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthLessThan1MissingImplicit()
        {
            var o = GetJ(@"{ }");
            LengthLessThanConstraint isnull = new LengthLessThanConstraint("TheProperty", 1);
            Assert.IsTrue(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthLessThan1MissingNotImplicit()
        {
            var o = GetJ(@"{ }");
            LengthLessThanConstraint isnull = new LengthLessThanConstraint("TheProperty", 1, false, true);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthLessThan4PropertyEqual3()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABC"" }");
            LengthLessThanConstraint isnull = new LengthLessThanConstraint("TheProperty", 4);
            Assert.IsTrue(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthLess4ThanPropertyEqual4()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCD"" }");
            LengthLessThanConstraint isnull = new LengthLessThanConstraint("TheProperty", 4);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthLess4ThanPropertyEqual5()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCDE"" }");
            LengthLessThanConstraint isnull = new LengthLessThanConstraint("TheProperty", 4);
            Assert.IsFalse(isnull.Evaluate(o));
        }



        [TestMethod]
        public void LengthGreaterThan4PropertyEqual3()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABC"" }");
            LengthGreaterThanConstraint isnull = new LengthGreaterThanConstraint("TheProperty", 4);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthGreater4ThanPropertyEqual4()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCD"" }");
            LengthGreaterThanConstraint isnull = new LengthGreaterThanConstraint("TheProperty", 4);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthLGreater4ThanPropertyEqual5()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCDE"" }");
            LengthGreaterThanConstraint isnull = new LengthGreaterThanConstraint("TheProperty", 4);
            Assert.IsTrue(isnull.Evaluate(o));
        }




        [TestMethod]
        public void LengthLtgt07PropertyMissingImplicit()
        {
            var o = GetJ(@"{}");
            LengthLessThanOrGreaterThanConstraint isnull = new LengthLessThanOrGreaterThanConstraint("TheProperty", 0, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt07PropertyMissingNotImplicit()
        {
            var o = GetJ(@"{}");
            LengthLessThanOrGreaterThanConstraint isnull = new LengthLessThanOrGreaterThanConstraint("TheProperty", 0, 7, false, true);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt17PropertyMissingImplicit()
        {
            var o = GetJ(@"{}");
            LengthLessThanOrGreaterThanConstraint isnull = new LengthLessThanOrGreaterThanConstraint("TheProperty", 1, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt17PropertyMissingNotImplicit()
        {
            var o = GetJ(@"{}");
            LengthLessThanOrGreaterThanConstraint isnull = new LengthLessThanOrGreaterThanConstraint("TheProperty", 1, 7, false, true);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt07PropertyNullImplicit()
        {
            var o = GetJ(@"{""TheProperty"":null}");
            LengthLessThanOrGreaterThanConstraint isnull = new LengthLessThanOrGreaterThanConstraint("TheProperty", 0, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt07PropertyNullNotImplicit()
        {
            var o = GetJ(@"{""TheProperty"":null}");
            LengthLessThanOrGreaterThanConstraint isnull = new LengthLessThanOrGreaterThanConstraint("TheProperty", 0, 7, false, false);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt17PropertyNullImplicit()
        {
            var o = GetJ(@"{""TheProperty"":null}");
            LengthLessThanOrGreaterThanConstraint isnull = new LengthLessThanOrGreaterThanConstraint("TheProperty", 1, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt17PropertyNullNotImplicit()
        {
            var o = GetJ(@"{""TheProperty"":null}");
            LengthLessThanOrGreaterThanConstraint isnull = new LengthLessThanOrGreaterThanConstraint("TheProperty", 1, 7, false, false);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt47PropertyEqual3()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABC"" }");
            LengthLessThanOrGreaterThanConstraint isnull = new LengthLessThanOrGreaterThanConstraint("TheProperty", 4, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(3, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt47PropertyEqual4()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCD"" }");
            LengthLessThanOrGreaterThanConstraint isnull = new LengthLessThanOrGreaterThanConstraint("TheProperty", 4, 7);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(4, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt47PropertyEqual7()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCDEFG"" }");
            LengthLessThanOrGreaterThanConstraint isnull = new LengthLessThanOrGreaterThanConstraint("TheProperty", 4, 7);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(7, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt47PropertyEqual8()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCDEFGH"" }");
            LengthLessThanOrGreaterThanConstraint isnull = new LengthLessThanOrGreaterThanConstraint("TheProperty", 4, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(8, isnull.ActualLength);
        }




        [TestMethod]
        public void LengthBetween07PropertyMissingImplicit()
        {
            var o = GetJ(@"{}");
            LengthBetweenConstraint isnull = new LengthBetweenConstraint("TheProperty", 0, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween07PropertyMissingNotImplicit()
        {
            var o = GetJ(@"{}");
            LengthBetweenConstraint isnull = new LengthBetweenConstraint("TheProperty", 0, 7, false, true);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween17PropertyMissingImplicit()
        {
            var o = GetJ(@"{}");
            LengthBetweenConstraint isnull = new LengthBetweenConstraint("TheProperty", 1, 7);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween17PropertyMissingNotImplicit()
        {
            var o = GetJ(@"{}");
            LengthBetweenConstraint isnull = new LengthBetweenConstraint("TheProperty", 1, 7, false, true);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween07PropertyNullImplicit()
        {
            var o = GetJ(@"{""TheProperty"":null}");
            LengthBetweenConstraint isnull = new LengthBetweenConstraint("TheProperty", 0, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween07PropertyNullNotImplicit()
        {
            var o = GetJ(@"{""TheProperty"":null}");
            LengthBetweenConstraint isnull = new LengthBetweenConstraint("TheProperty", 0, 7, false, false);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween17PropertyNullImplicit()
        {
            var o = GetJ(@"{""TheProperty"":null}");
            LengthBetweenConstraint isnull = new LengthBetweenConstraint("TheProperty", 1, 7);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween17PropertyNullNotImplicit()
        {
            var o = GetJ(@"{""TheProperty"":null}");
            LengthBetweenConstraint isnull = new LengthBetweenConstraint("TheProperty", 1, 7, false, false);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween47PropertyEqual3()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABC"" }");
            LengthBetweenConstraint isnull = new LengthBetweenConstraint("TheProperty", 4, 7);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(3, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween47PropertyEqual4()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCD"" }");
            LengthBetweenConstraint isnull = new LengthBetweenConstraint("TheProperty", 4, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(4, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween47PropertyEqual7()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCDEFG"" }");
            LengthBetweenConstraint isnull = new LengthBetweenConstraint("TheProperty", 4, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(7, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween47PropertyEqual8()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCDEFGH"" }");
            LengthBetweenConstraint isnull = new LengthBetweenConstraint("TheProperty", 4, 7);
            Assert.IsFalse(isnull.Evaluate(o));
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

            LengthEqualsConstraint c = new LengthEqualsConstraint("TheProperty", 10);
            s.Constraint(c);

            Assert.AreEqual(1, s.Constraints.Count);
        }

        [TestMethod]
        public void TwoConstraintsWithRehydration()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet");

            LengthEqualsConstraint c = new LengthEqualsConstraint("TheProperty", 10);
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
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.Constraint(new LengthEqualsConstraint("TheProperty", 4));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            var result = mgr.FindMatchingConstraints(null, null);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void NullContentonstraints()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.Constraint(new LengthEqualsConstraint("TheProperty", 4));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            var result = mgr.FindMatchingConstraints(new List<ConstraintBase>(), null);
            Assert.AreEqual(0, result.Count());
        }






        [TestMethod]
        public void MatchesJsonNoViolation()
        {
            var v = new LengthEqualsConstraint("TheProperty", 4);
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.Constraint(v);
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);

            var matches = mgr.FindMatchingConstraints(new List<ConstraintBase>() { v }, "{ }");
            Assert.AreEqual(0, matches.Count());
        }

        [TestMethod]
        public void MatchesJsonOneViolation()
        {
            var v = new LengthEqualsConstraint("TheProperty", 4);
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.Constraint(v);
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);

            var matches = mgr.FindMatchingConstraints(new List<ConstraintBase>() { v }, @"{ ""TheProperty"":""ABCD"" }");
            Assert.AreEqual(1, matches.Count());
        }




        [TestMethod]
        public void MatchesNoViolationsNoIndex()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.Constraint(new LengthEqualsConstraint("TheProperty", 4));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            StringContent content = new StringContent(@"{""TheProperty"":""ABCDE""}");
            var match = mgr.Match(HttpMethod.Post, content, "/Pet", new List<Header>(), false);
            Assert.IsNull(match);
        }

        [TestMethod]
        public void MatchesOneViolationsNoIndex()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.Constraint(new LengthEqualsConstraint("TheProperty", 4));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            StringContent content = new StringContent(@"{""TheProperty"":""ABCD""}");
            var match = mgr.Match(HttpMethod.Post, content, "/Pet", new List<Header>(), false);
            Assert.IsNotNull(match);
            Assert.AreEqual(1, match.EvaluatedMatchingConstraints.Count);
        }

        [TestMethod]
        public void MatchesNoViolationsIndex()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.NotExists("Kind").Constraint(new LengthEqualsConstraint("TheProperty", 4));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            StringContent content = new StringContent(@"{""TheProperty"":""ABCDE""}");
            var match = mgr.Match(HttpMethod.Post, content, "/Pet", new List<Header>(), false);
            Assert.IsNull(match);
        }

        [TestMethod]
        public void MatchesOneViolationsIndex()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.NotExists("Kind").Constraint(new LengthEqualsConstraint("TheProperty", 4));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            StringContent content = new StringContent(@"{""TheProperty"":""ABCD""}");
            var match = mgr.Match(HttpMethod.Post, content, "/Pet", new List<Header>(), false);
            Assert.IsNotNull(match);
            Assert.AreEqual(1, match.EvaluatedMatchingConstraints.Count);
        }

        [TestMethod]
        public void MatchesTwoViolationsIndex()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.NotExists("Kind").Constraint(new LengthEqualsConstraint("TheProperty", 4)).Constraint(new IsMissingConstraint("Other"));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            StringContent content = new StringContent(@"{""TheProperty"":""ABCD""}");
            var match = mgr.Match(HttpMethod.Post, content, "/Pet", new List<Header>(), false);
            Assert.IsNotNull(match);
            Assert.AreEqual(2, match.EvaluatedMatchingConstraints.Count);
        }

        [TestMethod]
        public void MatchesTwoViolationsOnlyOneIndex()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.NotExists("Kind").Constraint(new LengthEqualsConstraint("TheProperty", 4)).Constraint(new IsNullConstraint("Other"));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            StringContent content = new StringContent(@"{""TheProperty"":""ABCD""}");
            var match = mgr.Match(HttpMethod.Post, content, "/Pet", new List<Header>(), false);
            Assert.IsNull(match);
        }

        [TestMethod]
        public void MatchesTwoViolationsIndexExists()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.NotExists("Kind").Constraint(new LengthEqualsConstraint("TheProperty", 4)).Constraint(new IsMissingConstraint("Other"));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            StringContent content = new StringContent(@"{""TheProperty"":""ABCD""}");
            var match = mgr.Match(HttpMethod.Post, content, "/Pet", new List<Header>(), false);
            Assert.IsNotNull(match);
            Assert.AreEqual(2, match.EvaluatedMatchingConstraints.Count);
        }


        #endregion
    }
}
