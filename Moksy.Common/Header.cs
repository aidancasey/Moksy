using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy
{
    /// <summary>
    /// Describes a single HTTP Request / Response header.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class Header
    {
        /// <summary>
        /// Constructor (for Json.Net deserialization). 
        /// </summary>
        public Header()
        {
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Header(string name, string value)
        {
            if (null == name) throw new System.ArgumentNullException("name");

            Name = name;
            Value = value;
        }

        public string Name {get;set;}
        public string Value {get;set;}
    }
}
