using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Moksy.Common
{
    /// <summary>
    /// Helper classes for extracting and replacing placeholders in strings of the form {id}
    /// </summary>
    public class Substitution
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public Substitution()
        {
        }

        /// <summary>
        /// Extract all of the variables in the string of the form {[A-Za-z0-9]}. The returned dictionary will include the name of any variables and the starting
        /// position (0-based) of the variables first {
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public List<SubstitutionVariable> GetVariables(string content)
        {
            var result = new List<SubstitutionVariable>();
            if (null == content) return result;

            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("({[A-Za-z0-9]+})+");
            var matches = regex.Matches(content);
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                if (match.Success)
                {
                    foreach (Group g in match.Groups)
                    {
                        var key = g.Value.Replace("{", "").Replace("}", "");
                        var existing = result.FirstOrDefault(f => f.Name == key && f.Position == g.Index);
                        if (existing != null) continue;

                        SubstitutionVariable v = new SubstitutionVariable() { Name = key, Position = g.Index };
                        result.Add(v);
                    }
                }
            }

            return result;
        }



        /// <summary>
        /// Will replace each instance of pair.Key with pair.Value. The match is case insensitive. 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public string Substitute(string content, Dictionary<string, string> pairs)
        {
            if (null == content) return content;
            if (null == pairs) return content;
            if (pairs.Count == 0) return content;

            var result = content;

            var vars = GetVariables(content);
            if (vars.Count == 0) return result;

            // ASSERTION: We now need to substitute the placeholders with the new values. 
            List<string> components = new List<string>();
            int startIndex = 0;
            foreach (var var in vars)
            {
                var c = result.Substring(startIndex, var.Position - startIndex);
                components.Add(c);
                startIndex += c.Length;

                if (pairs.ContainsKey(var.Name))
                {
                    components.Add(pairs[var.Name]);
                }
                else
                {
                    components.Add(string.Format("{{{0}}}", var.Name));
                }
                startIndex += var.Name.Length + 2;
            }
            components.Add(result.Substring(startIndex));

            result = string.Join("", components);

            return result;
        }



        /// <summary>
        /// Returns true if this string has any placeholders. False otherwise. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool HasVariables(string value)
        {
            return GetVariables(value).Count > 0;
        }
    }
}
