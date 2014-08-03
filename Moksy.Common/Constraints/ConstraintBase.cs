using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common.Constraints
{
    /// <summary>
    /// Base class for all constraints. 
    /// </summary>
    /// <remarks>Base class for all constraints on properties. 
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
        /// Evaluate the constraint against the given object. 
        /// </summary>
        /// <param name="jobject"></param>
        /// <returns></returns>
        internal bool EvaluateRule(JObject jobject)
        {
            if (null == jobject) throw new System.ArgumentNullException("jobject");

            return Evaluate(jobject);
        }

        /// <summary>
        /// Implemented by all Rules. This method will only be called with a value jobject. 
        /// </summary>
        /// <param name="jobject">The Json object. </param>
        /// <returns></returns>
        public abstract bool Evaluate(JObject jobject);
    }
}
