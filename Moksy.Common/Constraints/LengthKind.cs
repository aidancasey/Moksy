using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common.Constraints
{
    /// <summary>
    /// Describes the length evaluation kind. 
    /// </summary>
    public enum LengthKind
    {
        Equals = 0,
        LessThan = 1,
        GreaterThan = 2,
        LessThanOrGreaterThan = 3,
        NotEquals = 4
    }
}
