using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common
{
    /// <summary>
    /// Configures how content will be evaluated. 
    /// </summary>
    public class ContentRule
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public ContentRule()
        {
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="content">The case sensitive content to match. Null and the empty string will always match. </param>
        public ContentRule(string content) : this(content, true)
        {
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="comparison"></param>
        public ContentRule(string content, ComparisonType comparison)
        {
            this.Content = content;
            this.ComparisonType = comparison;
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="content">The content to match. </param>
        /// <param name="caseSensitive">Determines if the check is case sensitive. </param>
        public ContentRule(string content, bool caseSensitive)
        {
            this.Content = content;
            this.ComparisonType = caseSensitive ? ComparisonType.CaseSensitive : ComparisonType.CaseInsensitive;
        }

        public string Content { get; set; }

        public ComparisonType ComparisonType { get; set; }
    }
}
