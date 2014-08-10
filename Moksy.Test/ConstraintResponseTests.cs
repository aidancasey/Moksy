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
            IsMissing c = new IsMissing("TheProperty");
            var j = GetJ(@"{}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""IsMissing"",""PropertyName"":""TheProperty"",""PropertyValue"":null,""PropertyHasValue"":false,""Description"":""The property 'TheProperty' was not expected to be in the Json. ""}", c.GetState(j));
        }

        [TestMethod]
        public void IsMissingNull()
        {
            IsMissing c = new IsMissing("TheProperty");
            var j = GetJ(@"{""TheProperty"":null}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""IsMissing"",""PropertyName"":""TheProperty"",""PropertyValue"":null,""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was not expected to be in the Json. ""}", c.GetState(j));
        }

        [TestMethod]
        public void IsMissingEmptyString()
        {
            IsMissing c = new IsMissing("TheProperty");
            var j = GetJ(@"{""TheProperty"":""""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""IsMissing"",""PropertyName"":""TheProperty"",""PropertyValue"":"""",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was not expected to be in the Json. ""}", c.GetState(j));
        }

        [TestMethod]
        public void IsMissingValueString()
        {
            IsMissing c = new IsMissing("TheProperty");
            var j = GetJ(@"{""TheProperty"":""TheValue""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""IsMissing"",""PropertyName"":""TheProperty"",""PropertyValue"":""TheValue"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was not expected to be in the Json. ""}", c.GetState(j));
        }




        [TestMethod]
        public void IsNullFalse()
        {
            IsNull c = new IsNull("TheProperty");
            var j = GetJ(@"{}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""IsNull"",""PropertyName"":""TheProperty"",""PropertyValue"":null,""PropertyHasValue"":false,""Description"":""The property 'TheProperty' was expected to be Null.""}", c.GetState(j));
        }

        [TestMethod]
        public void IsNullNull()
        {
            IsNull c = new IsNull("TheProperty");
            var j = GetJ(@"{""TheProperty"":null}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""IsNull"",""PropertyName"":""TheProperty"",""PropertyValue"":null,""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be Null.""}", c.GetState(j));
        }

        [TestMethod]
        public void IsNullEmptyString()
        {
            IsNull c = new IsNull("TheProperty");
            var j = GetJ(@"{""TheProperty"":""""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""IsNull"",""PropertyName"":""TheProperty"",""PropertyValue"":"""",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be Null.""}", c.GetState(j));
        }

        [TestMethod]
        public void IsNullValueString()
        {
            IsNull c = new IsNull("TheProperty");
            var j = GetJ(@"{""TheProperty"":""TheValue""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""IsNull"",""PropertyName"":""TheProperty"",""PropertyValue"":""TheValue"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be Null.""}", c.GetState(j));
        }




        [TestMethod]
        public void LengthEqualsMissing()
        {
            LengthEquals c = new LengthEquals("TheProperty", 4);
            var j = GetJ(@"{}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""Equals"",""ExpectedLength"":4,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":false,""Description"":""The property 'TheProperty' was expected to be of length '4'.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthEqualsNull()
        {
            LengthEquals c = new LengthEquals("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":null}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""Equals"",""ExpectedLength"":4,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be of length '4'.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthEqualsEmpty()
        {
            LengthEquals c = new LengthEquals("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""Equals"",""ExpectedLength"":4,""ActualLength"":0,""PropertyValue"":"""",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be of length '4'.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthEqualsTrue()
        {
            LengthEquals c = new LengthEquals("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""ABCD""}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""Equals"",""ExpectedLength"":4,""ActualLength"":4,""PropertyValue"":""ABCD"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be of length '4'.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthEqualsFalse()
        {
            LengthEquals c = new LengthEquals("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""ABCDEF""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""Equals"",""ExpectedLength"":4,""ActualLength"":6,""PropertyValue"":""ABCDEF"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be of length '4'.""}", c.GetState(j));
        }



        [TestMethod]
        public void LengthNotEqualsMissing()
        {
            LengthNotEquals c = new LengthNotEquals("TheProperty", 4);
            var j = GetJ(@"{}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""NotEquals"",""ExpectedLength"":4,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":false,""Description"":""The property 'TheProperty' was expected to not be of length '4'.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthNotEqualsNull()
        {
            LengthNotEquals c = new LengthNotEquals("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":null}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""NotEquals"",""ExpectedLength"":4,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to not be of length '4'.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthNotEqualsEmpty()
        {
            LengthLessThanOrGreaterThan c = new LengthNotEquals("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""""}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""NotEquals"",""ExpectedLength"":4,""ActualLength"":0,""PropertyValue"":"""",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to not be of length '4'.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthNotEqualsTrue()
        {
            LengthLessThanOrGreaterThan c = new LengthNotEquals("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""ABCD""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""NotEquals"",""ExpectedLength"":4,""ActualLength"":4,""PropertyValue"":""ABCD"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to not be of length '4'.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthNotEqualsFalse()
        {
            LengthLessThanOrGreaterThan c = new LengthNotEquals("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""ABCDEF""}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""NotEquals"",""ExpectedLength"":4,""ActualLength"":6,""PropertyValue"":""ABCDEF"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to not be of length '4'.""}", c.GetState(j));
        }



        [TestMethod]
        public void LengthGreaterThanMissing()
        {
            LengthGreaterThan c = new LengthGreaterThan("TheProperty", 4);
            var j = GetJ(@"{}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""GreaterThan"",""MinimumLength"":4,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":false,""Description"":""The property 'TheProperty' was expected to be longer than '4' characters.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthGreaterThanNull()
        {
            LengthGreaterThan c = new LengthGreaterThan("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":null}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""GreaterThan"",""MinimumLength"":4,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be longer than '4' characters.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthGreaterThanEmpty()
        {
            LengthGreaterThan c = new LengthGreaterThan("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""GreaterThan"",""MinimumLength"":4,""ActualLength"":0,""PropertyValue"":"""",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be longer than '4' characters.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthGreaterThan44()
        {
            LengthGreaterThan c = new LengthGreaterThan("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""ABCD""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""GreaterThan"",""MinimumLength"":4,""ActualLength"":4,""PropertyValue"":""ABCD"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be longer than '4' characters.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthGreaterThan45()
        {
            LengthGreaterThan c = new LengthGreaterThan("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""ABCDE""}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""GreaterThan"",""MinimumLength"":4,""ActualLength"":5,""PropertyValue"":""ABCDE"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be longer than '4' characters.""}", c.GetState(j));
        }



        [TestMethod]
        public void LengthLessThanMissing()
        {
            LengthLessThan c = new LengthLessThan("TheProperty", 4);
            var j = GetJ(@"{}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThan"",""MinimumLength"":4,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":false,""Description"":""The property 'TheProperty' was expected to be less than '4' characters.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLessThanNull()
        {
            LengthLessThan c = new LengthLessThan("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":null}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThan"",""MinimumLength"":4,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '4' characters.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLessThanEmpty()
        {
            LengthLessThan c = new LengthLessThan("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""""}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThan"",""MinimumLength"":4,""ActualLength"":0,""PropertyValue"":"""",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '4' characters.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLessThan34()
        {
            LengthLessThan c = new LengthLessThan("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""ABC""}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThan"",""MinimumLength"":4,""ActualLength"":3,""PropertyValue"":""ABC"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '4' characters.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLessThan44()
        {
            LengthLessThan c = new LengthLessThan("TheProperty", 4);
            var j = GetJ(@"{""TheProperty"":""ABCD""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThan"",""MinimumLength"":4,""ActualLength"":4,""PropertyValue"":""ABCD"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '4' characters.""}", c.GetState(j));
        }



        [TestMethod]
        public void LengthLtGtMissing()
        {
            LengthLessThanOrGreaterThan c = new LengthLessThanOrGreaterThan("TheProperty", 4, 8);
            var j = GetJ(@"{}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThanOrGreaterThan"",""MinimumLength"":4,""MaximumLength"":8,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":false,""Description"":""The property 'TheProperty' was expected to be less than '4' characters or greater than '8' characters in length.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLtGtMissing0()
        {
            LengthLessThanOrGreaterThan c = new LengthLessThanOrGreaterThan("TheProperty", 0, 8);
            var j = GetJ(@"{}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThanOrGreaterThan"",""MinimumLength"":0,""MaximumLength"":8,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":false,""Description"":""The property 'TheProperty' was expected to be less than '0' characters or greater than '8' characters in length.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLtGtNull()
        {
            LengthLessThanOrGreaterThan c = new LengthLessThanOrGreaterThan("TheProperty", 4, 8);
            var j = GetJ(@"{""TheProperty"":null}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThanOrGreaterThan"",""MinimumLength"":4,""MaximumLength"":8,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '4' characters or greater than '8' characters in length.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLtGtNull0()
        {
            LengthLessThanOrGreaterThan c = new LengthLessThanOrGreaterThan("TheProperty", 0, 8);
            var j = GetJ(@"{""TheProperty"":null}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThanOrGreaterThan"",""MinimumLength"":0,""MaximumLength"":8,""ActualLength"":0,""PropertyValue"":null,""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '0' characters or greater than '8' characters in length.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLtGtEmpty()
        {
            LengthLessThanOrGreaterThan c = new LengthLessThanOrGreaterThan("TheProperty", 4, 8);
            var j = GetJ(@"{""TheProperty"":""""}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThanOrGreaterThan"",""MinimumLength"":4,""MaximumLength"":8,""ActualLength"":0,""PropertyValue"":"""",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '4' characters or greater than '8' characters in length.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLtGtEmpty0()
        {
            LengthLessThanOrGreaterThan c = new LengthLessThanOrGreaterThan("TheProperty", 0, 8);
            var j = GetJ(@"{""TheProperty"":""""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThanOrGreaterThan"",""MinimumLength"":0,""MaximumLength"":8,""ActualLength"":0,""PropertyValue"":"""",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '0' characters or greater than '8' characters in length.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLtGtEmpty483()
        {
            LengthLessThanOrGreaterThan c = new LengthLessThanOrGreaterThan("TheProperty", 4, 8);
            var j = GetJ(@"{""TheProperty"":""ABC""}");
            Assert.IsTrue(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThanOrGreaterThan"",""MinimumLength"":4,""MaximumLength"":8,""ActualLength"":3,""PropertyValue"":""ABC"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '4' characters or greater than '8' characters in length.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLtGtEmpty484()
        {
            LengthLessThanOrGreaterThan c = new LengthLessThanOrGreaterThan("TheProperty", 4, 8);
            var j = GetJ(@"{""TheProperty"":""ABCD""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThanOrGreaterThan"",""MinimumLength"":4,""MaximumLength"":8,""ActualLength"":4,""PropertyValue"":""ABCD"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '4' characters or greater than '8' characters in length.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLtGtEmpty488()
        {
            LengthLessThanOrGreaterThan c = new LengthLessThanOrGreaterThan("TheProperty", 4, 8);
            var j = GetJ(@"{""TheProperty"":""ABCDEFGH""}");
            Assert.IsFalse(c.Evaluate(j));
            Assert.AreEqual(@"{""Name"":""Length"",""PropertyName"":""TheProperty"",""Kind"":""LessThanOrGreaterThan"",""MinimumLength"":4,""MaximumLength"":8,""ActualLength"":8,""PropertyValue"":""ABCDEFGH"",""PropertyHasValue"":true,""Description"":""The property 'TheProperty' was expected to be less than '4' characters or greater than '8' characters in length.""}", c.GetState(j));
        }

        [TestMethod]
        public void LengthLtGtEmpty489()
        {
            LengthLessThanOrGreaterThan c = new LengthLessThanOrGreaterThan("TheProperty", 4, 8);
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
