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
            HasValue = false;
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="name">Name of the header that must exist. </param>
        public Header(string name) : this(name, Common.Persistence.Exists)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the header. </param>
        /// <param name="persistence">Indicates whether the header should or should not exist. </param>
        public Header(string name, Common.Persistence persistence)
        {
            Name = name;
            Persistence = persistence;
            HasValue = false;
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Header(string name, string value) : this(name, value, Common.Persistence.Exists)
        {
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="name">Name of the header. Must not be null. </param>
        /// <param name="value">The value of the header. </param>
        /// <param name="persistence">Indicates whether the header should exist. </param>
        public Header(string name, string value, Moksy.Common.Persistence persistence)
        {
            if (null == name) throw new System.ArgumentNullException("name");

            Name = name;
            Value = value;
            Persistence = persistence;
            HasValue = true;
        }

        public string Name {get;set;}
        public string Value {get;set;}
        public Moksy.Common.Persistence Persistence { get; set; }
        public bool HasValue { get; set; }
    }
}
