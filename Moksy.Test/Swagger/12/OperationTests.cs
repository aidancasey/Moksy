using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Common.Swagger12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Test.Swagger._12
{
    /// <summary>
    /// Operation tests. 
    /// </summary>
    [TestClass]
    public class OperationTests : TestBase 
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public OperationTests()
        {
            Operation = new Operation();
        }

        protected Operation Operation;



        [TestMethod]
        public void OperationSanitySafe()
        {
            Assert.IsNotNull(Operation.Method);
            Assert.IsTrue(Operation.Method.Length > 0);
            Assert.AreEqual("", Operation.Summary);
            Assert.AreEqual("", Operation.Notes);
            Assert.IsTrue(Operation.NickName.Length > 0);
            Assert.IsNotNull(Operation.Produces);
            Assert.IsNotNull(Operation.Consumes);
            Assert.AreEqual("false", Operation.Deprecated);
            Assert.AreEqual(0, Operation.Parameters.Length);
            Assert.IsNotNull(Operation.Items);

            Operation.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void OperationSanityNotSafe()
        {
            Operation = new Common.Swagger12.Operation(false);

            Assert.IsNotNull(Operation.Method);
            Assert.IsTrue(Operation.Method.Length > 0);

            Assert.IsNull(Operation.Summary);
            Assert.IsNull(Operation.Notes);
            Assert.IsTrue(Operation.NickName.Length > 0);

            Assert.IsNull(Operation.Produces);
            Assert.IsNull(Operation.Consumes);
            Assert.IsNull(Operation.Deprecated);
            Assert.AreEqual(0, Operation.Parameters.Length);
            Assert.IsNull(Operation.Items);

            Operation.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }



        [TestMethod]
        public void OperationInvalidMethod()
        {
            Operation.Method = "NOTAMETHOD";

            Operation.Validate(Violations);
            AssertInvalidProperty("Method", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void OperationInvalidNull()
        {
            Operation.Method = null;

            Operation.Validate(Violations);
            AssertInvalidProperty("Method", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void OperationValidMethod()
        {
            Operation.Method = "OPTIONS";

            Operation.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void OperationCaseSensitive()
        {
            Operation.Method = "Post";

            Operation.Validate(Violations);
            AssertInvalidProperty("Method", Common.Swagger.Common.ViolationLevel.Error);
        }



        [TestMethod]
        public void NickNameNull()
        {
            Operation.NickName = null;

            Operation.Validate(Violations);
            AssertInvalidProperty("NickName", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void NickNameUnderscoresOnly()
        {
            Operation.NickName = "the_Nickname";

            Operation.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void NickNameWhitespaces()
        {
            Operation.NickName = "the Nickname";

            Operation.Validate(Violations);
            AssertInvalidProperty("NickName", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void NickNameNotAlphanumeric()
        {
            Operation.NickName = "the/Ni:ckname";

            Operation.Validate(Violations);
            AssertInvalidProperty("NickName", Common.Swagger.Common.ViolationLevel.Error);
        }



        [TestMethod]
        public void DeprecatedNull()
        {
            Operation.Deprecated = null;

            Operation.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void DeprecatedFalseCaseInsensitive()
        {
            Operation.Deprecated = "False";

            Operation.Validate(Violations);
            AssertInvalidProperty("Deprecated", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void DeprecatedTrueValid()
        {
            Operation.Deprecated = "true";

            Operation.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }



        [TestMethod]
        public void ParametersNull()
        {
            Operation.Parameters = null;

            Operation.Validate(Violations);
            AssertInvalidProperty("Parameters", Common.Swagger.Common.ViolationLevel.Error);
        }



        #region Return Type tests


        [TestMethod]
        public void IsVoid()
        {
            // How do we test for 'void'? I can't work it out from the specification. 
            Operation.Type = "void";

            Assert.IsTrue(Operation.IsVoid);
        }


        [TestMethod]
        public void TypeIsArrayValid()
        {
            Operation.Type = "array";

            Operation.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void TypeIsArrayValidButFormatNotNull()
        {
            Operation.Type = "array";
            Operation.Format = "notnull";

            Operation.Validate(Violations);
            AssertInvalidProperty("Type", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void TypeIntegerAndInt32Format()
        {
            Operation.Type = "integer";
            Operation.Format = "int32";

            Operation.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void TypeIntegerAndInt32FormatCaseSensitive()
        {
            Operation.Type = "Integer";
            Operation.Format = "notnaid";

            Operation.Validate(Violations);
            AssertInvalidProperty("Type", Common.Swagger.Common.ViolationLevel.Error);

        }

        [TestMethod]
        public void TypeIntegerAndInt32FormatMismatchFormat()
        {
            Operation.Type = "integer";
            Operation.Format = "int128";

            Operation.Validate(Violations);
            AssertInvalidProperty("Type", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void TypeIsProperty()
        {
            Operation.Type = "a";

            Operation.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }


        [TestMethod]
        public void Int32()
        {
            var op = new Operation() { Type = "integer", Format = "int32" };
            AssertType(op, true, false, false, false, false, false, false, false, false);
        }

        [TestMethod]
        public void Int64()
        {
            var op = new Operation() { Type = "integer", Format = "int64" };
            AssertType(op, false, true, false, false, false, false, false, false, false);
        }

        [TestMethod]
        public void Float()
        {
            var op = new Operation() { Type = "number", Format = "float" };
            AssertType(op, false, false, true, false, false, false, false, false, false);
        }

        [TestMethod]
        public void Double()
        {
            var op = new Operation() { Type = "number", Format = "double" };
            AssertType(op, false, false, false, true, false, false, false, false, false);
        }

        [TestMethod]
        public void String()
        {
            var op = new Operation() { Type = "string", Format = null };
            AssertType(op, false, false, false, false, true, false, false, false, false);
        }

        [TestMethod]
        public void Byte()
        {
            var op = new Operation() { Type = "string", Format = "byte" };
            AssertType(op, false, false, false, false, false, true, false, false, false);
        }

        [TestMethod]
        public void Boolean()
        {
            var op = new Operation() { Type = "boolean", Format = null };
            AssertType(op, false, false, false, false, false, false, true, false, false);
        }

        [TestMethod]
        public void Date()
        {
            var op = new Operation() { Type = "string", Format = "date" };
            AssertType(op, false, false, false, false, false, false, false, true, false);
        }

        [TestMethod]
        public void DateTime()
        {
            var op = new Operation() { Type = "string", Format = "date-time" };
            AssertType(op, false, false, false, false, false, false, false, false, true);
        }

        [TestMethod]
        public void Array()
        {
            var op = new Operation() { Type = "array", Format = null };
            Assert.IsTrue(op.IsArray);
        }

        [TestMethod]
        public void Model()
        {
            var op = new Operation() { Type = "theModelId", Format = null };
            Assert.IsTrue(op.IsModelReference);
        }

        [TestMethod]
        public void NotModel()
        {
            var op = new Operation() { Type = "theModelId", Format = "theFormat" };
            Assert.IsFalse(op.IsModelReference);
        }

        protected void AssertType(Operation p, bool isInt32, bool isInt64, bool isFloat, bool isDouble, bool isString, bool isByte, bool isBoolean, bool isDate, bool isDateTime)
        {
            Assert.AreEqual(p.IsInt32, isInt32);
            Assert.AreEqual(p.IsInt64, isInt64);
            Assert.AreEqual(p.IsFloat, isFloat);
            Assert.AreEqual(p.IsDouble, isDouble);
            Assert.AreEqual(p.IsString, isString);
            Assert.AreEqual(p.IsByte, isByte);
            Assert.AreEqual(p.IsBoolean, isBoolean);
            Assert.AreEqual(p.IsDate, isDate);
            Assert.AreEqual(p.IsDateTime, isDateTime);
        }

        #endregion

    }
}
