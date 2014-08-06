using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Moksy.Common.Constraints;

namespace Moksy.Test
{
    [TestClass]
    public class ConstraintResponseTests
    {
        public ConstraintResponseTests()
        {
        }



        [TestMethod]
        public void IsMissingTrue()
        {
            IsMissingConstraint c = new IsMissingConstraint("TheProperty");
            var j = GetJ(@"{}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""IsMissing"",""PropertyName"":""TheProperty"",""PropertyValue"":null,""PropertyHasValue"":false,""Description"":""The property 'TheProperty' was not expected to be in the Json. ""}", c.GetState(j));
        }

        [TestMethod]
        public void IsMissingNull()
        {
            IsMissingConstraint c = new IsMissingConstraint("TheProperty");
            var j = GetJ(@"{""TheProperty"":null}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""IsMissing"",""PropertyName"":""TheProperty"",""PropertyValue"":null,""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was not expected to be in the Json. ""}", c.GetState(j));
        }

        [TestMethod]
        public void IsMissingEmptyString()
        {
            IsMissingConstraint c = new IsMissingConstraint("TheProperty");
            var j = GetJ(@"{""TheProperty"":""""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""IsMissing"",""PropertyName"":""TheProperty"",""PropertyValue"":"""",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was not expected to be in the Json. ""}", c.GetState(j));
        }

        [TestMethod]
        public void IsMissingValueString()
        {
            IsMissingConstraint c = new IsMissingConstraint("TheProperty");
            var j = GetJ(@"{""TheProperty"":""TheValue""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""IsMissing"",""PropertyName"":""TheProperty"",""PropertyValue"":""TheValue"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was not expected to be in the Json. ""}", c.GetState(j));
        }




        [TestMethod]
        public void IsNullFalse()
        {
            IsNullConstraint c = new IsNullConstraint("TheProperty");
            var j = GetJ(@"{}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""IsNull"",""PropertyName"":""TheProperty"",""PropertyValue"":null,""PropertyHasValue"":false,""Description"":""The property 'TheProperty' was expected to be Null.""}", c.GetState(j));
        }

        [TestMethod]
        public void IsNullNull()
        {
            IsNullConstraint c = new IsNullConstraint("TheProperty");
            var j = GetJ(@"{""TheProperty"":null}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""IsNull"",""PropertyName"":""TheProperty"",""PropertyValue"":null,""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be Null.""}", c.GetState(j));
        }

        [TestMethod]
        public void IsNullEmptyString()
        {
            IsNullConstraint c = new IsNullConstraint("TheProperty");
            var j = GetJ(@"{""TheProperty"":""""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""IsNull"",""PropertyName"":""TheProperty"",""PropertyValue"":"""",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be Null.""}", c.GetState(j));
        }

        [TestMethod]
        public void IsNullValueString()
        {
            IsNullConstraint c = new IsNullConstraint("TheProperty");
            var j = GetJ(@"{""TheProperty"":""TheValue""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""IsNull"",""PropertyName"":""TheProperty"",""PropertyValue"":""TheValue"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be Null.""}", c.GetState(j));
        }




        [TestMethod]
        public void LengthEqualsMissing()
        {
            LengthEqualsConstraint c = new LengthEqualsConstraint("TheProperty", 4);
            var j = GetJ(@"{}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""Equals"",""ExpectedLength"":4,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":false,""Description"":""The property 'TheProperty' was expected to be of length '4'.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthEqualsNull()
        {
            LengthEqualsConstraint c = new LengthEqualsConstraint("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":null}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""Equals"",""ExpectedLength"":4,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be of length '4'.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthEqualsEmpty()
        {
            LengthEqualsConstraint c = new LengthEqualsConstraint("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""Equals"",""ExpectedLength"":4,""ActualLength"":0,""PropertyValue"":"""",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be of length '4'.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthEqualsTrue()
        {
            LengthEqualsConstraint c = new LengthEqualsConstraint("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""ABCD""}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""Equals"",""ExpectedLength"":4,""ActualLength"":4,""PropertyValue"":""ABCD"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be of length '4'.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthEqualsFalse()
        {
            LengthEqualsConstraint c = new LengthEqualsConstraint("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""ABCDEF""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""Equals"",""ExpectedLength"":4,""ActualLength"":6,""PropertyValue"":""ABCDEF"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be of length '4'.""}", c.GetState(j));
        }



        [TestMethod]
        public void LengthNotEqualsMissing()
        {
            LengthLessThanOrGreaterThanConstraint c = new LengthNotEqualsConstraint("TheProperty", 4);
            var j = GetJ(@"{}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""NotEquals"",""ExpectedLength"":4,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":false,""Description"":""The property 'TheProperty' was expected to not be of length '4'.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthNotEqualsNull()
        {
            LengthLessThanOrGreaterThanConstraint c = new LengthNotEqualsConstraint("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":null}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""NotEquals"",""ExpectedLength"":4,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to not be of length '4'.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthNotEqualsEmpty()
        {
            LengthLessThanOrGreaterThanConstraint c = new LengthNotEqualsConstraint("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""""}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""NotEquals"",""ExpectedLength"":4,""ActualLength"":0,""PropertyValue"":"""",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to not be of length '4'.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthNotEqualsTrue()
        {
            LengthLessThanOrGreaterThanConstraint c = new LengthNotEqualsConstraint("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""ABCD""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""NotEquals"",""ExpectedLength"":4,""ActualLength"":4,""PropertyValue"":""ABCD"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to not be of length '4'.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthNotEqualsFalse()
        {
            LengthLessThanOrGreaterThanConstraint c = new LengthNotEqualsConstraint("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""ABCDEF""}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""NotEquals"",""ExpectedLength"":4,""ActualLength"":6,""PropertyValue"":""ABCDEF"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to not be of length '4'.""}", c.GetState(j));
        }



        [TestMethod]
        public void LengthGreaterThanMissing()
        {
            LengthGreaterThanConstraint c = new LengthGreaterThanConstraint("TheProperty", 4);
            var j = GetJ(@"{}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""GreaterThan"",""MinimumLength"":4,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":false,""Description"":""The property 'TheProperty' was expected to be longer than '4' characters.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthGreaterThanNull()
        {
            LengthGreaterThanConstraint c = new LengthGreaterThanConstraint("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":null}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""GreaterThan"",""MinimumLength"":4,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be longer than '4' characters.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthGreaterThanEmpty()
        {
            LengthGreaterThanConstraint c = new LengthGreaterThanConstraint("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""GreaterThan"",""MinimumLength"":4,""ActualLength"":0,""PropertyValue"":"""",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be longer than '4' characters.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthGreaterThan44()
        {
            LengthGreaterThanConstraint c = new LengthGreaterThanConstraint("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""ABCD""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""GreaterThan"",""MinimumLength"":4,""ActualLength"":4,""PropertyValue"":""ABCD"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be longer than '4' characters.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthGreaterThan45()
        {
            LengthGreaterThanConstraint c = new LengthGreaterThanConstraint("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""ABCDE""}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""GreaterThan"",""MinimumLength"":4,""ActualLength"":5,""PropertyValue"":""ABCDE"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be longer than '4' characters.""}", c.GetState(j));
        }



        [TestMethod]
        public void LengthLessThanMissing()
        {
            LengthLessThanConstraint c = new LengthLessThanConstraint("TheProperty", 4);
            var j = GetJ(@"{}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThan"",""MinimumLength"":4,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":false,""Description"":""The property 'TheProperty' was expected to be less than '4' characters.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLessThanNull()
        {
            LengthLessThanConstraint c = new LengthLessThanConstraint("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":null}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThan"",""MinimumLength"":4,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '4' characters.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLessThanEmpty()
        {
            LengthLessThanConstraint c = new LengthLessThanConstraint("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""""}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThan"",""MinimumLength"":4,""ActualLength"":0,""PropertyValue"":"""",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '4' characters.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLessThan34()
        {
            LengthLessThanConstraint c = new LengthLessThanConstraint("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""ABC""}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThan"",""MinimumLength"":4,""ActualLength"":3,""PropertyValue"":""ABC"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '4' characters.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLessThan44()
        {
            LengthLessThanConstraint c = new LengthLessThanConstraint("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""ABCD""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThan"",""MinimumLength"":4,""ActualLength"":4,""PropertyValue"":""ABCD"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '4' characters.""}", c.GetState(j));
        }



        [TestMethod]
        public void LengthLtGtMissing()
        {
            LengthLessThanOrGreaterThanConstraint c = new LengthLessThanOrGreaterThanConstraint("TheProperty", 4, 8);
            var j = GetJ(@"{}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThanOrGreaterThan"",""MinimumLength"":4,""MaximumLength"":8,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":false,""Description"":""The property 'TheProperty' was expected to be less than '4' characters or greater than '8' characters in length.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLtGtMissing0()
        {
            LengthLessThanOrGreaterThanConstraint c = new LengthLessThanOrGreaterThanConstraint("TheProperty", 0, 8);
            var j = GetJ(@"{}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThanOrGreaterThan"",""MinimumLength"":0,""MaximumLength"":8,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":false,""Description"":""The property 'TheProperty' was expected to be less than '0' characters or greater than '8' characters in length.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLtGtNull()
        {
            LengthLessThanOrGreaterThanConstraint c = new LengthLessThanOrGreaterThanConstraint("TheProperty", 4, 8);
            var j = GetJ(@"{""TheProperty"":null}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThanOrGreaterThan"",""MinimumLength"":4,""MaximumLength"":8,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '4' characters or greater than '8' characters in length.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLtGtNull0()
        {
            LengthLessThanOrGreaterThanConstraint c = new LengthLessThanOrGreaterThanConstraint("TheProperty", 0, 8);
            var j = GetJ(@"{""TheProperty"":null}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThanOrGreaterThan"",""MinimumLength"":0,""MaximumLength"":8,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '0' characters or greater than '8' characters in length.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLtGtEmpty()
        {
            LengthLessThanOrGreaterThanConstraint c = new LengthLessThanOrGreaterThanConstraint("TheProperty", 4, 8);
            var j = GetJ(@"{""TheProperty"":""""}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThanOrGreaterThan"",""MinimumLength"":4,""MaximumLength"":8,""ActualLength"":0,""PropertyValue"":"""",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '4' characters or greater than '8' characters in length.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLtGtEmpty0()
        {
            LengthLessThanOrGreaterThanConstraint c = new LengthLessThanOrGreaterThanConstraint("TheProperty", 0, 8);
            var j = GetJ(@"{""TheProperty"":""""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThanOrGreaterThan"",""MinimumLength"":0,""MaximumLength"":8,""ActualLength"":0,""PropertyValue"":"""",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '0' characters or greater than '8' characters in length.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLtGtEmpty483()
        {
            LengthLessThanOrGreaterThanConstraint c = new LengthLessThanOrGreaterThanConstraint("TheProperty", 4, 8);
            var j = GetJ(@"{""TheProperty"":""ABC""}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThanOrGreaterThan"",""MinimumLength"":4,""MaximumLength"":8,""ActualLength"":3,""PropertyValue"":""ABC"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '4' characters or greater than '8' characters in length.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLtGtEmpty484()
        {
            LengthLessThanOrGreaterThanConstraint c = new LengthLessThanOrGreaterThanConstraint("TheProperty", 4, 8);
            var j = GetJ(@"{""TheProperty"":""ABCD""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThanOrGreaterThan"",""MinimumLength"":4,""MaximumLength"":8,""ActualLength"":4,""PropertyValue"":""ABCD"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '4' characters or greater than '8' characters in length.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLtGtEmpty488()
        {
            LengthLessThanOrGreaterThanConstraint c = new LengthLessThanOrGreaterThanConstraint("TheProperty", 4, 8);
            var j = GetJ(@"{""TheProperty"":""ABCDEFGH""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThanOrGreaterThan"",""MinimumLength"":4,""MaximumLength"":8,""ActualLength"":8,""PropertyValue"":""ABCDEFGH"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '4' characters or greater than '8' characters in length.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLtGtEmpty489()
        {
            LengthLessThanOrGreaterThanConstraint c = new LengthLessThanOrGreaterThanConstraint("TheProperty", 4, 8);
            var j = GetJ(@"{""TheProperty"":""ABCDEFGHI""}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThanOrGreaterThan"",""MinimumLength"":4,""MaximumLength"":8,""ActualLength"":9,""PropertyValue"":""ABCDEFGHI"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '4' characters or greater than '8' characters in length.""}", c.GetState(j));
        }


        protected JObject GetJ(string json)
        {
            JObject j = JsonConvert.DeserializeObject(json) as JObject;
            return j;
        }
    }
}
