using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common.Swagger.Common
{
    /// <summary>
    /// Represents a single violation.
    /// </summary>
    public class Violation
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public Violation()
        {
            ViolationLevel = ViolationLevel.None;
        }

        /// <summary>
        /// The unique error code. 
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The context of the error. ie: Property Name. 
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Description of the error. 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The violation level. 
        /// </summary>
        public ViolationLevel ViolationLevel { get; set; }
    }
}
