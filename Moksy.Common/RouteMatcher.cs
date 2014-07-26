using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common
{
    /// <summary>
    /// Helper methods for working with Routes. ie: matching /Pet/Dog/Toy/Bone against /Pet/{Kind}/Toy/{Name}
    /// </summary>
    public class RouteMatcher
    {
        /// <summary>
        /// Returns true if the pattern matches the path. 
        /// </summary>
        /// <param name="pattern">The pattern. ie: /Pet/{Kind}/Toy/{Name}</param>
        /// <param name="path">The path to match. ie: /Pet/Dog/Toy/Bone</param>
        /// <returns></returns>
        public static bool Matches(string pattern, string path)
        {
            if (null == pattern && null == path) return true;
            if (null == pattern || null == path) return false;

            var regexPattern = RouteParser.ConvertPatternToRegularExpression(pattern);
            if (null == regexPattern) return false;

            var regex = new System.Text.RegularExpressions.Regex(regexPattern);
            var matches = regex.Matches(path);
            return matches.Count > 0;
        }
    }
}
