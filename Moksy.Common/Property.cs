using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common
{
    /// <summary>
    /// Represents an individual Property.
    /// </summary>
    /// <remarks>Properties can be set in an object as part of a response. 
    /// </remarks>
    public class Property
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public Property()
        {
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="name">Property name. Must not be null. </param>
        /// <param name="value">Property value. Can contain placeholders (variables) that will be expanded as part of a response. </param>
        public Property(string name, string value)
        {
            if (null == name) throw new System.ArgumentNullException("name");

            Name = name;
            Value = value;
        }

        /// <summary>
        /// Property name. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value. Can be null or contain placeholders
        /// </summary>
        public string Value { get; set; }
    }
}
