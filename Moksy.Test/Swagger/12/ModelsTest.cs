using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Common.Swagger12;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Test.Swagger._12
{
    [TestClass]
    public class ModelsTest : TestBase 
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public ModelsTest()
        {
            Models = new Models();
        }

        protected Models Models;



        [TestMethod]
        public void ModelsSerializationEmpty()
        {
            var json = JsonConvert.SerializeObject(Models);
            Assert.AreEqual("{}", json);
        }

        [TestMethod]
        public void ModelsSerializationOneElement()
        {
            Models.Data["a"] = new Model() { Id = "a" };

            var json = JsonConvert.SerializeObject(Models);
            Assert.AreEqual(@"{""a"":{""id"":""a"",""description"":"""",""properties"":{},""subTypes"":[],""required"":[]}}", json);

            var constructed = JsonConvert.DeserializeObject<Models>(json);
            Assert.AreEqual(1, constructed.Data.Count);
            Assert.AreEqual(@"{""a"":{""id"":""a"",""description"":"""",""properties"":{},""subTypes"":[],""required"":[]}}", JsonConvert.SerializeObject(constructed));
        }



        [TestMethod]
        public void ModelIdMismatched()
        {
            Models.Data["a"] = new Model() { Id = "b" };

            Models.Validate(Violations);
            AssertInvalidProperty("Id", @"[""a""].Id", Common.Swagger.Common.ViolationLevel.Error);            
        }

        [TestMethod]
        public void ModelIdMismatchedCaseSensitive()
        {
            Models.Data["a"] = new Model() { Id = "A" };

            Models.Validate(Violations);
            AssertInvalidProperty("Id", @"[""a""].Id", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void ModelIdMatched()
        {
            Models.Data["a"] = new Model() { Id = "a" };

            Models.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }



        [TestMethod]
        public void ModelPropertiesIsNull()
        {
            Models models = new Models();
            Model model = new Model() { Id = "theModel" };
            model.Properties = null;

            models.Data["theModel"] = model;

            models.Validate(Violations);
            Assert.AreEqual(1, Violations.Count);
            AssertInvalidProperty("Properties", @"[""theModel""].Properties", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void ModelPropertyIsInvalid()
        {
            Models models = new Models();
            Model model = new Model() { Id = "theModel" };
            Property property = new Property() { Type = "invalidType", Format = "invalidFormat" };
            model.Properties.Data["theProperty"] = property;
            models.Data["theModel"] = model;

            models.Validate(Violations);
            Assert.AreEqual(1, Violations.Count);
            AssertInvalidProperty("Type", @"[""theModel""].Properties[""theProperty""].Type", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void ModelPropertyItemsIsInvalid()
        {
            Models models = new Models();
            Model model = new Model() { Id = "theModel" };
            Property property = new Property();
            Items items = new Items() { Type = "invalidType", Format = "invalidFormat" };
            property.Items = items;
            model.Properties.Data["theProperty"] = property;
            models.Data["theModel"] = model;

            models.Validate(Violations);
            Assert.AreEqual(1, Violations.Count);
            AssertInvalidProperty("Type", @"[""theModel""].Properties[""theProperty""].Items.Type", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void ModelPropertyRefValid()
        {
            Models models = new Models();
            Model model1 = new Model() { Id = "theModel1" };
            Model model2 = new Model() { Id = "theModel2" };
            Property property = new Property() { Reference = "theModel2", Type = null, Format = null };
            model1.Properties.Data["theProperty"] = property;
            models.Data["theModel1"] = model1;
            models.Data["theModel2"] = model2;

            models.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void ModelPropertyRefInvalid()
        {
            Models models = new Models();
            Model model1 = new Model() { Id = "theModel1" };
            Model model2 = new Model() { Id = "theModel2" };
            Property property = new Property() { Reference = "nonExistentModel", Type = null, Format = null };
            model1.Properties.Data["theProperty"] = property;
            models.Data["theModel1"] = model1;
            models.Data["theModel2"] = model2;

            models.Validate(Violations);
            AssertInvalidProperty("Reference", @"[""theModel1""].Properties[""theProperty""].Reference", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void ModelPropertyItemsRefValid()
        {
            Models models = new Models();
            Model model1 = new Model() { Id = "theModel1" };
            Model model2 = new Model() { Id = "theModel2" };
            Property property = new Property() { Type = "array", Format = null, Items = new Items() { Reference = "theModel1" } };
            model1.Properties.Data["theProperty"] = property;
            models.Data["theModel1"] = model1;
            models.Data["theModel2"] = model2;

            models.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void ModelPropertyItemsRefInvalid()
        {
            Models models = new Models();
            Model model1 = new Model() { Id = "theModel1" };
            Model model2 = new Model() { Id = "theModel2" };
            Property property = new Property() { Type = "array", Format = null, Items = new Items() { Type = null, Format = null, Reference = "noneExistentModel" } };
            model1.Properties.Data["theProperty"] = property;
            models.Data["theModel1"] = model1;
            models.Data["theModel2"] = model2;

            models.Validate(Violations);
            AssertInvalidProperty("Reference", @"[""theModel1""].Properties[""theProperty""].Items.Reference", Common.Swagger.Common.ViolationLevel.Error);
        }



        #region Sub Types

        [TestMethod]
        public void SubTypeIsSameCaseSensitive1()
        {
            Models models = new Models();
            Model model1 = new Model() { Id = "theModel1" };
            models.Data[model1.Id] = model1;
            model1.SubTypes = new string[] { "theModel1" };

            models.Validate(Violations);
            AssertInvalidProperty("SubTypes", @"[""theModel1""].SubTypes[""theModel1""]", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void SubTypeIsSameCaseSensitive2()
        {
            Models models = new Models();
            Model model1 = new Model() { Id = "theModel1" };
            models.Data[model1.Id] = model1;
            model1.SubTypes = new string[] { "theMODEL1" };

            models.Validate(Violations);
            AssertInvalidProperty("SubTypes", @"[""theModel1""].SubTypes[""theMODEL1""]", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void SubTypeReferenceIsValid()
        {
            Models models = new Models();
            Model model1 = new Model() { Id = "theModel1" };
            Model model2 = new Model() { Id = "theModel2" };
            models.Data[model1.Id] = model1;
            models.Data[model2.Id] = model2;
            model1.SubTypes = new string[] { "theModel2" };

            models.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void CircularReference()
        {
            Models models = new Models();
            Model model1 = new Model() { Id = "theModel1" };
            Model model2 = new Model() { Id = "theModel2" };
            models.Data[model1.Id] = model1;
            models.Data[model2.Id] = model2;
            model1.SubTypes = new string[] { "theModel2" };
            model2.SubTypes = new string[] { "theModel1" };

            models.Validate(Violations);

            Assert.AreEqual(2, Violations.Count);

            Assert.AreEqual("SubTypes", Violations[0].Code);
            Assert.AreEqual(@"[""theModel1""].SubTypes[""theModel2""]", Violations[0].Context);
            Assert.AreEqual(Common.Swagger.Common.ViolationLevel.Error, Violations[0].ViolationLevel);

            Assert.AreEqual("SubTypes", Violations[1].Code);
            Assert.AreEqual(@"[""theModel2""].SubTypes[""theModel1""]", Violations[1].Context);
            Assert.AreEqual(Common.Swagger.Common.ViolationLevel.Error, Violations[1].ViolationLevel);
        }

        [TestMethod]
        public void SubtypesMegaDepth()
        {
            Models models = new Models();

            // 1 -> 5
            Model model1 = new Model() { Id = "theModel1" };
            Model model5 = new Model() { Id = "theModel5" };
            models.Data[model1.Id] = model1;
            models.Data[model5.Id] = model5;

            // 2 -> 4 -> 7
            Model model2 = new Model() { Id = "theModel2" };
            Model model4 = new Model() { Id = "theModel4" };
            Model model7 = new Model() { Id = "theModel7" };
            models.Data[model2.Id] = model2;
            models.Data[model4.Id] = model4;
            models.Data[model7.Id] = model7;
            model2.SubTypes = new string[] { "theModel4" };
            model4.SubTypes = new string[] { "theModel7" };

            // 3 -> 6 -> 8 -> 9 -> 6
            Model model3 = new Model() { Id = "theModel3" };
            Model model6 = new Model() { Id = "theModel6" };
            Model model8 = new Model() { Id = "theModel8" };
            Model model9 = new Model() { Id = "theModel9" };
            models.Data[model3.Id] = model3;
            models.Data[model6.Id] = model6;
            models.Data[model8.Id] = model8;
            models.Data[model9.Id] = model9;
            model3.SubTypes = new string[] { "theModel6" };
            model6.SubTypes = new string[] { "theModel8" };
            model8.SubTypes = new string[] { "theModel9" };
            model9.SubTypes = new string[] { "theModel6" };

            models.Validate(Violations);

            Assert.AreEqual(4, Violations.Count);

            Assert.IsTrue(Violations.All((f) => f.ViolationLevel == Common.Swagger.Common.ViolationLevel.Error));
            Assert.IsTrue(Violations.All((f) => f.Code == "SubTypes"));

            Assert.AreEqual(@"[""theModel3""].SubTypes[""theModel6""]", Violations[0].Context);
            Assert.AreEqual(@"[""theModel6""].SubTypes[""theModel8""]", Violations[1].Context);
            Assert.AreEqual(@"[""theModel8""].SubTypes[""theModel9""]", Violations[2].Context);
            Assert.AreEqual(@"[""theModel9""].SubTypes[""theModel6""]", Violations[3].Context);
        }

        [TestMethod]
        public void SubtypesMegaBreadth()
        {
            Models models = new Models();

            // 1 -> 5
            Model model1 = new Model() { Id = "theModel1" };
            Model model5 = new Model() { Id = "theModel5" };
            models.Data[model1.Id] = model1;
            models.Data[model5.Id] = model5;

            // 2 -> 4 -> 7
            Model model2 = new Model() { Id = "theModel2",};
            Model model4 = new Model() { Id = "theModel4" };
            Model model7 = new Model() { Id = "theModel7" };
            models.Data[model2.Id] = model2;
            models.Data[model4.Id] = model4;
            models.Data[model7.Id] = model7;
            model2.SubTypes = new string[] { "theModel7", "theModel4" };

            // 3 -> 6 -> 8 -> 9 -> 6
            Model model3 = new Model() { Id = "theModel3" };
            Model model6 = new Model() { Id = "theModel6" };
            Model model8 = new Model() { Id = "theModel8" };
            Model model9 = new Model() { Id = "theModel9" };
            Model model10 = new Model() { Id = "theModel10" };
            models.Data[model3.Id] = model3;
            models.Data[model6.Id] = model6;
            models.Data[model8.Id] = model8;
            models.Data[model9.Id] = model9;
            models.Data[model10.Id] = model10;
            model3.SubTypes = new string[] { "theModel6" };
            model6.SubTypes = new string[] { "theModel8", "theModel10" };
            model8.SubTypes = new string[] { "theModel9" };
            model9.SubTypes = new string[] { "theModel6" };

            models.Validate(Violations);

            Assert.AreEqual(4, Violations.Count);

            Assert.IsTrue(Violations.All((f) => f.ViolationLevel == Common.Swagger.Common.ViolationLevel.Error));
            Assert.IsTrue(Violations.All((f) => f.Code == "SubTypes"));

            Assert.AreEqual(@"[""theModel3""].SubTypes[""theModel6""]", Violations[0].Context);
            Assert.AreEqual(@"[""theModel6""].SubTypes[""theModel8""]", Violations[1].Context);
            Assert.AreEqual(@"[""theModel8""].SubTypes[""theModel9""]", Violations[2].Context);
            Assert.AreEqual(@"[""theModel9""].SubTypes[""theModel6""]", Violations[3].Context);
        }

        #endregion

        #region Circular Reference Checks

        [TestMethod]
        public void None()
        {

        }

        #endregion
    }
}
