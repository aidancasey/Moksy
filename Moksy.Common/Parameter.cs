using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common
{
    /// <summary>
    /// Specifies a parameter condition that must be matched. 
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public Parameter()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Parameter name. Must not be null. </param>
        /// <param name="value">Value. Can be null. </param>
        public Parameter(string name, string value)
        {
            if (null == name) throw new System.ArgumentNullException("name");

            this.Name = name;
            this.Value = value;
            this.ParameterType = ParameterType.BodyParameter;
        }

        /// <summary>
        /// Parameter Name. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parameter Value. 
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The type of parameter. 
        /// </summary>
        public ParameterType ParameterType { get; set; }
    }
}
