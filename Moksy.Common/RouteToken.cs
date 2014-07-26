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
        /// The value of this token. 
        /// </summary>
        public string Value { get; set; }
    }
}
