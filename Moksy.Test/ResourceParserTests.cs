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
        public void SimpleConvertNoParametersWithCaptures()
        {
            var result = RouteParser.ConvertPatternToRegularExpression("/Pet", true);
            Assert.AreEqual("^?<resourceName>/Pet$", result);
        }

        [TestMethod]
        public void SimpleConvertOneParameter()
        {
            var result = RouteParser.ConvertPatternToRegularExpression("/Pet/{prop}");
            Assert.AreEqual("^(/Pet/)(.*?)()$", result);
        }

        [TestMethod]
        public void SimpleConvertOneParameterWithCaptures()
        {
            var result = RouteParser.ConvertPatternToRegularExpression("/Pet/{prop}", true);
            Assert.AreEqual("^(?<resourceName>/Pet/)(?<resourceIdentifier>.*?)()$", result);
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
        public void SimpleConvertTwoParametersWithResources()
        {
            var result = RouteParser.ConvertPatternToRegularExpression("/Pet/{prop}/Toy/{name}");
            Assert.AreEqual("^(/Pet/)(.*?)()(/Toy/)(.*?)()$", result);
        }

        [TestMethod]
        public void SimpleConvertTwoParametersWithResourcesAndCaptures()
        {
            var result = RouteParser.ConvertPatternToRegularExpression("/Pet/{prop}/Toy/{name}", true);
            Assert.AreEqual("^(?<resourceName>/Pet/)(?<resourceIdentifier>.*?)()(?<resourceName>/Toy/)(?<resourceIdentifier>.*?)()$", result);
        }

        [TestMethod]
        public void SimpleConvertTwoParametersWithResourcesAndResourceAtEnd()
        {
            var result = RouteParser.ConvertPatternToRegularExpression("/Pet/{prop}/Toy/{name}/Price");
            Assert.AreEqual("^(/Pet/)(.*?)()(/Toy/)(.*?)()(/Price)$", result);
        }

        [TestMethod]
        public void SimpleConvertTwoParametersNoResources()
        {
            var result = RouteParser.ConvertPatternToRegularExpression("/Pet/{prop}/{name}");
            Assert.AreEqual("^(/Pet/)(.*?)()(/)(.*?)()$", result);
        }

        [TestMethod]
        public void SimpleConvertThreeParameters()
        {
            var result = RouteParser.ConvertPatternToRegularExpression("/Pet/{prop}/Toy/{name}/Price/{price}");
            Assert.AreEqual("^(/Pet/)(.*?)()(/Toy/)(.*?)()(/Price/)(.*?)()$", result);
        }

        [TestMethod]
        public void SimpleConvertThreeParametersNoResources()
        {
            var result = RouteParser.ConvertPatternToRegularExpression("/{prop}/{name}/{price}");
            Assert.AreEqual("^(/)(.*?)()(/)(.*?)()(/)(.*?)()$", result);
        }

        [TestMethod]
        public void SimpleConvertThreeParametersSomeResourcesAndNoResources()
        {
            var result = RouteParser.ConvertPatternToRegularExpression("/{prop}/Toy/{name}/{price}/Cost/{currency}/{value}");
            Assert.AreEqual("^(/)(.*?)()(/Toy/)(.*?)()(/)(.*?)()(/Cost/)(.*?)()(/)(.*?)()$", result);
        }

        [TestMethod]
        public void SimpleConvertThreeParametersSomeResourcesAndNoResourcesWithCaptures()
        {
            var result = RouteParser.ConvertPatternToRegularExpression("/{prop}/Toy/{name}/{price}/Cost/{currency}/{value}", true);
            Assert.AreEqual("^(/)(?<resourceIdentifier>.*?)()(?<resourceName>/Toy/)(?<resourceIdentifier>.*?)()(/)(?<resourceIdentifier>.*?)()(?<resourceName>/Cost/)(?<resourceIdentifier>.*?)()(/)(?<resourceIdentifier>.*?)()$", result);
        }

        [TestMethod]
        public void PetKind()
        {
            var result = RouteParser.ConvertPatternToRegularExpression("/Pet('{Kind}')");
            Assert.AreEqual("^(/Pet\\(')(.*?)('\\))$", result);
        }



        [TestMethod]
        public void ConvertThreeParametersReference()
        {
            // This is a reference for how a path is parsed and the various components extracted. 
            var result = RouteParser.ConvertPatternToRegularExpression("/{prop}/Toy/{name}/{price}/Cost/{currency}/{value}", true);
            Assert.AreEqual("^(/)(?<resourceIdentifier>.*?)()(?<resourceName>/Toy/)(?<resourceIdentifier>.*?)()(/)(?<resourceIdentifier>.*?)()(?<resourceName>/Cost/)(?<resourceIdentifier>.*?)()(/)(?<resourceIdentifier>.*?)()$", result);

            var candidate = "/candidate/Toy/Bone/50/Cost/dollar/85";
            var matches = Regex.Matches(candidate, result, RegexOptions.ExplicitCapture);

            // We now need to extract the resourceNames and resourceIdentifiers. By sorting their start index, we can work out the order in which 
            // they occurred and then walk an in-memory database. 

            var resourceNames = matches[0].Groups["resourceName"];
            var resourceIdentifiers = matches[0].Groups["resourceIdentifier"];

            List<Placeholder> parsed = new List<Placeholder>();

            foreach (Capture n in resourceNames.Captures)
            {
                parsed.Add(new Placeholder() { Type = "resourceName", Value = n.Value.Replace("/", ""), Index = n.Index });
            }
            foreach(Capture n in resourceIdentifiers.Captures)
            {
                parsed.Add(new Placeholder() { Type = "resourceIdentifier", Value = n.Value, Index = n.Index });
            }

            parsed = parsed.OrderBy(f => f.Index).ToList();

            Assert.AreEqual("resourceIdentifier", parsed[0].Type);
            Assert.AreEqual("candidate", parsed[0].Value);

            Assert.AreEqual("resourceName", parsed[1].Type);
            Assert.AreEqual("Toy", parsed[1].Value);

            Assert.AreEqual("resourceIdentifier", parsed[2].Type);
            Assert.AreEqual("Bone", parsed[2].Value);

            Assert.AreEqual("resourceIdentifier", parsed[3].Type);
            Assert.AreEqual("50", parsed[3].Value);

            Assert.AreEqual("resourceName", parsed[4].Type);
            Assert.AreEqual("Cost", parsed[4].Value);

            Assert.AreEqual("resourceIdentifier", parsed[5].Type);
            Assert.AreEqual("dollar", parsed[5].Value);

            Assert.AreEqual("resourceIdentifier", parsed[6].Type);
            Assert.AreEqual("85", parsed[6].Value);
        }

        [TestMethod]
        public void ConvertThreeParametersWithParse()
        {
            var tokens = RouteParser.Parse("/candidate/Toy/Bone/50/Cost/dollar/85", "/{prop}/Toy/{name}/{price}/Cost/{currency}/{value}").ToList();
            Assert.AreEqual(10, tokens.Count());

            // NOTE: The reason that some resource are empty is because there HAS to be a resource (of some kind) prior to the property. 
            //       The easiest and most transparent way to do this is to simply make the resource name empty. 

            AssertRouteToken(tokens, 0, RouteTokenKind.Resource, "", "");
            AssertRouteToken(tokens, 1, RouteTokenKind.Property, "prop", "candidate");
            AssertRouteToken(tokens, 2, RouteTokenKind.Resource, "Toy", "Toy");
            AssertRouteToken(tokens, 3, RouteTokenKind.Property, "name", "Bone");
            AssertRouteToken(tokens, 4, RouteTokenKind.Resource, "", "");
            AssertRouteToken(tokens, 5, RouteTokenKind.Property, "price", "50");
            AssertRouteToken(tokens, 6, RouteTokenKind.Resource, "Cost", "Cost");
            AssertRouteToken(tokens, 7, RouteTokenKind.Property, "currency", "dollar");
            AssertRouteToken(tokens, 8, RouteTokenKind.Resource, "", "");
            AssertRouteToken(tokens, 9, RouteTokenKind.Property, "value", "85");
        }

        protected void AssertRouteToken(List<RouteToken> tokens, int index, RouteTokenKind kind, string name, string value)
        {
            Assert.AreEqual(kind, tokens[index].Kind);
            Assert.AreEqual(name, tokens[index].Name);
            Assert.AreEqual(value, tokens[index].Value);
        }

        private class Placeholder
        {
            public int Index {get;set;}
            public string Type {get;set;}
            public string Value {get;set;}
        }

        

        #endregion
    }
}
