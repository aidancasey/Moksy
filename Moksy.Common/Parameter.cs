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
            this.HasValue = false;
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
            this.HasValue = true;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Parameter name. Must not be null. </param>
        /// <param name="value">Value. Can be null. </param>
        /// <param name="comparison">How to compare. </param>
        public Parameter(string name, string value, ComparisonType comparison) : this(name,value)
        {
            ComparisonType = comparison;
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="name">Property value. </param>
        /// <param name="comparison">Is the name encoded before being compared in </param>
        public Parameter(string name, ComparisonType comparison) : this(name)
        {
            this.ComparisonType = comparison;
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="name">Property value. </param>
        /// <param name="comparison">Is the name encoded before being compared in </param>
        public Parameter(string name, ComparisonType comparison, ParameterType parameterType)
            : this(name)
        {
            this.ParameterType = parameterType;
            this.ComparisonType = comparison;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Parameter name. Must not be null. </param>
        public Parameter(string name)
        {
            if (null == name) throw new System.ArgumentNullException("name");

            this.Name = name;
            this.ParameterType = ParameterType.BodyParameter;
            this.HasValue = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Parameter name. Must not be null. </param>
        public Parameter(string name, ParameterType parameterType)
        {
            if (null == name) throw new System.ArgumentNullException("name");

            this.Name = name;
            this.ParameterType = parameterType;
            this.HasValue = false;
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
        /// If true, this Parameter has an expected value. If false, the 
        /// </summary>
        public bool HasValue { get; set; }

        /// <summary>
        /// The type of parameter. 
        /// </summary>
        public ParameterType ParameterType { get; set; }

        /// <summary>
        /// Determines how the Name and Value will be compared. 
        /// </summary>
        public ComparisonType ComparisonType { get; set; }
    }
}
