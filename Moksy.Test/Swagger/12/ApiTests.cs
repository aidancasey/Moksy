using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Common.Swagger12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Test.Swagger._12
{
    [TestClass]
    public class ApiTests : TestBase 
    {
        public ApiTests()
        {
            Api = new Api();
        }

        protected Api Api;



        [TestMethod]
        public void ApiSanitySafe()
        {
            Assert.IsNotNull(Api.Path);
            Assert.AreEqual(0, Api.Operations.Length);

            Assert.IsNotNull(Api.Models);

            Api.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void ApiSanityNotSafe()
        {
            Api = new Common.Swagger12.Api(false);

            Assert.IsNotNull(Api.Path);
            Assert.IsNull(Api.Description);
            Assert.IsNull(Api.Operations);
            Assert.IsNull(Api.Models);

            Api.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void ApiPathNotRelativeEmpty()
        {
            Api.Path = "";

            Api.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void ApiPathNotRelativeNotRelative()
        {
            Api.Path = "hsdf://sdfkjsdkf/dsf.com";

            Api.Validate(Violations);
            AssertInvalidProperty("Path", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void ApiNotValidNested()
        {
            ApiDeclaration d = new ApiDeclaration();
            d.Apis = new Api[] { new Api(), new Api() { Path = null } };

            d.Validate(Violations);
            AssertInvalidProperty("Apis[1].Path", Common.Swagger.Common.ViolationLevel.Error);
        }



        [TestMethod]
        public void ApiOperationItemsReference()
        {
            var api = new Api();

            var op = new Operation();
            op.Type = "array";
            op.Items.Type = null;
            op.NickName = "doIt";
            op.Items.Reference = "noneExistentModel";
            api.Operations = new Operation[] { op };

            api.Validate(Violations);
            Assert.AreEqual(1, Violations.Count);

            Assert.AreEqual(@"Operations[""doIt""].Items.Reference", Violations[0].Context);
        }
    }
}
