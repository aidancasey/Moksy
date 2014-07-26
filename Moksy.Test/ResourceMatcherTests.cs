using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moksy.Common;

namespace Moksy.Test
{
    [TestClass]
    public class ResourceMatcherTests
    {
        public ResourceMatcherTests()
        {
        }

        [TestMethod]
        public void NullPatternNullPath()
        {
            Assert.IsTrue(RouteMatcher.Matches(null, null));
        }

        [TestMethod]
        public void NullPatternPath()
        {
            Assert.IsFalse(RouteMatcher.Matches(null, "thepath"));
        }

        [TestMethod]
        public void PatternNullPath()
        {
            Assert.IsFalse(RouteMatcher.Matches("/Pet", null));
        }

        [TestMethod]
        public void SimplePatternSimplePathYes()
        {
            Assert.IsTrue(RouteMatcher.Matches("/Pet", "/Pet"));
        }

        [TestMethod]
        public void SimplePatternSimplePathNo()
        {
            Assert.IsFalse(RouteMatcher.Matches("/Pet", "/PET"));
        }

        [TestMethod]
        public void SimplePatternSimplePathTrailingYes()
        {
            Assert.IsTrue(RouteMatcher.Matches("/Pet/", "/Pet/"));
        }

        [TestMethod]
        public void SimplePatternSimplePathTrailingNot()
        {
            Assert.IsFalse(RouteMatcher.Matches("/Pet/", "/Pet/A"));
        }



        [TestMethod]
        public void OneDeepYes()
        {
            Assert.IsTrue(RouteMatcher.Matches("/Pet/{Kind}", "/Pet/Dog"));
        }

        [TestMethod]
        public void OneDeep2Yes()
        {
            Assert.IsTrue(RouteMatcher.Matches("/Pet/{Kind}", "/Pet/DOG"));
        }

        [TestMethod]
        public void OneDeep3Yes()
        {
            // This will match - all matches are .*? (match anything, not greedy). 
            Assert.IsTrue(RouteMatcher.Matches("/Pet/{Kind}", "/Pet/Dog('woo')"));
        }

        [TestMethod]
        public void OneDeepNo2()
        {
            // This will match - all matches are .*? (match anything, not greedy). 
            Assert.IsTrue(RouteMatcher.Matches("/Pet/{Kind}", "/Pet/Dog/Toy"));
        }

        [TestMethod]
        public void OneDeepYesTrailing()
        {
            Assert.IsTrue(RouteMatcher.Matches("/Pet/{Kind}/", "/Pet/Dog/"));
        }


        [TestMethod]
        public void OnePoint5DeepYes()
        {
            Assert.IsTrue(RouteMatcher.Matches("/Pet/{Kind}/Toy", "/Pet/Dog/Toy"));
        }

        [TestMethod]
        public void OnePoint5DeepYesTrailing()
        {
            Assert.IsTrue(RouteMatcher.Matches("/Pet/{Kind}/Toy/", "/Pet/Dog/Toy/"));
        }



        [TestMethod]
        public void TwoDeepYes()
        {
            Assert.IsTrue(RouteMatcher.Matches("/Pet/{Kind}/Toy/{Name}", "/Pet/Dog/Toy/Bone"));
        }

        [TestMethod]
        public void TwoDeepNo()
        {
            Assert.IsFalse(RouteMatcher.Matches("/Pet/{Kind}/Toy/{Name}", "/Pet/Dog/TOY/Bone"));
        }

        [TestMethod]
        public void TwoDeepNo2()
        {
            Assert.IsFalse(RouteMatcher.Matches("/Pet/{Kind}/Toy/{Name}", "/Pet/Dog/Food/Bone"));
        }



        [TestMethod]
        public void TwoDeepYesTrailing()
        {
            Assert.IsTrue(RouteMatcher.Matches("/Pet/{Kind}/Toy/{Name}/", "/Pet/Dog/Toy/Bone/"));
        }
    }
}
