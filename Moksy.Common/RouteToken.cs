using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common
{
    /// <summary>
    /// Represents an individual token in a route. 
    /// </summary>
    public class RouteToken
    {
        public RouteToken()
        {
        }

        /// <summary>
        /// The kind of token. 
        /// </summary>
        public RouteTokenKind Kind { get; set; }

        /// <summary>
        /// The name of the token. If this token is a Resource, the Name and Value are the same.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of this token. If this token is a Resource, the Name and Value are the same. 
        /// </summary>
        public string Value { get; set; }
    }
}
