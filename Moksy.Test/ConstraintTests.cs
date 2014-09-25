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
            IsNull isnull = new IsNull("TheProperty");
            Assert.IsFalse(isnull.Evaluate(o));
        }



        [TestMethod]
        public void IsNullIsTrue()
        {
            var o = GetJ(@"{ ""TheProperty"" : null }");
            IsNull isnull = new IsNull("TheProperty");
            Assert.IsTrue(isnull.Evaluate(o));
        }

        [TestMethod]
        public void IsNullPropertyNull()
        {
            var o = GetJ(@"{ ""TheProperty"" : null }");
            IsNull isnull = new IsNull(null);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void IsNullJobNull()
        {
            IsNull isnull = new IsNull("TheProperty");
            Assert.IsFalse(isnull.Evaluate(null)); 
        }




        [TestMethod]
        public void IsMissingIsTrue()
        {
            var o = GetJ(@"{ }");
            IsMissing isnull = new IsMissing("TheProperty");
            Assert.IsTrue(isnull.Evaluate(o));
        }

        [TestMethod]
        public void IsMissingPropertyNull()
        {
            var o = GetJ(@"{ ""TheProperty"" : null }");
            IsMissing isnull = new IsMissing("TheProperty");
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void IsMissingPropertySet()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""TheValue"" }");
            IsMissing isnull = new IsMissing("TheProperty");
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void IsMissingPropertyNameNull()
        {
            var o = GetJ(@"{ ""TheProperty"" : null }");
            IsMissing isnull = new IsMissing(null);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void IsMissingJobNull()
        {
            IsMissing isnull = new IsMissing("TheProperty");
            Assert.IsFalse(isnull.Evaluate(null));
        }



        [TestMethod]
        public void LengthEqualsMissingImplicit()
        {
            var o = GetJ(@"{ }");
            LengthEquals isnull = new LengthEquals("TheProperty", 0);
            Assert.IsTrue(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthEqualsMissingNotImplicit()
        {
            var o = GetJ(@"{ }");
            LengthEquals isnull = new LengthEquals("TheProperty", 0, false, true);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthPropertyNullImplicit()
        {
            var o = GetJ(@"{ ""TheProperty"" : null }");
            LengthEquals isnull = new LengthEquals("TheProperty", 0);
            Assert.IsTrue(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthPropertyNullNotImplicit()
        {
            var o = GetJ(@"{ ""TheProperty"" : null }");
            LengthEquals isnull = new LengthEquals("TheProperty", 0, false, false);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthPropertyNameNull()
        {
            var o = GetJ(@"{ ""TheProperty"" : null }");
            LengthEquals isnull = new LengthEquals(null, 0);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthJobNull()
        {
            LengthEquals isnull = new LengthEquals("TheProperty", 0);
            Assert.IsFalse(isnull.Evaluate(null));
            Assert.AreEqual(0, isnull.ActualLength);
        }



        [TestMethod]
        public void LengthEqualsPropertyEqual0()
        {
            var o = GetJ(@"{ ""TheProperty"" : """" }");
            LengthEquals isnull = new LengthEquals("TheProperty", 0);
            Assert.IsTrue(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthEqualsPropertyEqual4()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCD"" }");
            LengthEquals isnull = new LengthEquals("TheProperty", 0);
            Assert.IsFalse(isnull.Evaluate(o));
        }



        [TestMethod]
        public void LengthNotEquals0MissingImplicit()
        {
            var o = GetJ(@"{}");
            LengthNotEquals isnull = new LengthNotEquals("TheProperty", 0);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthNotEquals0MissingExplicit()
        {
            var o = GetJ(@"{}");
            LengthNotEquals isnull = new LengthNotEquals("TheProperty", 0, false, true);
            Assert.IsTrue(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthNotEquals0NullImplicit()
        {
            var o = GetJ(@"{""TheProperty"":null}");
            LengthNotEquals isnull = new LengthNotEquals("TheProperty", 0);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthNotEquals0NullExplicit()
        {
            var o = GetJ(@"{""TheProperty"":null}");
            LengthNotEquals isnull = new LengthNotEquals("TheProperty", 0, false, false);
            Assert.IsTrue(isnull.Evaluate(o));
        }



        [TestMethod]
        public void LengthNotEquals00()
        {
            var o = GetJ(@"{ ""TheProperty"" : """" }");
            LengthNotEquals isnull = new LengthNotEquals("TheProperty", 0);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthNotEqualsPropertyEqual1()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""A"" }");
            LengthNotEquals isnull = new LengthNotEquals("TheProperty", 1);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        public void LengthNotEqualsPropertyEqual12()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""AB"" }");
            LengthNotEquals isnull = new LengthNotEquals("TheProperty", 1);
            Assert.IsTrue(isnull.Evaluate(o));
        }


        
        [TestMethod]
        public void LengthLessThan0MissingImplicit()
        {
            var o = GetJ(@"{}");
            LengthLessThan isnull = new LengthLessThan("TheProperty", 0);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthLessThan0MissingNotImplicit()
        {
            var o = GetJ(@"{}");
            LengthLessThan isnull = new LengthLessThan("TheProperty", 0, false, true);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthLessThan1MissingImplicit()
        {
            var o = GetJ(@"{ }");
            LengthLessThan isnull = new LengthLessThan("TheProperty", 1);
            Assert.IsTrue(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthLessThan1MissingNotImplicit()
        {
            var o = GetJ(@"{ }");
            LengthLessThan isnull = new LengthLessThan("TheProperty", 1, false, true);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthLessThan4PropertyEqual3()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABC"" }");
            LengthLessThan isnull = new LengthLessThan("TheProperty", 4);
            Assert.IsTrue(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthLess4ThanPropertyEqual4()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCD"" }");
            LengthLessThan isnull = new LengthLessThan("TheProperty", 4);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthLess4ThanPropertyEqual5()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCDE"" }");
            LengthLessThan isnull = new LengthLessThan("TheProperty", 4);
            Assert.IsFalse(isnull.Evaluate(o));
        }



        [TestMethod]
        public void LengthGreaterThan4PropertyEqual3()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABC"" }");
            LengthGreaterThan isnull = new LengthGreaterThan("TheProperty", 4);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthGreater4ThanPropertyEqual4()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCD"" }");
            LengthGreaterThan isnull = new LengthGreaterThan("TheProperty", 4);
            Assert.IsFalse(isnull.Evaluate(o));
        }

        [TestMethod]
        public void LengthLGreater4ThanPropertyEqual5()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCDE"" }");
            LengthGreaterThan isnull = new LengthGreaterThan("TheProperty", 4);
            Assert.IsTrue(isnull.Evaluate(o));
        }




        [TestMethod]
        public void LengthLtgt07PropertyMissingImplicit()
        {
            var o = GetJ(@"{}");
            LengthLessThanOrGreaterThan isnull = new LengthLessThanOrGreaterThan("TheProperty", 0, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt07PropertyMissingNotImplicit()
        {
            var o = GetJ(@"{}");
            LengthLessThanOrGreaterThan isnull = new LengthLessThanOrGreaterThan("TheProperty", 0, 7, false, true);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt17PropertyMissingImplicit()
        {
            var o = GetJ(@"{}");
            LengthLessThanOrGreaterThan isnull = new LengthLessThanOrGreaterThan("TheProperty", 1, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt17PropertyMissingNotImplicit()
        {
            var o = GetJ(@"{}");
            LengthLessThanOrGreaterThan isnull = new LengthLessThanOrGreaterThan("TheProperty", 1, 7, false, true);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt07PropertyNullImplicit()
        {
            var o = GetJ(@"{""TheProperty"":null}");
            LengthLessThanOrGreaterThan isnull = new LengthLessThanOrGreaterThan("TheProperty", 0, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt07PropertyNullNotImplicit()
        {
            var o = GetJ(@"{""TheProperty"":null}");
            LengthLessThanOrGreaterThan isnull = new LengthLessThanOrGreaterThan("TheProperty", 0, 7, false, false);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt17PropertyNullImplicit()
        {
            var o = GetJ(@"{""TheProperty"":null}");
            LengthLessThanOrGreaterThan isnull = new LengthLessThanOrGreaterThan("TheProperty", 1, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt17PropertyNullNotImplicit()
        {
            var o = GetJ(@"{""TheProperty"":null}");
            LengthLessThanOrGreaterThan isnull = new LengthLessThanOrGreaterThan("TheProperty", 1, 7, false, false);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt47PropertyEqual3()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABC"" }");
            LengthLessThanOrGreaterThan isnull = new LengthLessThanOrGreaterThan("TheProperty", 4, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(3, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt47PropertyEqual4()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCD"" }");
            LengthLessThanOrGreaterThan isnull = new LengthLessThanOrGreaterThan("TheProperty", 4, 7);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(4, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt47PropertyEqual7()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCDEFG"" }");
            LengthLessThanOrGreaterThan isnull = new LengthLessThanOrGreaterThan("TheProperty", 4, 7);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(7, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthLtgt47PropertyEqual8()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCDEFGH"" }");
            LengthLessThanOrGreaterThan isnull = new LengthLessThanOrGreaterThan("TheProperty", 4, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(8, isnull.ActualLength);
        }




        [TestMethod]
        public void LengthBetween07PropertyMissingImplicit()
        {
            var o = GetJ(@"{}");
            LengthBetween isnull = new LengthBetween("TheProperty", 0, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween07PropertyMissingNotImplicit()
        {
            var o = GetJ(@"{}");
            LengthBetween isnull = new LengthBetween("TheProperty", 0, 7, false, true);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween17PropertyMissingImplicit()
        {
            var o = GetJ(@"{}");
            LengthBetween isnull = new LengthBetween("TheProperty", 1, 7);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween17PropertyMissingNotImplicit()
        {
            var o = GetJ(@"{}");
            LengthBetween isnull = new LengthBetween("TheProperty", 1, 7, false, true);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween07PropertyNullImplicit()
        {
            var o = GetJ(@"{""TheProperty"":null}");
            LengthBetween isnull = new LengthBetween("TheProperty", 0, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween07PropertyNullNotImplicit()
        {
            var o = GetJ(@"{""TheProperty"":null}");
            LengthBetween isnull = new LengthBetween("TheProperty", 0, 7, false, false);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween17PropertyNullImplicit()
        {
            var o = GetJ(@"{""TheProperty"":null}");
            LengthBetween isnull = new LengthBetween("TheProperty", 1, 7);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween17PropertyNullNotImplicit()
        {
            var o = GetJ(@"{""TheProperty"":null}");
            LengthBetween isnull = new LengthBetween("TheProperty", 1, 7, false, false);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(0, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween47PropertyEqual3()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABC"" }");
            LengthBetween isnull = new LengthBetween("TheProperty", 4, 7);
            Assert.IsFalse(isnull.Evaluate(o));
            Assert.AreEqual(3, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween47PropertyEqual4()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCD"" }");
            LengthBetween isnull = new LengthBetween("TheProperty", 4, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(4, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween47PropertyEqual7()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCDEFG"" }");
            LengthBetween isnull = new LengthBetween("TheProperty", 4, 7);
            Assert.IsTrue(isnull.Evaluate(o));
            Assert.AreEqual(7, isnull.ActualLength);
        }

        [TestMethod]
        public void LengthBetween47PropertyEqual8()
        {
            var o = GetJ(@"{ ""TheProperty"" : ""ABCDEFGH"" }");
            LengthBetween isnull = new LengthBetween("TheProperty", 4, 7);
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

            LengthEquals c = new LengthEquals("TheProperty", 10);
            s.Constraint(c);

            Assert.AreEqual(1, s.Constraints.Count);
        }

        [TestMethod]
        public void TwoConstraintsWithRehydration()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet");

            LengthEquals c = new LengthEquals("TheProperty", 10);
            s.Constraint(c);

            IsNull n = new IsNull("TheOtherProperty");
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
            b.Add(new IsNull("TheProperty"));
            s.Constraint(b);

            Assert.AreEqual(1, s.Constraints.Count);            
        }



        [TestMethod]
        public void NullNoneMatchingConstraints()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.Constraint(new LengthEquals("TheProperty", 4));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            var result = mgr.FindMatchingConstraints(null, null, null);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void NullContentonstraints()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.Constraint(new LengthEquals("TheProperty", 4));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            var result = mgr.FindMatchingConstraints(new List<ConstraintBase>(), null, null);
            Assert.AreEqual(0, result.Count());
        }






        [TestMethod]
        public void MatchesJsonNoViolation()
        {
            var v = new LengthEquals("TheProperty", 4);
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.Constraint(v);
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);

            var matches = mgr.FindMatchingConstraints(new List<ConstraintBase>() { v }, "{ }", null);
            Assert.AreEqual(0, matches.Count());
        }

        [TestMethod]
        public void MatchesJsonOneViolation()
        {
            var v = new LengthEquals("TheProperty", 4);
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.Constraint(v);
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);

            var matches = mgr.FindMatchingConstraints(new List<ConstraintBase>() { v }, @"{ ""TheProperty"":""ABCD"" }", null);
            Assert.AreEqual(1, matches.Count());
        }




        [TestMethod]
        public void MatchesNoViolationsNoIndex()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.Constraint(new LengthEquals("TheProperty", 4));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            StringContent content = new StringContent(@"{""TheProperty"":""ABCDE""}");
            var match = mgr.Match(HttpMethod.Post, content, "/Pet", new List<Header>(), false);
            Assert.IsNull(match);
        }

        [TestMethod]
        public void MatchesOneViolationsNoIndex()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.Constraint(new LengthEquals("TheProperty", 4));
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
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.NotExists("Kind").Constraint(new LengthEquals("TheProperty", 4));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            StringContent content = new StringContent(@"{""TheProperty"":""ABCDE""}");
            var match = mgr.Match(HttpMethod.Post, content, "/Pet", new List<Header>(), false);
            Assert.IsNull(match);
        }

        [TestMethod]
        public void MatchesOneViolationsIndex()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.NotExists("Kind").Constraint(new LengthEquals("TheProperty", 4));
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
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.NotExists("Kind").Constraint(new LengthEquals("TheProperty", 4)).Constraint(new IsMissing("Other"));
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
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.NotExists("Kind").Constraint(new LengthEquals("TheProperty", 4)).Constraint(new IsNull("Other"));
            SimulationManager mgr = new SimulationManager();
            mgr.Add(s.Simulation);
            StringContent content = new StringContent(@"{""TheProperty"":""ABCD""}");
            var match = mgr.Match(HttpMethod.Post, content, "/Pet", new List<Header>(), false);
            Assert.IsNull(match);
        }

        [TestMethod]
        public void MatchesTwoViolationsIndexExists()
        {
            var s = Moksy.Common.SimulationFactory.When.I.Post().ToImdb("/Pet").With.NotExists("Kind").Constraint(new LengthEquals("TheProperty", 4)).Constraint(new IsMissing("Other"));
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
