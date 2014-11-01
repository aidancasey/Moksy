using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Common.Swagger.Common;
using Moksy.Common.Swagger12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Test.Swagger._12
{
    [TestClass]
    public class ResourceListingTests : TestBase
    {
        public ResourceListingTests()
        {
            ResourceListing = new ResourceListing();
        }

        protected ResourceListing ResourceListing;


        [TestMethod]
        public void ResourceListingSanitySafe()
        {
            Assert.IsNotNull(ResourceListing.Apis);
            Assert.IsNotNull(ResourceListing.ApiVersion);
            Assert.IsNotNull(ResourceListing.Info);
            Assert.IsNotNull(ResourceListing.SwaggerVersion);

            ResourceListing.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void ResourceListingSanityNotSafe()
        {
            ResourceListing = new ResourceListing(false);

            Assert.IsNotNull(ResourceListing.Apis);
            Assert.IsNotNull(ResourceListing.SwaggerVersion);

            Assert.IsNull(ResourceListing.ApiVersion);
            Assert.IsNull(ResourceListing.Info);

            ResourceListing.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        

        [TestMethod]
        public void SwaggerVersionNull()
        {
            ResourceListing.SwaggerVersion = null;

            ResourceListing.Validate(Violations);
            AssertInvalidProperty("SwaggerVersion", ViolationLevel.Error);
        }

        [TestMethod]
        public void SwaggerVersionEmpty()
        {
            ResourceListing.SwaggerVersion = null;

            ResourceListing.Validate(Violations);
            AssertInvalidProperty("SwaggerVersion", ViolationLevel.Error);
        }



        [TestMethod]
        public void ApisNull()
        {
            ResourceListing.Apis = null;

            ResourceListing.Validate(Violations);
            AssertInvalidProperty("Apis", ViolationLevel.Error);
        }
    }
}
