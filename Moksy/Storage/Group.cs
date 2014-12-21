using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Storage
{
    /// <summary>
    /// A Group is a logical grouping of data and is typically discriminated against use the header. This allows separate Imdb's/contexts to be built up using hte header
    /// as appropriate. This is currently a GLOBAL value that affects the end path / pattern / resource. 
    /// </summary>
    internal class Group
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public Group()
            : this("")
        { }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="name">Name of the group. Must not be null. </param>
        public Group(string name)
        {
            if (null == name) throw new System.ArgumentNullException("name");

            this.Name = name;
            Resources = new List<Resource>();
        }

        /// <summary>
        /// Name of the Group. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Resources associated with this group. 
        /// </summary>
        internal readonly List<Resource> Resources;
    }
}
