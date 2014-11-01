using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moksy.Common.Swagger12;

namespace Moksy.Test.Swagger._12
{
    /// <summary>
    /// Items Tests
    /// </summary>
    [TestClass]
    public class ItemsTests : TestBase
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public ItemsTests()
        {
            Items = new Items();
        }

        protected Items Items;



        [TestMethod]
        public void SanitySafe()
        {
            Assert.IsTrue(Items.IsString);

            Items.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void SanityNotSafe()
        {
            Items = new Common.Swagger12.Items(false);

            Items.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }


        #region Type Checking

        [TestMethod]
        public void Int32()
        {
            var items = new Items() { Type = "integer", Format = "int32" };
            AssertType(items, true, false, false, false, false, false, false, false, false);
        }

        [TestMethod]
        public void Int64()
        {
            var items = new Items() { Type = "integer", Format = "int64" };
            AssertType(items, false, true, false, false, false, false, false, false, false);
        }

        [TestMethod]
        public void Float()
        {
            var items = new Items() { Type = "number", Format = "float" };
            AssertType(items, false, false, true, false, false, false, false, false, false);
        }

        [TestMethod]
        public void Double()
        {
            var items = new Items() { Type = "number", Format = "double" };
            AssertType(items, false, false, false, true, false, false, false, false, false);
        }

        [TestMethod]
        public void String()
        {
            var items = new Items() { Type = "string", Format = null };
            AssertType(items, false, false, false, false, true, false, false, false, false);
        }

        [TestMethod]
        public void Byte()
        {
            var items = new Items() { Type = "string", Format = "byte" };
            AssertType(items, false, false, false, false, false, true, false, false, false);
        }

        [TestMethod]
        public void Boolean()
        {
            var items = new Items() { Type = "boolean", Format = null };
            AssertType(items, false, false, false, false, false, false, true, false, false);
        }

        [TestMethod]
        public void Date()
        {
            var items = new Items() { Type = "string", Format = "date" };
            AssertType(items, false, false, false, false, false, false, false, true, false);
        }

        [TestMethod]
        public void DateTime()
        {
            var items = new Items() { Type = "string", Format = "date-time" };
            AssertType(items, false, false, false, false, false, false, false, false, true);
        }

        [TestMethod]
        public void Reference()
        {
            var items = new Items() { Type = null, Reference = "theModel" };
            Assert.IsTrue(items.IsReference);
        }

        protected void AssertType(Items items, bool isInt32, bool isInt64, bool isFloat, bool isDouble, bool isString, bool isByte, bool isBoolean, bool isDate, bool isDateTime)
        {
            Assert.AreEqual(items.IsInt32, isInt32);
            Assert.AreEqual(items.IsInt64, isInt64);
            Assert.AreEqual(items.IsFloat, isFloat);
            Assert.AreEqual(items.IsDouble, isDouble);
            Assert.AreEqual(items.IsString, isString);
            Assert.AreEqual(items.IsByte, isByte);
            Assert.AreEqual(items.IsBoolean, isBoolean);
            Assert.AreEqual(items.IsDate, isDate);
            Assert.AreEqual(items.IsDateTime, isDateTime);
        }

        #endregion // Type Checking
    }
}
