using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Common.Swagger12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Moksy.Test.Swagger._12
{
    /// <summary>
    /// Property Tests
    /// </summary>
    [TestClass]
    public class PropertyTests : TestBase
    {
        public PropertyTests()
        {
            Property = new Property();
        }

        protected Property Property;



        [TestMethod]
        public void PropertySanitySafe()
        {
            Assert.AreEqual("", Property.Description);
            Assert.IsFalse(Property.UniqueItems);
            Assert.AreEqual(0, Property.Enums.Length);
            Assert.IsNotNull(Property.Items);

            Property.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void PropertySanityNotSafe()
        {
            Property = new Property(false);
            Assert.IsNull(Property.Description);
            Assert.IsFalse(Property.UniqueItems);
            Assert.IsNull(Property.Enums);
            Assert.IsNotNull(Property.Items);

            Property.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void TypeIsArrayValid()
        {
            Property.Type = "array";

            Property.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void TypeIsArrayValidButFormatNotNull()
        {
            Property.Type = "array";
            Property.Format = "notnull";

            Property.Validate(Violations);
            AssertInvalidProperty("Type", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void TypeIntegerAndInt32Format()
        {
            Property.Type = "integer";
            Property.Format = "int32";

            Property.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void TypeIntegerAndInt32FormatCaseSensitive()
        {
            Property.Type = "Integer";
            Property.Format = "notnaid";

            Property.Validate(Violations);
            AssertInvalidProperty("Type", Common.Swagger.Common.ViolationLevel.Error);

        }

        [TestMethod]
        public void TypeIntegerAndInt32FormatMismatchFormat()
        {
            Property.Type = "integer";
            Property.Format = "int128";

            Property.Validate(Violations);
            AssertInvalidProperty("Type", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void TypeIsProperty()
        {
            Property.Type = "a";

            Property.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }



        [TestMethod]
        public void DefaultSerializesAsNull()
        {
            var property = new Property();
            property.DefaultValue = null;
            var json = JsonConvert.SerializeObject(property);
            Assert.IsTrue(json.Contains(@"""defaultValue"":null"));
        }

        [TestMethod]
        public void DefaultSerializesAsString()
        {
            var property = new Property();
            property.DefaultValue = "theValue";
            var json = JsonConvert.SerializeObject(property);
            Assert.IsTrue(json.Contains(@"""defaultValue"":""theValue"""));
        }

        [TestMethod]
        public void DefaultSerializesAsInt32()
        {
            var property = new Property();
            property.DefaultValue = 123;
            var json = JsonConvert.SerializeObject(property);
            Assert.IsTrue(json.Contains(@"""defaultValue"":123"));
        }



        #region Type Checking

        [TestMethod]
        public void Int32()
        {
            var property = new Property() { Type = "integer", Format = "int32" };
            AssertType(property, true, false, false, false, false, false, false, false, false);
        }

        [TestMethod]
        public void Int64()
        {
            var property = new Property() { Type = "integer", Format = "int64" };
            AssertType(property, false, true, false, false, false, false, false, false, false);
        }

        [TestMethod]
        public void Float()
        {
            var property = new Property() { Type = "number", Format = "float" };
            AssertType(property, false, false, true, false, false, false, false, false, false);
        }

        [TestMethod]
        public void Double()
        {
            var property = new Property() { Type = "number", Format = "double" };
            AssertType(property, false, false, false, true, false, false, false, false, false);
        }

        [TestMethod]
        public void String()
        {
            var property = new Property() { Type = "string", Format = null };
            AssertType(property, false, false, false, false, true, false, false, false, false);
        }

        [TestMethod]
        public void Byte()
        {
            var property = new Property() { Type = "string", Format = "byte" };
            AssertType(property, false, false, false, false, false, true, false, false, false);
        }

        [TestMethod]
        public void Boolean()
        {
            var property = new Property() { Type = "boolean", Format = null };
            AssertType(property, false, false, false, false, false, false, true, false, false);
        }

        [TestMethod]
        public void Date()
        {
            var property = new Property() { Type = "string", Format = "date" };
            AssertType(property, false, false, false, false, false, false, false, true, false);
        }

        [TestMethod]
        public void DateTime()
        {
            var property = new Property() { Type = "string", Format = "date-time" };
            AssertType(property, false, false, false, false, false, false, false, false, true);
        }

        [TestMethod]
        public void Array()
        {
            var property = new Property() { Type = "array", Format = null };
            Assert.IsTrue(property.IsArray);
        }

        [TestMethod]
        public void Model()
        {
            var property = new Property() { Type = "theModelId", Format = null };
            Assert.IsTrue(property.IsModelReference);
        }

        [TestMethod]
        public void NotModel()
        {
            var property = new Property() { Type = "theModelId", Format = "theFormat" };
            Assert.IsFalse(property.IsModelReference);
        }



        [TestMethod]
        public void Int32MinimumInvalid()
        {
            AssertPropertyTypeException("integer", "int32", "Minimum", "notanumber", null, null);
        }

        [TestMethod]
        public void Int64MaximumInvalid()
        {
            AssertPropertyTypeException("integer", "int64", "Maximum", null, "notanumber", null);
        }

        [TestMethod]
        public void FloatDefaultValueInvalid()
        {
            AssertPropertyTypeException("number", "float", "DefaultValue", null, null, "notanumber");
        }



        [TestMethod]
        public void Int32DefaultOutOfRange()
        {
            var property = new Property() { Type = "integer", Format = "int32", DefaultValue = "100", Minimum = "110", Maximum = "120" };
            property.Validate(Violations);
            AssertInvalidProperty("DefaultValue", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void Int32DefaultWithinRange()
        {
            var property = new Property() { Type = "integer", Format = "int32", DefaultValue = "115", Minimum = "110", Maximum = "120" };
            property.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void Int64DefaultOutOfRange()
        {
            var property = new Property() { Type = "integer", Format = "int64", DefaultValue = "100", Minimum = "110", Maximum = "120" };
            property.Validate(Violations);
            AssertInvalidProperty("DefaultValue", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void Int64DefaultWithinRange()
        {
            var property = new Property() { Type = "integer", Format = "int64", DefaultValue = "115", Minimum = "110", Maximum = "120" };
            property.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void FloatDefaultOutOfRange()
        {
            var property = new Property() { Type = "number", Format = "float", DefaultValue = "100", Minimum = "110", Maximum = "120" };
            property.Validate(Violations);
            AssertInvalidProperty("DefaultValue", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void FloatDefaultWithinRange()
        {
            var property = new Property() { Type = "number", Format = "float", DefaultValue = "115", Minimum = "110", Maximum = "120" };
            property.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void DoubleDefaultOutOfRange()
        {
            var property = new Property() { Type = "number", Format = "double", DefaultValue = "100", Minimum = "110", Maximum = "120" };
            property.Validate(Violations);
            AssertInvalidProperty("DefaultValue", Common.Swagger.Common.ViolationLevel.Error);
        }

        [TestMethod]
        public void DoubleDefaultWithinRange()
        {
            var property = new Property() { Type = "number", Format = "double", DefaultValue = "115", Minimum = "110", Maximum = "120" };
            property.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }




        protected void AssertPropertyTypeException(string type, string format, string propertyName, string minimum, string maximum, string defaultValue)
        {
            var property = new Property() { Type = type, Format = format, Minimum = minimum, Maximum = maximum, DefaultValue = defaultValue };
            property.Validate(Violations);
            AssertInvalidProperty(propertyName, Common.Swagger.Common.ViolationLevel.Error);
        }

        protected void AssertType(Property p, bool isInt32, bool isInt64, bool isFloat, bool isDouble, bool isString, bool isByte, bool isBoolean, bool isDate, bool isDateTime)
        {
            Assert.AreEqual(p.IsInt32, isInt32);
            Assert.AreEqual(p.IsInt64, isInt64);
            Assert.AreEqual(p.IsFloat, isFloat);
            Assert.AreEqual(p.IsDouble, isDouble);
            Assert.AreEqual(p.IsString, isString);
            Assert.AreEqual(p.IsByte, isByte);
            Assert.AreEqual(p.IsBoolean, isBoolean);
            Assert.AreEqual(p.IsDate, isDate);
            Assert.AreEqual(p.IsDateTime, isDateTime);
        }

        #endregion // Type Checking

        #region Enum

        [TestMethod]
        public void EnumsEmptyIsNotEnum()
        {
            Assert.IsFalse(Property.IsEnum);
        }

        [TestMethod]
        public void EnumsOneEntryIsEnum()
        {
            Property.Enums = new string[] { "theValue1" };

            Assert.IsTrue(Property.IsEnum);
        }

        [TestMethod]
        public void EnumsOneEntryIsEnumDefaultValueMatches()
        {
            Property.Enums = new string[] { "theValue1", "theValue2", "theValue3" };
            Property.DefaultValue = "theValue2";

            Assert.IsTrue(Property.IsEnum);

            Property.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void EnumsOneEntryIsEnumDefaultValueNotMatches()
        {
            Property.Enums = new string[] { "theValue1", "theValue2", "theValue3" };
            Property.DefaultValue = "someOtherValue";

            Assert.IsTrue(Property.IsEnum);

            Property.Validate(Violations);
            AssertInvalidProperty("DefaultValue", Common.Swagger.Common.ViolationLevel.Error);
        }



        [TestMethod]
        public void ReferenceNullIsNotReference()
        {
            Property.Reference = null;

            Assert.IsFalse(Property.IsReference);
        }

        [TestMethod]
        public void ReferenceNullIsReference()
        {
            Property.Type = null;
            Property.Reference = "TheVar";

            Assert.IsTrue(Property.IsReference);
        }

        #endregion
    }
}
