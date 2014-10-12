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
        public ContentRule()
        {
        }

        public ContentRule(string content)
        {
            this.Content = content;
        }

        public string Content { get; set; }
    }
}
