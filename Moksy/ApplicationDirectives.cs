using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy
{
    /// <summary>
    /// Parameters and context passed through to the Application from the command line. 
    /// </summary>
    public class ApplicationDirectives
    {
        public ApplicationDirectives()
        {
        }

        /// <summary>
        /// If true, all requests will be output. 
        /// </summary>
        public bool Log { get; set; }
    }
}
