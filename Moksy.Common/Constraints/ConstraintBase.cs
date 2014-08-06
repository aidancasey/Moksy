using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common.Constraints
{
    /// <summary>
    /// Base class for all assertions. 
    /// </summary>
    /// <remarks>Base class for all assertions on properties. 
    /// </remarks>
    public abstract class ConstraintBase
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public ConstraintBase()
        {
        }

        /// <summary>
        /// Evaluate the assertion against the given object. 
        /// </summary>
        /// <param name="jobject"></param>
        /// <returns></returns>
        internal bool EvaluateRule(JObject jobject)
        {
            if (null == jobject) throw new System.ArgumentNullException("jobject");

            return Evaluate(jobject);
        }

        /// <summary>
        /// Evaluate the response string based on this object. 
        /// </summary>
        /// <returns></returns>
        public virtual string GetState(Newtonsoft.Json.Linq.JObject jobject)
        {
            return null;
        }

        /// <summary>
        /// Get the value of the property in a string-friendly way. This will be null if the value is null; null if the property is missing; or will contain a quote-wrapped
        /// value if the property has a real value.
        /// </summary>
        /// <param name="j">The parsed Json object. </param>
        /// <param name="propertyName">Name of the property to retrieve. </param>
        /// <returns></returns>
        public virtual string GetValue(JObject j, string propertyName)
        {
            if (j == null) return "null";
            var result = j[propertyName];
            if (result == null)
            {
                return "null";
            }
            if (result.Type.ToString() == "Null") return "null";
            if (result.Type.ToString() == "String") return string.Format(@"""{0}""", result.ToString());
            return result.ToString();
        }

        /// <summary>
        /// Implemented by all Rules. This method will only be called with a value jobject. 
        /// </summary>
        /// <param name="jobject">The Json object. </param>
        /// <returns></returns>
        public abstract bool Evaluate(JObject jobject);

        /// <summary>
        /// Contains the fully evaluated response for this object. TODO: Refactor. 
        /// </summary>
        public string EvaluatedResponse { get; set; }
    }
}
