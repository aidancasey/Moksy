using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common
{
    /// <summary>
    /// Determines how the comparison of any Contains operation will be performed. 
    /// </summary>
    [Flags]
    public enum ComparisonType
    {
        /// <summary>
        /// Any string comparisons are case sensitive (the default). 
        /// </summary>
        CaseSensitive = 0,

        /// <summary>
        /// Exists. 
        /// </summary>
        Exists = 0,

        /// <summary>
        /// Any string comparisons must be case insensitive. 
        /// </summary>
        CaseInsensitive = 1,
        [Obsolete(@"UrlEncoded was badly named. Use UrlEncode instead which means 'UrlEncode this value before doing the comparison")]
        UrlEncoded = 2,

        /// <summary>
        /// The value will be Urlencoded prior to being evaluated. You probably want to specify this value for all Parameter types. 
        /// </summary>
        UrlEncode = 2,

        [Obsolete(@"Use NotExists instead of NotContains for parameter comparisons. ")]
        NotContains = 4,

        /// <summary>
        /// The value must not exist. 
        /// </summary>
        NotExists = 8,

        /// <summary>
        /// Allow a partial match on the Value (not the identifier itself, such as the Header Name or Body Parameter name). The Exists and NotExists rules still apply. 
        /// </summary>
        PartialValue = 16
    }
}
