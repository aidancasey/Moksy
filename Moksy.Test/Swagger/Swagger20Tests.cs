using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moksy.Common.Swagger20;
using Moksy.Common.Swagger.Common;

namespace Moksy.Test.Swagger
{
    [TestClass]
    public class Swagger20Tests
    {
        public Swagger20Tests()
        {
        }

        [TestInitialize]
        public void Setup()
        {
            Swagger = new Moksy.Common.Swagger20.Swagger();
        }

        protected Moksy.Common.Swagger20.Swagger Swagger;



        #region Swagger Model

        [TestMethod]
        public void SwaggerSanity()
        {
            Assert.AreEqual("2.0", Swagger.Version);
            Assert.AreEqual("localhost", Swagger.Host);
            Assert.AreEqual("/", Swagger.BasePath);

            Assert.IsNotNull(Swagger.Schemes);
            Assert.IsNotNull(Swagger.Consumes);
            Assert.IsNotNull(Swagger.Produces);
            Assert.IsNotNull(Swagger.ExternalDocs);

            Assert.IsNotNull(Swagger.Info);
            Assert.IsNotNull(Swagger.Info.Title);
            Assert.IsNotNull(Swagger.Info.Description);
            Assert.IsNotNull(Swagger.Info.TermsOfService);
            Assert.IsNotNull(Swagger.Info.Version);

            Assert.IsNotNull(Swagger.Contact);
            Assert.IsNotNull(Swagger.Contact.Email);
            Assert.IsNotNull(Swagger.Contact.Name);
            Assert.IsNotNull(Swagger.Contact.Url);

            Assert.IsNotNull(Swagger.License);
            Assert.IsNotNull(Swagger.License.Name);

            Assert.IsNotNull(Swagger.Tags);

            var result = SwaggerValidator.Validate(Swagger);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void SwaggerSanityNotSafe()
        {
            Swagger = new Moksy.Common.Swagger20.Swagger(false);

            Assert.AreEqual("2.0", Swagger.Version);
            Assert.IsNull(Swagger.Host);
            Assert.IsNull(Swagger.BasePath);

            Assert.IsNull(Swagger.Schemes);
            Assert.IsNull(Swagger.Consumes);
            Assert.IsNull(Swagger.Produces);
            Assert.IsNull(Swagger.ExternalDocs);
            
            Assert.IsNotNull(Swagger.Info);
            Assert.IsNotNull(Swagger.Info.Title);
            Assert.IsNotNull(Swagger.Info.Version);

            Assert.IsNull(Swagger.Info.Description);
            Assert.IsNull(Swagger.Info.TermsOfService);

            Assert.IsNull(Swagger.Contact);
            Assert.IsNull(Swagger.License);
            Assert.IsNull(Swagger.Tags);

            var result = SwaggerValidator.Validate(Swagger);
            var errors = result.Where(f => f.ViolationLevel == ViolationLevel.Error);
            Assert.AreEqual(0, errors.Count());
        }

        [TestMethod]
        public void VersionNull()
        {
            Swagger.Version = null;

            AssertInvalidProperty("Swagger", ViolationLevel.Error);
        }

        [TestMethod]
        public void Version12()
        {
            Swagger.Version = "1.2";

            AssertInvalidProperty("Swagger", ViolationLevel.Error);
        }

        [TestMethod]
        public void BasePathNull()
        {
            Swagger.BasePath = null;

            AssertInvalidProperty("BasePath", ViolationLevel.Informational);
        }

        [TestMethod]
        public void HostNull()
        {
            Swagger.Host = null;

            AssertInvalidProperty("Host", ViolationLevel.Informational);        
        }

        [TestMethod]
        public void HostEmpty()
        {
            Swagger.Host = "";

            AssertInvalidProperty("Host", ViolationLevel.Informational);
        }

        [TestMethod]
        public void SchemesNull()
        {
            Swagger.Schemes = null;

            AssertInvalidProperty("Schemes", ViolationLevel.Informational);
        }

        [TestMethod]
        public void SchemesEmpty()
        {
            Swagger.Schemes = new string[0];

            AssertInvalidProperty("Schemes", ViolationLevel.Informational);
        }

        [TestMethod]
        public void SchemesNotValid()
        {
            Swagger.Schemes = new string[3] { "https", "file", "ws" };

            AssertInvalidProperty("Schemes", ViolationLevel.Informational);
        }

        [TestMethod]
        public void SchemesValidSingle()
        {
            Swagger.Schemes = new string[1] { "https" };

            var result = SwaggerValidator.Validate(Swagger);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void SchemesValidAll4()
        {
            Swagger.Schemes = new string[4] { "https", "http", "ws", "wss" };

            var result = SwaggerValidator.Validate(Swagger);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ExternalDocsNull()
        {
            Swagger.ExternalDocs = null;

            var result = SwaggerValidator.Validate(Swagger);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ExternalDocsDescriptionNull()
        {
            Swagger.ExternalDocs.Description = null;

            AssertInvalidProperty("ExternalDocs.Description", ViolationLevel.Informational);
        }

        [TestMethod]
        public void ExternalDocsUrlNull()
        {
            Swagger.ExternalDocs.Url = null;

            AssertInvalidProperty("ExternalDocs.Url", ViolationLevel.Error);
        }

        [TestMethod]
        public void ExternalDocsUrlEmpty()
        {
            Swagger.ExternalDocs.Url = "";

            AssertInvalidProperty("ExternalDocs.Url", ViolationLevel.Error);
        }

        [TestMethod]
        public void ExternalDocsUrlInvalidUrl()
        {
            Swagger.ExternalDocs.Url = "NotAUrl";

            AssertInvalidProperty("ExternalDocs.Url", ViolationLevel.Error);
        }

        [TestMethod]
        public void ExternalDocsUrlValid()
        {
            Swagger.ExternalDocs.Url = "http://somewhere.com/docs.html";

            var result = SwaggerValidator.Validate(Swagger);
            Assert.AreEqual(0, result.Count);
        }



        [TestMethod]
        public void InfoMissing()
        {
            Swagger.Info = null;

            AssertInvalidProperty("Info", ViolationLevel.Error);
        }

        [TestMethod]
        public void InfoTitleNull()
        {
            Swagger.Info.Title = null;

            AssertInvalidProperty("Info.Title", ViolationLevel.Error);
        }

        [TestMethod]
        public void InfoTitleEmpty()
        {
            Swagger.Info.Title = "";

            var result = SwaggerValidator.Validate(Swagger);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void InfoVersionNull()
        {
            Swagger.Info.Version = null;

            AssertInvalidProperty("Info.Version", ViolationLevel.Error);
        }

        [TestMethod]
        public void InfoVersionEmpty()
        {
            Swagger.Info.Version = "";

            var result = SwaggerValidator.Validate(Swagger);
            Assert.AreEqual(0, result.Count);
        }



        [TestMethod]
        public void ContactUrlNull()
        {
            Swagger.Contact.Url = null;

            var result = SwaggerValidator.Validate(Swagger);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ContactUrlEmpty()
        {
            Swagger.Contact.Url = "";

            AssertInvalidProperty("Contact.Url", ViolationLevel.Error);
        }

        [TestMethod]
        public void ContactUrlInvalidUrl()
        {
            Swagger.Contact.Url = "http://wdsfkjs.@allkjd.\\sdsd/.com";

            AssertInvalidProperty("Contact.Url", ViolationLevel.Error);
        }

        [TestMethod]
        public void ContactUrlValidUrl()
        {
            Swagger.Contact.Url = "http://localhost.com/contact.html";

            var result = SwaggerValidator.Validate(Swagger);
            Assert.AreEqual(0, result.Count);
        }



        [TestMethod]
        public void LicenseNull()
        {
            Swagger.License = null;

            var result = SwaggerValidator.Validate(Swagger);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void LicenseNameNull()
        {
            Swagger.License.Name = null;

            AssertInvalidProperty("License.Name", ViolationLevel.Error);
        }

        [TestMethod]
        public void LicenseNameEmpty()
        {
            Swagger.License.Name = "";

            var result = SwaggerValidator.Validate(Swagger);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void LicenseUriNull()
        {
            Swagger.License.Url = null;

            var result = SwaggerValidator.Validate(Swagger);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void LicenseUrlEmpty()
        {
            Swagger.License.Url = "";

            AssertInvalidProperty("License.Url", ViolationLevel.Error);
        }

        [TestMethod]
        public void LicenseUrlInvalid()
        {
            Swagger.License.Url = "hsfd://sdfkljsdflkj\\\\sdf@sdfjhdsf.com";

            AssertInvalidProperty("License.Url", ViolationLevel.Error);
        }

        [TestMethod]
        public void LicenseUrlValid()
        {
            Swagger.License.Url = "http://localhost/license.html";

            var result = SwaggerValidator.Validate(Swagger);
            Assert.AreEqual(0, result.Count);
        }

        #endregion

        #region Tag

        [TestMethod]
        public void TagObjectSafe()
        {
            TagObject tag = new TagObject();
            Assert.IsNotNull(tag.Name);
            Assert.IsNotNull(tag.Description);
            Assert.IsNotNull(tag.ExternalDocs);
        }

        [TestMethod]
        public void TagObjectNotSafe()
        {
            TagObject tag = new TagObject(false);
            Assert.IsNull(tag.Name);
            Assert.IsNull(tag.Description);
            Assert.IsNull(tag.ExternalDocs);
        }

        [TestMethod]
        public void TagValid()
        {
            TagObject tag = new TagObject();
            Swagger.Tags = new TagObject[] { tag };

            var result = SwaggerValidator.Validate(Swagger);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void TagNameInvalid()
        {
            TagObject safeTag = new TagObject();
            TagObject nameTag = new TagObject() { Name = null };
            Swagger.Tags = new TagObject[] { safeTag, nameTag };

            AssertInvalidProperty("Tags[1].Name", ViolationLevel.Error);
        }

        [TestMethod]
        public void TagExternalDocsValid()
        {
            TagObject safeTag = new TagObject();
            TagObject docs = new TagObject() { Name = "a", ExternalDocs = new ExternalDocumentationObject() { Url = null } };
            Swagger.Tags = new TagObject[] { safeTag, docs };

            AssertInvalidProperty("Tags[1].ExternalDocs.Url", ViolationLevel.Error);
        }

        [TestMethod]
        public void TagNameClashCaseInsensitive()
        {
            TagObject filler = new TagObject();
            TagObject safeTag = new TagObject() { Name = "Dog" };
            TagObject clashTag = new TagObject() { Name = "DOG" };
            Swagger.Tags = new TagObject[] { filler, safeTag, clashTag };

            AssertInvalidProperty("Tags[2].Name", ViolationLevel.Error);
        }

        #endregion

        protected void AssertInvalidProperty(string s, ViolationLevel level)
        {
            var result = SwaggerValidator.Validate(Swagger);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(s, result[0].Code);
            Assert.AreEqual(level, result[0].ViolationLevel);
        }
    }
}
