﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Moksy.Common
{
    /// <summary>
    /// Evaluation logic for establishing whether a particular request matches a given condition. 
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class SimulationConditionEvaluator
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public SimulationConditionEvaluator()
        {
        }

        /// <summary>
        /// Returns True if the condition matches the given method. 
        /// </summary>
        /// <param name="method">One of the HttpMethods. Such as: Get, Post</param>
        /// <returns></returns>
        public bool Matches(SimulationCondition condition, HttpMethod method)
        {
            if (condition == null) return false;

            return condition.SimulationConditionContent.HttpMethod == method;
        }

        /// <summary>
        /// Returns True if the condition matches the given path,
        /// </summary>
        /// <param name="path">Typically the Uri to match. ie: /Product or a specific item such as /Product(100)</param>
        /// <returns></returns>
        public bool Matches(SimulationCondition condition, string path)
        {
            if (condition == null) return false;

            return Matches(condition.SimulationConditionContent.Pattern, path);
        }

        /// <summary>
        /// Returns true if path matches pattern. 
        /// </summary>
        /// <param name="path">The path we want to match. ie: /Pet('Dog')</param>
        /// <param name="pattern">The path pattern set during the Simulation. ie: /Pet('{id}')</param>
        /// <returns>true - the Url is matched by path. 
        /// False - The url is not matched by path</returns>
        public bool Matches(string pattern, string path)
        {
            if (null == pattern && path == null) return true;
            if (null == pattern && path != null) return false;
            if (null != pattern && path == null) return false;

            Substitution s = new Substitution();
            var regex = RouteParser.ConvertPatternToRegularExpression(pattern);

            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(regex);
            var result = rex.Match(path);
            return result.Success;
        }

        /// <summary>
        /// Returns True if the simulation condition contains those headers. An empty (or null) headers collection will always return true.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public bool Matches(SimulationCondition condition, IEnumerable<Header> headers)
        {
            if (condition == null) return false;

            if (headers == null) return true;

            if (condition.IsGroupedByImdbHeaderDiscriminator)
            {
                // The header must exist in the request.
                var match = headers.FirstOrDefault(f => f.Name == condition.SimulationConditionContent.ImdbHeaderDiscriminator);
                if (null == match) return false;
            }

            if (headers.Count() > 0 && condition.RequestHeaders.Count == 0) return true;

            foreach (var h in condition.RequestHeaders)
            {
                bool ignoreCase = ((h.ComparisonType & ComparisonType.CaseInsensitive) != 0);
                StringComparison sc = StringComparison.InvariantCulture;
                if(ignoreCase)
                {
                    sc = StringComparison.InvariantCultureIgnoreCase;
                }
                Header match = null;
                var name = h.Name;
                if ((h.ComparisonType & ComparisonType.UrlEncode) != 0)
                {
                    name = RestSharp.Contrib.HttpUtility.UrlEncode(name);
                }

                if (h.HasValue)
                {
                    var value = h.Value;
                    if ((h.ComparisonType & ComparisonType.UrlEncode) != 0 && h.Value != null)
                    {
                        value = RestSharp.Contrib.HttpUtility.UrlEncode(value);
                    }

                    if ((h.ComparisonType & ComparisonType.PartialValue) != 0)
                    {
                        match = headers.FirstOrDefault(f => string.Compare(f.Name, name, ignoreCase) == 0 && f.Value.IndexOf(value, sc) >= 0);
                    }
                    else
                    {
                        match = headers.FirstOrDefault(f => string.Compare(f.Name, name, ignoreCase) == 0 && string.Compare(f.Value, value, ignoreCase) == 0);
                    }

                    if ((h.ComparisonType & ComparisonType.NotExists) != 0)
                    {
                        if (match != null) return false;
                        if (match == null) continue;
                    }
                    else
                    {
                        if (match != null) continue;
                        if (match == null) return false;
                    }
                }
                else
                {
                    if ((h.ComparisonType & ComparisonType.NotExists) != 0)
                    {
                        match = headers.FirstOrDefault(f => string.Compare(f.Name, name, ignoreCase) == 0);
                        if (match != null) return false;
                        if (match == null) continue;
                    }
                    else
                    {
                        match = headers.FirstOrDefault(f => string.Compare(f.Name, name, ignoreCase) == 0);
                        if (match != null) continue;
                        if (match == null) return false;
                    }
                }

            }
            return true;
        }

        /// <summary>
        /// Returns True if the simulation condition contains those headers. An empty (or null) headers collection will always return true.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public bool Matches(SimulationCondition condition, IEnumerable<Parameter> headers)
        {
            if (condition == null) return false;

            if (headers == null) return true;

            if (headers.Count() > 0 && condition.Parameters.Count == 0) return true;

            foreach (var h in condition.Parameters)
            {
                Parameter match = null;
                bool ignoreCase = ((h.ComparisonType & ComparisonType.CaseInsensitive) != 0);
                StringComparison sc = StringComparison.InvariantCulture;
                if (ignoreCase)
                {
                    sc = StringComparison.InvariantCultureIgnoreCase;
                }
                string name = h.Name;
                string value = h.Value;
                if ((h.ComparisonType & ComparisonType.UrlEncode) != 0)
                {
                    name = RestSharp.Contrib.HttpUtility.UrlEncode(name);
                    if (h.HasValue)
                    {
                        value = RestSharp.Contrib.HttpUtility.UrlEncode(value);
                    }
                }

                if (h.HasValue)
                {
                    if ((h.ComparisonType & ComparisonType.PartialValue) != 0)
                    {
                        match = headers.FirstOrDefault(f => string.Compare(f.Name, name, ignoreCase) == 0 && f.Value.IndexOf(value, sc) >= 0);
                    }
                    else
                    {
                        match = headers.FirstOrDefault(f => string.Compare(f.Name, name, ignoreCase) == 0 && string.Compare(f.Value, value, ignoreCase) == 0);
                    }
                }
                else
                {
                    match = headers.FirstOrDefault(f => string.Compare(f.Name, name, ignoreCase) == 0);
                }
                if ((h.ComparisonType & ComparisonType.NotExists) != 0)
                {
                    if (match != null) return false;
                }
                else
                {
                    if (match != null) continue;
                    if (match == null) return false;
                }

            }
            return true;
        }

        /// <summary>
        /// Matches the condition against the path and the headers.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public bool Matches(SimulationCondition condition, System.Net.Http.HttpMethod method, string path, string query, IEnumerable<Header> headers)
        {
            return Matches(condition, null, method, path, query, headers);
        }

        /// <summary>
        /// Matches the condition against the path and the headers.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="content"></param>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public bool Matches(SimulationCondition condition, HttpContent content, System.Net.Http.HttpMethod method, string path, string query, IEnumerable<Header> headers)
        {
            if (null == condition) return false;

            var match = Matches(condition, method);
            if (!match) return false;

            match = MatchesUrlParameters (condition, query);
            if (!match) return false;

            match = Matches(condition, path);
            if (!match) return false;

            match = Matches(condition, headers);
            if (!match) return false;

            if (condition.SimulationConditionContent.ContentKind != ContentKind.File)
            {
                match = MatchesBodyParameters(condition, content);
                if (!match) return false;

                match = MatchesContentRules(condition, content);
                if (!match) return false;
            }

            return true;
        }

        /// <summary>
        /// Determine if the condition (typically: the Parameters) matches the given content. 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool MatchesBodyParameters(SimulationCondition condition, HttpContent content)
        {
            if (null == content) return true;
            var bodyParameters = condition.Parameters.Where(f => f.ParameterType == ParameterType.BodyParameter);
            if (bodyParameters.Count() == 0) return true;

            try
            {
                var t = content.ReadAsStringAsync();
                t.Wait();
                var s = t.Result;
                if (null == s) return false;
                if ("" == s) return false;

                // We have converted the content to a string; now split on & and = to create our parameters list. 
                // TODO: Decode/Encode
                List<Parameter> ps = new List<Parameter>();
                var nameValuePairs = s.Split('&');
                foreach (var pair in nameValuePairs)
                {
                    var nv = pair.Split('=');
                    if (2 != nv.Length) continue;

                    ps.Add(new Parameter() { Name = nv[0], Value = nv[1], ParameterType = ParameterType.BodyParameter });
                }

                return Matches(condition, ps);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Determine if the condition (typically: the Parameters) matches the given Url parameters. 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public bool MatchesUrlParameters(SimulationCondition condition, string query)
        {
            if (null == query) query = "";
            if (query.StartsWith("?")) query = query.Substring(1);

            var bodyParameters = condition.Parameters.Where(f => f.ParameterType == ParameterType.UrlParameter);
            if (bodyParameters.Count() == 0) return true;

            try
            {
                var s = query;

                // We have converted the content to a string; now split on & and = to create our parameters list. 
                // TODO: Decode/Encode
                List<Parameter> ps = new List<Parameter>();
                var nameValuePairs = s.Split('&');
                foreach (var pair in nameValuePairs)
                {
                    var nv = pair.Split('=');
                    if (2 != nv.Length) continue;

                    ps.Add(new Parameter() { Name = nv[0], Value = nv[1], ParameterType = ParameterType.UrlParameter });
                }

                return Matches(condition, ps);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if the ContentRules match in the condition. 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool MatchesContentRules(SimulationCondition condition, HttpContent content)
        {
            if (content == null) return true;
            if (condition.ContentRules.Count == 0) return true;

            try
            {
                var t = content.ReadAsStringAsync();
                t.Wait();
                var s = t.Result;
                if (null == s) return false;

                foreach (var rule in condition.ContentRules)
                {
                    var ruleContent = rule.Content;

                    if ((rule.ComparisonType & ComparisonType.UrlEncode) != 0)
                    {
                        ruleContent = RestSharp.Contrib.HttpUtility.UrlEncode(ruleContent);
                    }

                    bool match = false;
                    if ((rule.ComparisonType & ComparisonType.CaseInsensitive) != 0)
                    {
                        match = s.ToUpper().Contains(ruleContent.ToUpper()); 
                    }
                    else
                    {
                        match = s.Contains(ruleContent);   
                    }
                    if ((rule.ComparisonType & ComparisonType.NotContains) != 0)
                    {
                        if (match) return false;
                    }
                    else
                    {
                        if (!match) return false;
                    }

                }

                return true;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Determines if the path and headers match for this simulation. This is a helper method around the Simulation.Condition property. 
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public bool Matches(Simulation simulation, System.Net.Http.HttpMethod method, string path, string query, IEnumerable<Header> headers)
        {
            return Matches(simulation, null, method, path, query, headers);
        }

        /// <summary>
        /// Determines if the path and headers match for this simulation. This is a helper method around the Simulation.Condition property. 
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="content"></param>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public bool Matches(Simulation simulation, HttpContent content, System.Net.Http.HttpMethod method, string path, string query, IEnumerable<Header> headers)
        {
            if (null == simulation) return false;

            return Matches(simulation.Condition, content, method, path, query, headers);
        }

        /// <summary>
        /// Find the first match in the given collection. 
        /// </summary>
        /// <param name="simulations">The collection of simulations. Can be null or empty. </param>
        /// <param name="path"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public Simulation Match(SimulationCollection simulations, System.Net.Http.HttpMethod method, string path, string query, IEnumerable<Header> headers)
        {
            if (null == simulations) return null;
            if (simulations.Count() == 0) return null;

            var matches = simulations.Where(f => Matches(f, method, path, query, headers) == true);
            var match = matches.FirstOrDefault();
            return match;
        }

        /// <summary>
        /// Returns all matches in the order in which they were added. 
        /// </summary>
        /// <param name="simulations">The collection of simulations. Can be null or empty. </param>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public IEnumerable<Simulation> Matches(SimulationCollection simulations, System.Net.Http.HttpMethod method, string path, string query, IEnumerable<Header> headers)
        {
            return Matches(simulations, null, method, path, query, headers);
        }

        /// <summary>
        /// Returns all matches in the order in which they were added. 
        /// </summary>
        /// <param name="simulations">The collection of simulations. Can be null or empty. </param>
        /// <param name="content"></param>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public IEnumerable<Simulation> Matches(SimulationCollection simulations, HttpContent content, System.Net.Http.HttpMethod method, string path, string query, IEnumerable<Header> headers)
        {
            IEnumerable<Simulation> matches = new List<Simulation>();
            if (null == simulations) return matches;
            if (simulations.Count() == 0) return matches;

            matches = simulations.Where(f => Matches(f, content, method, path, query, headers) == true);
            return matches;
        }
    }
}
