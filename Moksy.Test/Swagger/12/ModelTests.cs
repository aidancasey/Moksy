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
    /// Model tests. 
    /// </summary>
    [TestClass]
    public class ModelTests : TestBase
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public ModelTests()
        {
            Model = new Model();
        }

        protected Model Model;



        [TestMethod]
        public void ModelSanitySafe()
        {
            Assert.AreEqual("", Model.Description);
            Assert.IsNotNull(Model.Properties);
            Assert.AreEqual(0, Model.SubTypes.Length);
            Assert.AreEqual(0, Model.Required.Length);

            Model.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void ModelSanityNotSafe()
        {
            Model = new Common.Swagger12.Model(false);

            Assert.IsNull(Model.Description);
            Assert.IsNotNull(Model.Properties);
            Assert.IsNull(Model.SubTypes);
            Assert.IsNull(Model.Required);

            Model.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);            
        }



        [TestMethod]
        public void IdNull()
        {
            Model.Id = null;

            Model.Validate(Violations);
            AssertInvalidProperty("Id", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void IsEmpty()
        {
            Model.Id = "";

            Model.Validate(Violations);
            AssertInvalidProperty("Id", Common.Swagger.Common.ViolationLevel.Error);
        }



        [TestMethod]
        public void RequiredExists()
        {
            Model.Required = new string[] { "theModel" };
            Model.Properties.Data["theModel"] = new Model() { Id = "theModel" };

            Model.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void RequiredDoesNotExist()
        {
            Model.Required = new string[] { "theModel2" };
            Model.Properties.Data["theModel"] = new Model() { Id = "theModel" };

            Model.Validate(Violations);
            AssertInvalidProperty("Required", @"Required[""theModel2""]", Common.Swagger.Common.ViolationLevel.Error);
        }
    }
}
