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
    public class ConditionEvaluatorTests
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public ConditionEvaluatorTests()
        {
        }

        [TestInitialize]
        public void Initialize()
        {
            Evaluator = new SimulationConditionEvaluator();
        }

        SimulationConditionEvaluator Evaluator;



        [TestMethod]
        public void PathMatchesNullDefault()
        {
            SimulationCondition c = new SimulationCondition();
            
            Assert.IsTrue(Evaluator.Matches(c, (string) null));
        }

        [TestMethod]
        public void PathDoesNotMatchEmpty()
        {
            SimulationCondition c = new SimulationCondition();
            
            Assert.IsFalse(Evaluator.Matches(c, ""));
        }

        [TestMethod]
        public void PathDoesNotMatchNonsense()
        {
            SimulationCondition c = new SimulationCondition();

            Assert.IsFalse(Evaluator.Matches(c, "NonSense"));
        }

        [TestMethod]
        public void PathMatchesCaseSensitive()
        {
            SimulationCondition c = new SimulationCondition() { Pattern = "/Product" };

            Assert.IsTrue(Evaluator.Matches(c, "/Product"));
        }

        [TestMethod]
        public void PathDoesNotMatchCaseSensitive()
        {
            SimulationCondition c = new SimulationCondition() { Pattern = "/Product" };

            Assert.IsFalse(Evaluator.Matches(c, "/PRodUCt"));
        }


        [TestMethod]
        public void MethodDoesNotMatch()
        {
            SimulationCondition c = new SimulationCondition() { HttpMethod = System.Net.Http.HttpMethod.Post };
            Assert.IsFalse(Evaluator.Matches(c, System.Net.Http.HttpMethod.Get));
        }

        [TestMethod]
        public void MethodMatches()
        {
            SimulationCondition c = new SimulationCondition() { HttpMethod = System.Net.Http.HttpMethod.Get };
            Assert.IsTrue(Evaluator.Matches(c, System.Net.Http.HttpMethod.Get));
        }



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
        public void OneConditionMatchesAgainstTwo()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue"));
            headers.Add(new Header("TheHeader2", "TheValue2"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader", "TheValue");

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
            c.Header("TheH/eader", ComparisonType.Exists | ComparisonType.UrlEncoded);

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void OneHeaderAndValueOnlyUrlEncodedMatch()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheH%2feader", "TheV%falue"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheH/eader", "TheV/alue", ComparisonType.Exists | ComparisonType.UrlEncoded);

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
            c.Header("TheHeader", "TheValue", Persistence.NotExists);

            Assert.IsFalse(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void HeaderAndValueConditionNotExistsButNotExists()
        {
            List<Header> headers = new List<Header>();

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader", "TheValue", Persistence.NotExists);

            Assert.IsTrue(Evaluator.Matches(c, headers));
        }

        [TestMethod]
        public void HeaderAndValueConditionNotExistsButNotExists2()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("TheHeader", "TheValue2"));

            SimulationCondition c = new SimulationCondition();
            c.Header("TheHeader", "TheValue", Persistence.NotExists);

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
            c.Header("TheHeader", Persistence.NotExists);

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



        #region BodyParameters

        // Body Parameters are passed as a=b&c=d in the Body of the request. 
        [TestMethod]
        public void NoBodyParametersMatches()
        {
            List<Parameter> ps = new List<Parameter>();

            SimulationCondition c = new SimulationCondition();

            Assert.IsTrue(Evaluator.Matches(c, ps));
        }

        [TestMethod]
        public void ExactlyOneParameterMatch()
        {
             List<Parameter> ps = new List<Parameter>();
             ps.Add(new Parameter("thename", "thevalue"));

            SimulationCondition c = new SimulationCondition();
            c.Parameter("thename", "thevalue");

            Assert.IsTrue(Evaluator.Matches(c, ps));
        }

        [TestMethod]
        public void OneParameterButNotAMatch()
        {
            List<Parameter> ps = new List<Parameter>();
            ps.Add(new Parameter("thename2", "thevalue2"));

            SimulationCondition c = new SimulationCondition();
            c.Parameter("thename", "thevalue");

            Assert.IsFalse(Evaluator.Matches(c, ps));
        }

        [TestMethod]
        public void OneConditionButNoParameters()
        {
            List<Parameter> ps = new List<Parameter>();

            SimulationCondition c = new SimulationCondition();
            c.Parameter("thename", "thevalue");

            Assert.IsFalse(Evaluator.Matches(c, ps));
        }

        [TestMethod]
        public void OneParameterButNoCondition()
        {
            List<Parameter> ps = new List<Parameter>();
            ps.Add(new Parameter("thename2", "thevalue2"));

            SimulationCondition c = new SimulationCondition();

            Assert.IsTrue(Evaluator.Matches(c, ps));
        }

        [TestMethod]
        public void ParameterExistsValueIgnored()
        {
            List<Parameter> ps = new List<Parameter>();
            ps.Add(new Parameter("thename2"));

            SimulationCondition c = new SimulationCondition();
            c.Parameter("thename", "thevalue");

            Assert.IsFalse(Evaluator.Matches(c, ps));
        }

        [TestMethod]
        public void ParameterExistsValueIgnored2()
        {
            List<Parameter> ps = new List<Parameter>();
            ps.Add(new Parameter("thename", "thevalue"));

            SimulationCondition c = new SimulationCondition();
            c.Parameter("thename");

            Assert.IsTrue(Evaluator.Matches(c, ps));
        }

        #endregion

        #region Parameters - With Content

        [TestMethod]
        public void NullContentAlwaysMatches()
        {
            // Will always match
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();

            Assert.IsTrue(e.MatchesParameters(c, (System.Net.Http.HttpContent) null));
        }

        [TestMethod]
        public void EmptyContentAlwaysMatches()
        {
            // Will always match
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("", Encoding.UTF8, "application/json");

            Assert.IsTrue(e.MatchesParameters(c, content));
        }

        [TestMethod]
        public void ContentWithPairMatchesBecauseNoParameters()
        {
            // Will always match
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("a=b", Encoding.UTF8, "application/json");

            Assert.IsTrue(e.MatchesParameters(c, content));
        }

        [TestMethod]
        public void ContentWithMatchingPairMatches()
        {
            // Will always match
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Parameters.Add(new Parameter("a", "b"));

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("a=b", Encoding.UTF8, "application/json");

            Assert.IsTrue(e.MatchesParameters(c, content));
        }

        [TestMethod]
        public void ContentWithNoMatchingPairMatches()
        {
            // Will always match
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Parameter("c", "d");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("a=b", Encoding.UTF8, "application/json");

            Assert.IsFalse(e.MatchesParameters(c, content));
        }

        [TestMethod]
        public void ContentWithOneOfTwoMatchingPairMatches()
        {
            // Will always match
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Parameter("c", "d");
            c.Parameter("a", "b");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("a=b", Encoding.UTF8, "application/json");

            Assert.IsFalse(e.MatchesParameters(c, content));
        }

        [TestMethod]
        public void ContentWithTwoExactMatchingPairMatches()
        {
            // Will always match
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Parameter("c", "d");
            c.Parameter("a", "b");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("a=b&c=d", Encoding.UTF8, "application/json");

            Assert.IsTrue(e.MatchesParameters(c, content));
        }

        [TestMethod]
        public void ContentWithTwoOfThreeMatchingPairMatches()
        {
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Parameter("c", "d");
            c.Parameter("a", "b");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("a=b&c=d&e=f", Encoding.UTF8, "application/json");

            Assert.IsTrue(e.MatchesParameters(c, content));
        }

        [TestMethod]
        public void TwoConditionsSecondOneMatches()
        {
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c1 = new SimulationCondition();
            c1.Parameter("c", "d");
            c1.Parameter("g", "h");

            SimulationCondition c2 = new SimulationCondition();
            c2.Parameter("c", "d");
            c2.Parameter("e", "f");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("a=b&c=d&e=f", Encoding.UTF8, "application/json");

            Assert.IsFalse(e.MatchesParameters(c1, content));
            Assert.IsTrue(e.MatchesParameters(c2, content)); 
        }

        [TestMethod]
        public void ContentWithTwoMatchesButOneNotMatching()
        {
            // Will always match
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Parameter("c", "d");
            c.Parameter("a", "b");
            c.Parameter("m", "n");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("a=b&c=d&e=f", Encoding.UTF8, "application/json");

            Assert.IsFalse(e.MatchesParameters(c, content));
        }

        #endregion

        #region Contains

        [TestMethod]
        public void NoContentRulesAlwaysEvaluatesAsTrue()
        {
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("something", Encoding.UTF8, "application/json");

            Assert.IsTrue(e.MatchesContentRules(c, content));
        }

        [TestMethod]
        public void EmptyContentRulesAlwaysEvaluatesAsTrue()
        {
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Contains("");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("something", Encoding.UTF8, "application/json");

            Assert.IsTrue(e.MatchesContentRules(c, content));
        }

        [TestMethod]
        public void OneContentRulesMatches()
        {
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Contains("thin");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("something", Encoding.UTF8, "application/json");

            Assert.IsTrue(e.MatchesContentRules(c, content));
        }

        [TestMethod]
        public void OneContentRuleDoesNotMatch()
        {
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Contains("nothing");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("something", Encoding.UTF8, "application/json");

            Assert.IsFalse(e.MatchesContentRules(c, content));
        }

        [TestMethod]
        public void OneContentRuleMatchesOneDoesNotMatch()
        {
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Contains("nothing");
            c.Contains("something");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("some", Encoding.UTF8, "application/json");

            Assert.IsFalse(e.MatchesContentRules(c, content));
        }

        [TestMethod]
        public void TwoMatches()
        {
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Contains("some");
            c.Contains("thing");

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("something", Encoding.UTF8, "application/json");

            Assert.IsTrue(e.MatchesContentRules(c, content));
        }

        [TestMethod]
        public void ContainsCaseSensitiveMatch()
        {
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Contains("some", true);

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("SOMEthing", Encoding.UTF8, "application/json");

            Assert.IsFalse(e.MatchesContentRules(c, content)); 
        }

        [TestMethod]
        public void ContainsCaseSensitiveMatch2()
        {
            SimulationConditionEvaluator e = new SimulationConditionEvaluator();
            SimulationCondition c = new SimulationCondition();
            c.Contains("some", false);

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("SOMEthing", Encoding.UTF8, "application/json");

            Assert.IsTrue(e.MatchesContentRules(c, content));
        }

        #endregion
    }
}
