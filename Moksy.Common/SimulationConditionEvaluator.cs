using System;
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

            return condition.HttpMethod == method;
        }

        /// <summary>
        /// Returns True if the condition matches the given path,
        /// </summary>
        /// <param name="path">Typically the Uri to match. ie: /Product or a specific item such as /Product(100)</param>
        /// <returns></returns>
        public bool Matches(SimulationCondition condition, string path)
        {
            if (condition == null) return false;

            return Matches(condition.Path, path);
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
            var regex = string.Format("{0}$",s.ConvertPatternToRegularExpression(pattern));

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
            if (headers.Count() == 0) return true;
            if (headers.Count() > 0 && condition.RequestHeaders.Count == 0) return true;

            foreach (var h in condition.RequestHeaders)
            {
                var match = headers.FirstOrDefault(f => f.Name == h.Name && f.Value == h.Value);
                if (match == null) return false;
            }

            return true;
        }

        /// <summary>
        /// Matches the condition against the path and the headers.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="path"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public bool Matches(SimulationCondition condition, System.Net.Http.HttpMethod method, string path, IEnumerable<Header> headers)
        {
            if (null == condition) return false;

            var match = Matches(condition, method);
            if (!match) return false;

            match = Matches(condition, path);
            if (!match) return false;

            match = Matches(condition, headers);
            if (!match) return false;

            return true;
        }

        /// <summary>
        /// Determines if the path and headers match for this simulation. This is a helper method around the Simulation.Condition property. 
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="path"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public bool Matches(Simulation simulation, System.Net.Http.HttpMethod method, string path, IEnumerable<Header> headers)
        {
            if (null == simulation) return false;

            return Matches(simulation.Condition, method, path, headers);
        }

        /// <summary>
        /// Find the first match in the given collection. 
        /// </summary>
        /// <param name="simulations">The collection of simulations. Can be null or empty. </param>
        /// <param name="path"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public Simulation Match(SimulationCollection simulations, System.Net.Http.HttpMethod method, string path, IEnumerable<Header> headers)
        {
            if (null == simulations) return null;
            if (simulations.Count() == 0) return null;

            var match = simulations.FirstOrDefault(f => Matches(f, method, path, headers) == true);
            return match;
        }

        /// <summary>
        /// Returns all matches in the order in which they were added. 
        /// </summary>
        /// <param name="simulations">The collection of simulations. Can be null or empty. </param>
        /// <param name="path"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public IEnumerable<Simulation> Matches(SimulationCollection simulations, System.Net.Http.HttpMethod method, string path, IEnumerable<Header> headers)
        {
            IEnumerable<Simulation> matches = new List<Simulation>();
            if (null == simulations) return matches;
            if (simulations.Count() == 0) return matches;

            matches = simulations.Where(f => Matches(f, method, path, headers) == true);
            return matches;
        }
    }
}
