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
        CaseSensitive = 0,
        CaseInsensitive = 1,
        UrlEncoded = 2
    }
}
