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
    /// Properties Tests
    /// </summary>
    [TestClass]
    public class PropertiesTests : TestBase
    {
        public PropertiesTests()
        {
            Properties = new Properties();
        }

        protected Properties Properties;



        [TestMethod]
        public void SanitySafe()
        {
            Assert.AreEqual(0, Properties.Data.Count);

            Properties.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }

        [TestMethod]
        public void SanityNotSafe()
        {
            Properties = new Common.Swagger12.Properties(false);

            Assert.AreEqual(0, Properties.Data.Count);

            Properties.Validate(Violations);
            Assert.AreEqual(0, Violations.Count);
        }
    }
}
