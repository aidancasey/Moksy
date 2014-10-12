using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Moksy.Test
{
    [TestClass]
    public class ResourceParserTests
    {
        #region Parse Resources

        [TestMethod]
        public void Null()
        {
            var result = RouteParser.Parse(null, null);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void Null2()
        {
            var result = RouteParser.Parse("", null);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void Null3()
        {
            var result = RouteParser.Parse(null, "");
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void Empty()
        {
            var result = RouteParser.Parse("", "").ToArray();
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("", result[0].Value);
            Assert.AreEqual(RouteTokenKind.Resource, result[0].Kind);
        }



        [TestMethod]
        public void ExactMatchResource()
        {
            var result = RouteParser.Parse("/Pet", "/Pet").ToArray();
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Pet", result[0].Value);
            Assert.AreEqual("Pet", result[0].Name);
            Assert.AreEqual(RouteTokenKind.Resource, result[0].Kind);
        }

        [TestMethod]
        public void ExactMatchResourceAndIndex()
        {
            var result = RouteParser.Parse("/Pet/Dog", "/Pet/{Kind}").ToArray();
            Assert.AreEqual(2, result.Count());

            Assert.AreEqual("Pet", result[0].Value);
            Assert.AreEqual("Pet", result[0].Name);
            Assert.AreEqual(RouteTokenKind.Resource, result[0].Kind);

            Assert.AreEqual("Dog", result[1].Value);
            Assert.AreEqual("Kind", result[1].Name);
            Assert.AreEqual(RouteTokenKind.Property, result[1].Kind);
        }

        [TestMethod]
        public void MatchResourcePropertyResource()
        {
            var result = RouteParser.Parse("/Pet/Dog/Toy", "/Pet/{Kind}/Toy").ToArray();
            Assert.AreEqual(3, result.Count());

            Assert.AreEqual("Pet", result[0].Value);
            Assert.AreEqual(RouteTokenKind.Resource, result[0].Kind);

            Assert.AreEqual("Dog", result[1].Value);
            Assert.AreEqual("Kind", result[1].Name);
            Assert.AreEqual(RouteTokenKind.Property, result[1].Kind);

            Assert.AreEqual("Toy", result[2].Value);
            Assert.AreEqual("Toy", result[2].Name);
            Assert.AreEqual(RouteTokenKind.Resource, result[2].Kind);
        }

        [TestMethod]
        public void MatchResourcePropertyResourceProperty()
        {
            var result = RouteParser.Parse("/Pet/Dog/Toy/Bone", "/Pet/{Kind}/Toy/{Name}").ToArray();
            Assert.AreEqual(4, result.Count());
 
            Assert.AreEqual("Pet", result[0].Value);
            Assert.AreEqual(RouteTokenKind.Resource, result[0].Kind);

            Assert.AreEqual("Dog", result[1].Value);
            Assert.AreEqual("Kind", result[1].Name);
            Assert.AreEqual(RouteTokenKind.Property, result[1].Kind);

            Assert.AreEqual("Toy", result[2].Value);
            Assert.AreEqual(RouteTokenKind.Resource, result[2].Kind);

            Assert.AreEqual("Bone", result[3].Value);
            Assert.AreEqual("Name", result[3].Name);
            Assert.AreEqual(RouteTokenKind.Property, result[3].Kind);
        }




        [TestMethod]
        public void ParsePatternNull()
        {
            var result = RouteParser.Parse(null).ToArray();
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void ParsePatternEmpty()
        {
            var result = RouteParser.Parse("").ToArray();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("", result[0].Value);
            Assert.AreEqual(RouteTokenKind.Resource, result[0].Kind);
        }

        [TestMethod]
        public void ParsePatternResource()
        {
            var result = RouteParser.Parse("/Pet").ToArray();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("Pet", result[0].Value);
            Assert.AreEqual(RouteTokenKind.Resource, result[0].Kind);
        }

        [TestMethod]
        public void ParsePatternResourceProperty()
        {
            var result = RouteParser.Parse("/Pet/{Kind}").ToArray();
            Assert.AreEqual(2, result.Length);

            Assert.AreEqual("Pet", result[0].Value);
            Assert.AreEqual(RouteTokenKind.Resource, result[0].Kind);

            Assert.AreEqual("Kind", result[1].Value);
            Assert.AreEqual(RouteTokenKind.Property, result[1].Kind);
        }

        [TestMethod]
        public void ParsePatternResourcePropertyResource()
        {
            var result = RouteParser.Parse("/Pet/{Kind}/Toy").ToArray();
            Assert.AreEqual(3, result.Length);

            Assert.AreEqual("Pet", result[0].Value);
            Assert.AreEqual(RouteTokenKind.Resource, result[0].Kind);

            Assert.AreEqual("Kind", result[1].Value);
            Assert.AreEqual(RouteTokenKind.Property, result[1].Kind);

            Assert.AreEqual("Toy", result[2].Value);
            Assert.AreEqual(RouteTokenKind.Resource, result[2].Kind);
        }




        [TestMethod]
        public void SimpleConvertNoParameters()
        {
            var result = RouteParser.ConvertPatternToRegularExpression("/Pet");
            Assert.AreEqual("^/Pet$", result);
        }

        [TestMethod]
        public void SimpleConvertOneParameter()
        {
            var result = RouteParser.ConvertPatternToRegularExpression("/Pet/{prop}");
            Assert.AreEqual("^(/Pet/)(.*?)()$", result);
        }

        [TestMethod]
        public void SimpleConvertOneAndAHalfParameters()
        {
            var result = RouteParser.ConvertPatternToRegularExpression("/Pet/{prop}/Toy");
            Assert.AreEqual("^(/Pet/)(.*?)()(/Toy)$", result);
        }

        [TestMethod]
        public void SimpleConvertOneAndAHalfParametersWithAddedSlash()
        {
            var result = RouteParser.ConvertPatternToRegularExpression("/Pet/{prop}/Toy/");
            Assert.AreEqual("^(/Pet/)(.*?)()(/Toy/)$", result);
        }

        [TestMethod]
        public void SimpleConvertTwoParameters()
        {
            var result = RouteParser.ConvertPatternToRegularExpression("/Pet/{prop}/Toy/{name}");
            Assert.AreEqual("^(/Pet/)(.*?)()(/Toy/)(.*?)()$", result);
        }

        [TestMethod]
        public void PetKind()
        {
            var result = RouteParser.ConvertPatternToRegularExpression("/Pet('{Kind}')");
            Assert.AreEqual("^(/Pet\\(')(.*?)('\\))$", result);
        }

        

        #endregion
    }
}
