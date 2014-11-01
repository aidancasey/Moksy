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
    /// Parameter Tests.
    /// </summary>
    [TestClass]
    public class ParameterTests : TestBase
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public ParameterTests()
        {
            Parameter = new Parameter();
        }

        protected Parameter Parameter;



        [TestMethod]
        public void ParameterSanitySafe()
        {
            Assert.AreEqual("path", Parameter.ParamType);
            Assert.IsNotNull(Parameter.Name);
            Assert.AreEqual("", Parameter.Description);
            Assert.IsTrue(Parameter.Required);

            Parameter.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void ParameterSanityNotSafe()
        {
            Parameter = new Common.Swagger12.Parameter(false);

            Assert.AreEqual("path", Parameter.ParamType);
            Assert.IsNotNull(Parameter.Name);
            Assert.IsNull(Parameter.Description);
            Assert.IsTrue(Parameter.Required);

            Parameter.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }



        [TestMethod]
        public void ParameterParamTypeCaseSensitive()
        {
            Parameter.ParamType = "Path";

            Parameter.Validate(Violations);
            AssertInvalidProperty("ParamType", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void ParameterParamTypeNotValid()
        {
            Parameter.ParamType = "Disk";

            Parameter.Validate(Violations);
            AssertInvalidProperty("ParamType", Common.Swagger.Common.ViolationLevel.Error);
        }



        #region Parameter Validation

        [TestMethod]
        public void ParamTypePathApiNullIsValid()
        {
            Parameter p = new Parameter();
            p.ParamType = "path";
            p.Name = null;

            Assert.IsTrue(Moksy.Common.Swagger.Common.ValidationHelpers.IsParameterNameValid(null, p));
        }

        [TestMethod]
        public void ParamTypePathApiNotNulIsValid()
        {
            Api api = new Api();
            api.Path = "/";

            Parameter p = new Parameter();
            p.ParamType = "path";
            p.Name = "/";

            Assert.IsTrue(Moksy.Common.Swagger.Common.ValidationHelpers.IsParameterNameValid(api, p));
        }

        [TestMethod]
        public void ParamTypePathApiNotNulIsNotValid()
        {
            Api api = new Api();
            api.Path = "/";

            Parameter p = new Parameter();
            p.ParamType = "path";
            p.Name = "/Abc";

            Assert.IsFalse(Moksy.Common.Swagger.Common.ValidationHelpers.IsParameterNameValid(api, p));
        }



        [TestMethod]
        public void ParamTypeQueryEMptyName()
        {
            Parameter p = new Parameter() { ParamType = "query" };
            p.Name = "";

            Assert.IsFalse(Moksy.Common.Swagger.Common.ValidationHelpers.IsParameterNameValid(null, p));
        }

        [TestMethod]
        public void ParamTypeQueryName()
        {
            Parameter p = new Parameter() { ParamType = "query" };
            p.Name = "ABC";

            Assert.IsTrue(Moksy.Common.Swagger.Common.ValidationHelpers.IsParameterNameValid(null, p));
        }

        [TestMethod]
        public void ParamTypeBodyNameNotBody()
        {
            Parameter p = new Parameter() { ParamType = "body", Name = null };

            Assert.IsFalse(Moksy.Common.Swagger.Common.ValidationHelpers.IsParameterNameValid(null, p));
        }

        [TestMethod]
        public void ParamTypeBodyNameBody()
        {
            Parameter p = new Parameter() { ParamType = "body", Name = "body" };

            Assert.IsTrue(Moksy.Common.Swagger.Common.ValidationHelpers.IsParameterNameValid(null, p));
        }



        [TestMethod]
        public void ParamTypeFormNameEmpty()
        {
            Parameter p = new Parameter() { ParamType = "form", Name = "" };

            Assert.IsFalse(Moksy.Common.Swagger.Common.ValidationHelpers.IsParameterNameValid(null, p));
        }

        [TestMethod]
        public void ParamTypeFormNameNotEmpty()
        {
            Parameter p = new Parameter() { ParamType = "form", Name = "sdfdf" };

            Assert.IsTrue(Moksy.Common.Swagger.Common.ValidationHelpers.IsParameterNameValid(null, p));
        }



        [TestMethod]
        public void ParamTypeHeaderNameEmpty()
        {
            Parameter p = new Parameter() { ParamType = "header", Name = "" };

            Assert.IsFalse(Moksy.Common.Swagger.Common.ValidationHelpers.IsParameterNameValid(null, p));
        }

        [TestMethod]
        public void ParamTypeHeaderNameNotEmpty()
        {
            Parameter p = new Parameter() { ParamType = "form", Name = "abcde" };

            Assert.IsTrue(Moksy.Common.Swagger.Common.ValidationHelpers.IsParameterNameValid(null, p));
        }

        #endregion

        [TestMethod]
        public void PathRequired()
        {
            Parameter p = new Parameter() { ParamType = "path", Required = true };

            p.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void PathNotRequired()
        {
            Parameter p = new Parameter() { ParamType = "path", Required = false };

            p.Validate(Violations);
            AssertInvalidProperty("Required", Common.Swagger.Common.ViolationLevel.Error);
        }
    }
}
