using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moksy.Common.Swagger12;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Moksy.Test.Swagger._12
{
    [TestClass]
    public class ResourceTests : TestBase 
    {
        public ResourceTests()
        {
            Resource = new Resource();
        }

        protected Resource Resource;

        [TestMethod]
        public void SanitySafe()
        {
            Assert.IsNotNull(Resource.Path);
            Assert.AreNotEqual("", Resource.Path);

            Assert.IsNotNull(Resource.Description);

            Resource.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void SanityNotSafe()
        {
            Resource = new Common.Swagger12.Resource(false);

            Assert.IsNotNull(Resource.Path);
            Assert.AreNotEqual("", Resource.Path);

            Assert.IsNull(Resource.Description);

            Resource.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }



        [TestMethod]
        public void PathNull()
        {
            Resource.Path = null;

            Resource.Validate(Violations);
            AssertInvalidProperty("Path", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void PathEmpty()
        {
            Resource.Path = "";

            Resource.Validate(Violations);
            AssertInvalidProperty("Path", Common.Swagger.Common.ViolationLevel.Error);
        }



        [TestMethod]
        public void NestedResource()
        {
            ResourceListing listing = new ResourceListing();
            listing.Apis = new Resource[] { new Resource(), new Resource() { Path = null }, new Resource() };

            listing.Validate(Violations);
            AssertInvalidProperty("Apis[1].Path", Common.Swagger.Common.ViolationLevel.Error);
        }
    }
}
