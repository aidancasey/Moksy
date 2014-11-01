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
    }
}
