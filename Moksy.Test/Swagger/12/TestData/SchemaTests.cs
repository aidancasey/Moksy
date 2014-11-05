using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Test.Swagger._12.TestData
{
    /// <summary>
    /// High level 
    /// </summary>
    [TestClass]
    [DeploymentItem("Swagger\\12\\TestData", "TestData")]
    public class SchemaTests : TestBase
    {
        public SchemaTests()
        {
        }



        /// <summary>
        /// Sanity test for the Pet resource listing (v1.2 Swagger specificaiton at http://petstore.swagger.wordnik.com/api/api-docs
        /// </summary>
        [TestMethod]
        public void ResourceListingSanity()
        {
            var path = System.IO.Path.Combine(TestContext.DeploymentDirectory, "TestData", "ResourceListing.json");
            var content = System.IO.File.ReadAllText(path);

            var resourceListing = JsonConvert.DeserializeObject<global::Moksy.Common.Swagger12.ResourceListing>(content);

            AssertResourceListing(resourceListing);

            // If we get to here, it means that the original Json has been serialized in the form we expect. Now roundtrip. 
            var json = JsonConvert.SerializeObject(resourceListing);
            resourceListing = JsonConvert.DeserializeObject<global::Moksy.Common.Swagger12.ResourceListing>(json);
            AssertResourceListing(resourceListing);
        }

        protected void AssertResourceListing(global::Moksy.Common.Swagger12.ResourceListing resourceListing)
        {
            Assert.AreEqual("1.0.0", resourceListing.ApiVersion);
            Assert.AreEqual("1.2", resourceListing.SwaggerVersion);

            Assert.AreEqual(3, resourceListing.Apis.Length);
            Assert.AreEqual("/pet", resourceListing.Apis[0].Path);
            Assert.IsTrue(resourceListing.Apis[0].Description.Contains("Operations about pets"));
            Assert.AreEqual("/user", resourceListing.Apis[1].Path);
            Assert.IsTrue(resourceListing.Apis[1].Description.Contains("Operations about user"));
            Assert.AreEqual("/store", resourceListing.Apis[2].Path);
            Assert.IsTrue(resourceListing.Apis[2].Description.Contains("Operations about store"));

            Assert.IsTrue(resourceListing.Info.Title.Contains("Swagger Sample App"));
            Assert.IsTrue(resourceListing.Info.Description.Contains("You can find out more about"));
            Assert.AreEqual("http://helloreverb.com/terms/", resourceListing.Info.TermsOfServiceUrl);
            Assert.AreEqual("apiteam@wordnik.com", resourceListing.Info.Contact);
            Assert.AreEqual("Apache 2.0", resourceListing.Info.License);
            Assert.AreEqual("http://www.apache.org/licenses/LICENSE-2.0.html", resourceListing.Info.LicenseUrl);
        }

        [TestMethod]
        public void ApiDeclarationPet()
        {
            var path = System.IO.Path.Combine(TestContext.DeploymentDirectory, "TestData", "Pet.json");
            var content = System.IO.File.ReadAllText(path);

            var apiDeclaration = JsonConvert.DeserializeObject<global::Moksy.Common.Swagger12.ApiDeclaration>(content);

            Assert.Inconclusive("UPTO: Finish this off. Do a sanity test. ");
        }
    }
}
