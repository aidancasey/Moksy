using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common
{
    /// <summary>
    /// Indicates a condition to confirm whether an object exists or not. 
    /// </summary>
    public enum Persistence
    {
        /// <summary>
        /// Undefined. Has no effect. 
        /// </summary>
        None = 0,

        /// <summary>
        /// The referenced entity exists. 
        /// </summary>
        Exists = 1,

        /// <summary>
        /// The referenced entity does not exist. 
        /// </summary>
        NotExists = 2
    }
}
