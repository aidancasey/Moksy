using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Storage
{
    /// <summary>
    /// Represents an individual resource. ie: Pet
    /// </summary>
    internal class Resource
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="name">Name of the resource. </param>
        public Resource(string name)
        {
            this.Name = name;
            Data = new List<string>();
        }

        /// <summary>
        /// Name of this resource. 
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// List of entries associated with this resource (Json)
        /// </summary>
        public readonly List<string> Data;
    }
}
