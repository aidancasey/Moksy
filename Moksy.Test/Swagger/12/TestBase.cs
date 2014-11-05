using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Common.Swagger.Common;
using Moksy.Common.Swagger12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Test.Swagger._12
{
    [TestClass]
    public class TestBase
    {
        public TestBase()
        {

            Violations = new ViolationCollection();
        }

        protected void AssertInvalidProperty(string context, ViolationLevel level)
        {
            Assert.AreEqual(1, Violations.Count);
            Assert.AreEqual(context, Violations[0].Context);
            Assert.AreEqual(context, Violations[0].Code);
            Assert.AreEqual(level, Violations[0].ViolationLevel);
        }

        protected void AssertInvalidProperty(string code, string context, ViolationLevel level)
        {
            Assert.AreEqual(1, Violations.Count);
            Assert.AreEqual(code, Violations[0].Code);
            Assert.AreEqual(context, Violations[0].Context);
            Assert.AreEqual(level, Violations[0].ViolationLevel);
        }

        protected ViolationCollection Violations;

        public TestContext TestContext { get; set; }
    }
}
