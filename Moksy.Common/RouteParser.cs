using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Moksy.Common
{
    /// <summary>
    /// Parses a path pattern of the form /Route. The path should not include the host or port. 
    /// </summary>
    public class RouteParser
    {
        /// <summary>
        /// Get the first resource from the path. 
        /// </summary>
        /// <param name="path">The path. ie: /Pet/Dog</param>
        /// <param name="pattern">The pattern. ie: /Pet/{Kind}</param>
        /// <returns></returns>
        public static string GetFirstResource(string path, string pattern)
        {
            var result = Parse(path, pattern);
            var match = result.FirstOrDefault(f => f.Kind == RouteTokenKind.Resource);
            if (null == match) return null;
            return match.Value;
        }

        /// <summary>
        /// Parse the pattern and extract the tokens. 
        /// </summary>
        /// <param name="pattern">The path / pattern (typically from a condition). ie: /Pet/{Kind}</param>
        /// <returns>The parsed route. ie: Pet, Kind</returns>
        public static IEnumerable<RouteToken> Parse(string pattern)
        {
            List<RouteToken> result = new List<RouteToken>();
            if (null == pattern) return result;
            if (pattern == "")
            {
                RouteToken token = new RouteToken() { Kind = RouteTokenKind.Resource, Value = "" };
                result.Add(token);

                return result;
            }

            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(RegularExpression);
            var matches = r.Matches(pattern);
            if (matches.Count == 0) return result;

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                int offset = 1;

                while (offset < match.Groups.Count)
                {
                    // var resourceName = m.Groups["resourceName"];
                    // var resourceIdentifier = m.Groups["resourceIdentifier"];
                    // var tail = m.Groups["tail"];
                    string resourceName = "";
                    string resourceIdentifier = "";

                    if (offset != match.Groups.Count)
                    {
                        resourceName = match.Groups[offset].Value;

                        RouteToken token = new RouteToken() { Kind = RouteTokenKind.Resource, Value = SanitizeResource(resourceName) };
                        result.Add(token);

                        // Edge case: A fixed /Resource is passed in. In this case, if the resource is the same as the pattern, we are done. 
                        if (resourceName == match.Value)
                        {
                            break;
                        }

                        offset++;
                    }
                    if (offset != match.Groups.Count)
                    {
                        resourceIdentifier = match.Groups[offset].Value;

                        RouteToken token = new RouteToken() { Kind = RouteTokenKind.Property, Value = resourceIdentifier.Replace("{", "").Replace("}", "") };
                        result.Add(token);

                        offset++;
                    }
                    if (offset != match.Groups.Count)
                    {
                        // Tail. 
                        offset++;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Parse a (relative) path against a pattern and extract the various tokens. 
        /// </summary>
        /// <param name="path">The actual path. For example: /Pet(Dog)/Toy(Bone)</param>
        /// <param name="pattern">The pattern. ie: /Pet({Kind})/Toy({Name}</param>
        /// <returns>A list of individual tokens. For example: Pet; Kind; Toy; Name</returns>
        public static IEnumerable<RouteToken>  Parse(string path, string pattern)
        {
            List<RouteToken> result = new List<RouteToken>();
            if (null == path || null == pattern) return result;

            if (path == pattern)
            {
                RouteToken token = new RouteToken() { Kind = RouteTokenKind.Resource, Value = SanitizeResource(path) };
                token.Name = token.Value;
                result.Add(token);

                return result;
            }

            var regex = ConvertPatternToRegularExpression(pattern);

            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(regex);
            var pathMatches = r.Matches(path);
            if (pathMatches.Count != 1) return result;

            System.Text.RegularExpressions.Regex r2 = new Regex(regex);
            var patternMatches = r2.Matches(pattern);
            if (patternMatches.Count != 1) return result;
            
            // There should be a single match. 
            // The Groups will be of the form: Full
            //                                 ResourceName
            //                                 ResourceIdentifier
            //                                 Tail
            if (pathMatches[0].Groups.Count == 1) return result;

            var full = pathMatches[0].Groups[0].Value;

            int offset = 1;

            // The matches will be in groups of three because of the regular expression: 
            // resourceName
            // resourceIdentifier
            // tail
            while (offset < pathMatches[0].Groups.Count)
            {
                // var resourceName = m.Groups["resourceName"];
                // var resourceIdentifier = m.Groups["resourceIdentifier"];
                // var tail = m.Groups["tail"];
                string resourceName = "";
                string resourceIdentifier = "";
                string tail = "";

                if (offset != pathMatches[0].Groups.Count)
                {
                    resourceName = pathMatches[0].Groups[offset].Value;

                    RouteToken token = new RouteToken() { Kind = RouteTokenKind.Resource, Value = SanitizeResource(resourceName) };
                    token.Name = token.Value;
                    result.Add(token);

                    offset++;
                }
                if (offset != pathMatches[0].Groups.Count)
                {
                    resourceIdentifier = pathMatches[0].Groups[offset].Value;

                    RouteToken token = new RouteToken() { Kind = RouteTokenKind.Property, Value = resourceIdentifier };
                    token.Name = patternMatches[0].Groups[offset].Value.Replace("{", "").Replace("}", "");
                    result.Add(token);

                    offset++;
                }
                if (offset != pathMatches[0].Groups.Count)
                {
                    // Tail. 
                    offset++;
                }
            }

            return result;
        }

        /// <summary>
        /// Will match the URL. 
        /// </summary>
        /// <remarks>Constructing this was one of the most memorable weeks of my life. 
        /// </remarks>
        public const string RegularExpression = "(?<resourceName>/[^{]+)(?<resourceIdentifier>[{][^}]+[}])(?<tail>[^/]*)|(?<resourceName>/[^{]+)|(?<resourceIdentifier>/[{][^}]+[}])(?<tail>[^/]*)";

        /// <summary>
        /// Convert a pattern - such as /Pet/{Kind}/Toy/{Name} to a regular expression to help matching against real paths. 
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string ConvertPatternToRegularExpression(string pattern)
        {
            return ConvertPatternToRegularExpression(pattern, false);
        }

        /// <summary>
        /// Convert a pattern - such as /Pet/{Kind}/Toy/{Name} to a regular expression to help matching against real paths. 
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="nameCaptures">If true, the regular expression will name the capture groups. </param>
        /// <returns></returns>
        public static string ConvertPatternToRegularExpression(string pattern, bool nameCaptures)
        {
            if (null == pattern) return pattern;

            string resourceName = "?<resourceName>";
            string resourceIdentifier = "?<resourceIdentifier>";

            if (!nameCaptures)
            {
                resourceName = "";
                resourceIdentifier = "";
            } 
            
            Substitution s = new Substitution();

            var vars = s.GetVariables(pattern);
            if (vars.Count == 0)
            {
                if (nameCaptures)
                {
                    return string.Format("^?<resourceName>{0}$", pattern);
                }

                return string.Format("^{0}$", pattern);
            }

            // NOTE: Currently only matches the first {variable}.
            //       We need to replace the {variable} with a regular expression that will match. 
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(RegularExpression);
            var matches = r.Matches(pattern);

            StringBuilder b = new StringBuilder();
            foreach (System.Text.RegularExpressions.Match m in matches)
            {
                vars = s.GetVariables(m.Value);
                if (vars.Count == 0)
                {
                    // This is entirely a resourceName - a fixed value.
                    b.AppendFormat(string.Format("({1}{0})", Regex.Escape(m.Value), resourceName));
                    continue;
                }
                
                // This includes variables. In particular: /Pet/{Kind} 
                foreach (var v in vars)
                {
                    var fullVariable = string.Format("{{{0}}}", v.Name);
                    var start = m.Value.Substring(0, v.Position);
                    start = Regex.Escape(start);
                    if (nameCaptures && start != "/")
                    {
                        start = resourceName + start;
                    }
                    var regex = ".*?";
                    if (nameCaptures)
                    {
                        regex = resourceIdentifier + regex;
                    }
                    var end = m.Value.Substring(v.Position + fullVariable.Length);

                    b.AppendFormat("({0})({1})({2})", start, regex, Regex.Escape(end));
                }
            }
            // By doing this, we get separate tokens. 
            var result = string.Format("^{0}$", b.ToString());
            return result;
        }

        /// <summary>
        /// Strip characters from the resource name. 
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        protected static string SanitizeResource(string resource)
        {
            // TODO: Improve this. 
            if (null == resource) return resource;

            return resource.Replace("/", "").Replace("(", "").Replace("'", "");
        }
    }
}
