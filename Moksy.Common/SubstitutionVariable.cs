using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common
{
    /// <summary>
    /// Describes an individual substituion variable.
    /// </summary>
    public class SubstitutionVariable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public SubstitutionVariable()
        {
            Name = "";
        }

        public string Name { get; set; }
        public int Position { get; set; }
    }
}
