using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common
{
    /// <summary>
    /// Describes the type of tokens that can exist for a route / Uri. 
    /// </summary>
    public enum RouteTokenKind
    {
        /// <summary>
        /// This is a resource. 
        /// </summary>
        Resource = 0,

        /// <summary>
        /// This is a property reference. 
        /// </summary>
        Property = 1
    }
}
