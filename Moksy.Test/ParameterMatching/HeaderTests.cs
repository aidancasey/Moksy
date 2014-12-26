using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Common;
using Moksy.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Test.ParameterMatching
{
    /// <summary>
    /// Tests conditions where Headers are specified. 
    /// </summary>
    [TestClass]
    public class HeaderTests
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public HeaderTests()
        {
        }

        [TestInitialize]
        public void Initialize()
        {
            Evaluator = new SimulationConditionEvaluator();
        }

        SimulationConditionEvaluator Evaluator;


        
        [TestMethod]
        public void NullHeadersAlwaysMatches()
        {
            SimulationCondition c = new SimulationCondition();
            Assert.IsTrue(Evaluator.Matches(c, (IEnumerable<Header>) null));

            c.Header("TheHeader", "Woo");
            Assert.IsTrue(Evaluator.Matches(c, (IEnumerable<Header>)null));
        }

        [TestMethod]
        public void EmptyHeadersAndConditionAlwaysMatches()
        {
            List<Header> headers = new List<Header>();
            SimulationCondition c = new SimulationCondition();
            Assert.IsTrue(Evaluator.Matches(c, headers)); 
        }

        [TestMethod]
        public void OneHeaderDoesMatchIfThereAreNoHeadersInTheCondition()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));

            SimulationCondition c = new SimulationCondition();

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void OneHeaderMatchesCaseSensitive()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader", "TheValue");

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void OneHeaderDoesnotMatchCaseSensitive()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeadER", "TheValue");

            Assert.IsFalse(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void OneConditionMatchesAgainstTwoFirst()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));
            headers.Add(new Header("TheHeader2", "TheValue2"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader", "TheValue");

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void OneConditionMatchesAgainstTwoSecond()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));
            headers.Add(new Header("TheHeader2", "TheValue2"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader2", "TheValue2");

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void TwoConditionsSameHeaderNameDifferentValuesMatchesLast()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue1"));
            headers.Add(new Header("TheHeader", "TheValue2"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader", "TheValue2");

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }


        [TestMethod]
        public void OneHeaderExistsComparison()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader", "TheValue", ComparisonType.Exists);

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void OneHeaderNotExistsComparison()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader", "TheValue", ComparisonType.NotExists);

            Assert.IsFalse(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void OneHeaderExistsComparisonCaseSensitive()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader", "TheValue", ComparisonType.Exists | ComparisonType.CaseSensitive);

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void OneHeaderExistsComparisonCaseSensitive1()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeAder", "ThEValue", ComparisonType.Exists | ComparisonType.CaseSensitive);

            Assert.IsFalse(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void OneHeaderExistsComparisonCaseSensitive2()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("ThEHeader", "ThEValue", ComparisonType.Exists | ComparisonType.CaseInsensitive);

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void OneHeaderOnlyExistsComparisonCaseSensitive()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader", ComparisonType.Exists | ComparisonType.CaseSensitive);

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void OneHeaderOnlyExistsComparisonCaseSensitive2()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHEader", ComparisonType.Exists | ComparisonType.CaseSensitive);

            Assert.IsFalse(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void OneHeaderOnlyExistsComparisonCaseInsensitive2()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("ThEHeader", "TheValue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeADer", ComparisonType.Exists | ComparisonType.CaseInsensitive);

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void OneHeaderOnlyUrlEncodedNotMatch()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheH%2feader", "TheValue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheH/eader", ComparisonType.Exists);

            Assert.IsFalse(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void OneHeaderOnlyUrlEncodedMatch()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheH%2feader", "TheValue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheH/eader", ComparisonType.Exists | ComparisonType.UrlEncode);

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void OneHeaderAndValueOnlyUrlEncodedMatch()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheH%2feader", "TheV%2falue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheH/eader", "TheV/alue", ComparisonType.Exists | ComparisonType.UrlEncode);

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void OneHeaderAndValueOnlyUrlEncodedNotMatch()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheH%2feader", "TheV%falue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheH/eader", "TheV/alue", ComparisonType.Exists);

            Assert.IsFalse(Evaluator.Matches(c, headers));
        }




        [TestMethod]
        public void HeaderAndValueNone()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue", Persistence.None));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader", "TheValue", Persistence.None);

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void HeaderAndValueImplicitExists()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader", "TheValue");

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void HeaderAndValueImplicitNoneIsSkipped()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader", "TheValue", Persistence.None);

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void HeaderAndValueConditionNotExistsButExists()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader", "TheValue", ComparisonType.NotExists);

            Assert.IsFalse(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void HeaderAndValueConditionNotExistsButNotExists()
        {
            List<Header> headers = new List<Header>();

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader", "TheValue", ComparisonType.NotExists);

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void HeaderAndValueConditionNotExistsButNotExists2()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue2"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader", "TheValue", ComparisonType.NotExists);

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }



        [TestMethod]
        public void HeaderOnlyExistsImplicit()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader");

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void HeaderOnlyNotExists()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader", ComparisonType.NotExists);

            Assert.IsFalse(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void HeaderOnlyNotExistsImplicitNoHeaders()
        {
            List<Header> headers = new List<Header>();

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader");

            Assert.IsFalse(Evaluator.Matches(c, headers));
        }



        [TestMethod]
        public void HeaderPartialValueMatchCaseSensitive()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader", "eVal", ComparisonType.PartialValue);

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void HeaderPartialValueMatchCaseInsensitive()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader", "eVaL", ComparisonType.PartialValue | ComparisonType.CaseInsensitive);

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }
    }
}
