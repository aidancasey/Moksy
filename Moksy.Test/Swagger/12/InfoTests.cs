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
    /// Info tests. 
    /// </summary>
    [TestClass]
    public class InfoTests : TestBase
    {
        public InfoTests()
        {
            Info = new Info();
        }

        protected Info Info;



        [TestMethod]
        public void SanitySafe()
        {
            Assert.IsNotNull(Info.Title);
            Assert.IsNotNull(Info.Description);

            Assert.IsNotNull(Info.Contact);
            Assert.IsNotNull(Info.License);
            Assert.IsNotNull(Info.LicenseUrl);
            Assert.IsNotNull(Info.TermsOfServiceUrl);

            Info.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void SanityNotSafe()
        {
            Info = new Info(false);

            Assert.IsNotNull(Info.Title);
            Assert.IsNotNull(Info.Description);

            Assert.IsNull(Info.Contact);
            Assert.IsNull(Info.License);
            Assert.IsNull(Info.LicenseUrl);
            Assert.IsNull(Info.TermsOfServiceUrl);

            Info.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }



        [TestMethod]
        public void TitleIsNull()
        {
            Info.Title = null;

            Info.Validate(Violations);
            AssertInvalidProperty("Title", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void DescriptionIsNull()
        {
            Info.Title = null;

            Info.Validate(Violations);
            AssertInvalidProperty("Title", Common.Swagger.Common.ViolationLevel.Error);
        }



        [TestMethod]
        public void NestedInfo()
        {
            ResourceListing listing = new ResourceListing();
            listing.Info = new Info() { Title = null };

            listing.Validate(Violations);
            AssertInvalidProperty("Info.Title", Common.Swagger.Common.ViolationLevel.Error);
        }
    }
}
