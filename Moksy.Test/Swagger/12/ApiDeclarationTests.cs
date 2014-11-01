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
    /// ApiDeclaration tests. 
    /// </summary>
    [TestClass]
    public class ApiDeclarationTests : TestBase 
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public ApiDeclarationTests()
        {
            ApiDeclaration = new ApiDeclaration();
        }

        protected ApiDeclaration ApiDeclaration;



        [TestMethod]
        public void ApiDeclarationSanitySafe()
        {
            Assert.AreEqual("1.2", ApiDeclaration.SwaggerVersion);
            Assert.AreEqual("", ApiDeclaration.ApiVersion);
            Assert.IsFalse(string.IsNullOrEmpty(ApiDeclaration.BasePath));
            Assert.AreEqual("/", ApiDeclaration.ResourcePath);

            Assert.AreEqual(0, ApiDeclaration.Produces.Length);
            Assert.AreEqual(0, ApiDeclaration.Consumes.Length);
            Assert.AreEqual(0, ApiDeclaration.Apis.Length);

            ApiDeclaration.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void ApiDeclarationSanityNotSafe()
        {
            ApiDeclaration = new Common.Swagger12.ApiDeclaration(false);

            Assert.AreEqual("1.2", ApiDeclaration.SwaggerVersion);
            Assert.IsNull(ApiDeclaration.ApiVersion);
            Assert.IsFalse(string.IsNullOrEmpty(ApiDeclaration.BasePath));
            Assert.IsNull(ApiDeclaration.ResourcePath);
            Assert.IsNull(ApiDeclaration.Produces);
            Assert.IsNull(ApiDeclaration.Consumes);
            Assert.IsNull(ApiDeclaration.Apis);

            ApiDeclaration.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }



        #region SwaggerVersion 

        [TestMethod]
        public void ApiDeclarationSwaggerNotValid()
        {
            ApiDeclaration.SwaggerVersion = "sdfsdf";
            ApiDeclaration.Validate(Violations);
            AssertInvalidProperty("SwaggerVersion", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void ApiDeclarationSwaggerNull()
        {
            ApiDeclaration.SwaggerVersion = "sdfsdf";
            ApiDeclaration.Validate(Violations);
            AssertInvalidProperty("SwaggerVersion", Common.Swagger.Common.ViolationLevel.Error);
        }

        #endregion

        #region BasePath

        [TestMethod]
        public void ApiDeclarationBasePathNull()
        {
            ApiDeclaration.BasePath = null;
            ApiDeclaration.Validate(Violations);
            AssertInvalidProperty("BasePath", Common.Swagger.Common.ViolationLevel.Error);
        }

        #endregion
    }
}
