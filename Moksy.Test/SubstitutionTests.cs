using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Common;
using Moksy.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Test
{
    [TestClass]
    public class SubstitutionTests
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public SubstitutionTests()
        {
            Substitution = new Substitution();
        }

        Substitution Substitution;

        [TestMethod]
        public void ExtractVariablesNullString()
        {
            var result = Substitution.GetVariables(null);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ExtractVariablesEmptyString()
        {
            var result = Substitution.GetVariables("");
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ExtractVariablesNormalString()
        {
            var result = Substitution.GetVariables("WooHoo");
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ExtractVariablesStartHalfVariableSpecified()
        {
            var result = Substitution.GetVariables("Woo{Hoo");
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ExtractVariablesEndHalfVariableSpecified()
        {
            var result = Substitution.GetVariables("Woo}Hoo");
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ExtractVariablesEmptyVariableSpecified()
        {
            var result = Substitution.GetVariables("Woo{}Hoo");
            Assert.AreEqual(0, result.Count);
        }



        [TestMethod]
        public void ExtractVariablesOneVariableSpecifiedExactCase()
        {
            var result = Substitution.GetVariables("Woo{myVar}Hoo");
            Assert.AreEqual(1, result.Count);

            var position = result["myVar"];
            Assert.AreEqual(3, position);
        }

        [TestMethod]
        public void ExtractVariablesOneVariableSpecifiedCaseInsensitive()
        {
            var result = Substitution.GetVariables("Woo{myVar}Hoo");
            Assert.AreEqual(1, result.Count);

            var position = result["MYVAR"];
            Assert.AreEqual(3, position);
        }

        [TestMethod]
        public void ExtractVariablesTwoVariablesSpecified()
        {
            var result = Substitution.GetVariables("Woo{myVar1}Ho{myVar2}o");
            Assert.AreEqual(2, result.Count);

            var position = result["myVar1"];
            Assert.AreEqual(3, position);

            position = result["myVar2"];
            Assert.AreEqual(13, position);
        }


        [TestMethod]
        public void ExtractVariablesTwoVariablesSpecifiedButWrapped()
        {
            var result = Substitution.GetVariables("{Woo{myVar1}Ho{myVar2}o}");
            Assert.AreEqual(2, result.Count);

            var position = result["myVar1"];
            Assert.AreEqual(4, position);

            position = result["myVar2"];
            Assert.AreEqual(14, position);
        }


        [TestMethod]
        public void SubstituteVariableFullString()
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>() { { "myVar", "TheValue" } };
            var result = Substitution.Substitute("{myVar}", pairs);
            Assert.AreEqual("TheValue", result);
        }

        [TestMethod]
        public void SubstituteVariablePlacedLeft()
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>() { { "myVar", "TheValue" } };
            var result = Substitution.Substitute("Abc{myVar}", pairs);
            Assert.AreEqual("AbcTheValue", result);
        }

        [TestMethod]
        public void SubstituteVariablePlacedRight()
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>() { { "myVar", "TheValue" } };
            var result = Substitution.Substitute("{myVar}Def", pairs);
            Assert.AreEqual("TheValueDef", result);
        }

        [TestMethod]
        public void SubstituteVariablePlacedMiddle()
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>() { { "myVar", "TheValue" } };
            var result = Substitution.Substitute("Abc{myVar}D", pairs);
            Assert.AreEqual("AbcTheValueD", result);
        }

        [TestMethod]
        public void SubstituteTwoVariables()
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>() { { "myVar1", "TheValue1" }, { "myVar2", "TheValue2" } };
            var result = Substitution.Substitute("Abc{myVar1}D{myVar2}", pairs);
            Assert.AreEqual("AbcTheValue1DTheValue2", result);
        }

        [TestMethod]
        public void SubstituteTwoVariablesFirstOneNotMatchedEnd2()
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>() { { "myVar1", "TheValue1" }, { "myVar2", "TheValue2" } };
            var result = Substitution.Substitute("Abc{NotmyVar1}D{myVar2}", pairs);
            Assert.AreEqual("Abc{NotmyVar1}DTheValue2", result);
        }

        [TestMethod]
        public void SubstituteTwoVariablesSecondOneNotMatchedEnd2()
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>() { { "myVar1", "TheValue1" }, { "myVar2", "TheValue2" } };
            var result = Substitution.Substitute("Abc{myVar1}D{notmyVar2}", pairs);
            Assert.AreEqual("AbcTheValue1D{notmyVar2}", result);
        }

        [TestMethod]
        public void SubstituteTwoVariablesFirstOneNotMatchedNotAtEnd2()
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>() { { "myVar1", "TheValue1" }, { "myVar2", "TheValue2" } };
            var result = Substitution.Substitute("Abc{NotmyVar1}D{myVar2}Opq", pairs);
            Assert.AreEqual("Abc{NotmyVar1}DTheValue2Opq", result);
        }

        [TestMethod]
        public void SubstituteTwoVariablesSecondOneNotMatchedNotAtEnd2()
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>() { { "myVar1", "TheValue1" }, { "myVar2", "TheValue2" } };
            var result = Substitution.Substitute("Abc{myVar1}D{notmyVar2}Opq", pairs);
            Assert.AreEqual("AbcTheValue1D{notmyVar2}Opq", result);
        }



        [TestMethod]
        public void StringHasNoVarialbesNull()
        {
            Assert.IsFalse(Substitution.HasVariables(null));
        }

        [TestMethod]
        public void StringHasNoVarialbesEmpty()
        {
            Assert.IsFalse(Substitution.HasVariables(""));
        }

        [TestMethod]
        public void StringHasNoVarialbesString()
        {
            Assert.IsFalse(Substitution.HasVariables("/Pet"));
        }

        [TestMethod]
        public void StringHasNoVarialbesOneVariable()
        {
            Assert.IsTrue(Substitution.HasVariables("/Pet('{id}')"));
        }



        [TestMethod]
        public void RegExConverationWorksWithNull()
        {
            Assert.IsNull(Substitution.ConvertPatternToRegularExpression(null));
        }

        [TestMethod]
        public void RegExConverationWorksWithEmpty()
        {
            Assert.AreEqual("", Substitution.ConvertPatternToRegularExpression(""));
        }

        [TestMethod]
        public void RegExConverationWorksWithNoVars()
        {
            Assert.AreEqual("/Pet", Substitution.ConvertPatternToRegularExpression("/Pet"));
        }

        [TestMethod]
        public void RegExConverationWorksWithOneVar()
        {
            Assert.AreEqual(@"/Pet\('(.*)'\)", Substitution.ConvertPatternToRegularExpression("/Pet('{id}')"));
        }
    }
}
