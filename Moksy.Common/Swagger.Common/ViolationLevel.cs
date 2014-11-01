using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common.Swagger.Common
{
    /// <summary>
    /// Indicates the level of the violation. 
    /// </summary>
    public enum ViolationLevel
    {
        /// <summary>
        /// None. Undefined. 
        /// </summary>
        None = 0,
        
        /// <summary>
        /// This is an informational suggestion. 
        /// </summary>
        Informational = 1,

        /// <summary>
        /// The field is required and must be fixed according to the Swagger specification. 
        /// </summary>
        Error = 2
    }
}
