using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common
{
    /// <summary>
    /// Indicates the kinds of parameters available. 
    /// </summary>
    public enum ParameterType
    {
        /// <summary>
        /// This parameter is to be matched against the Body content. 
        /// </summary>
        BodyParameter = 0,

        /// <summary>
        /// This parameter is to be matched against the Url parameters. 
        /// </summary>
        UrlParameter = 1
    }
}
