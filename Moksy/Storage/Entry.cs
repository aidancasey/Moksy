using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Storage
{
    /// <summary>
    /// Describes an individual entry in a resource. 
    /// </summary>
    /// <remarks>A resource might have multiple content types associated with it: an Entry is a container for the different types. 
    /// </remarks>
    public class Entry
    {
        public Entry()
        {
            Resources = new List<Resource>();
        }

        /// <summary>
        /// The Json associated with this Entry. Can be null. 
        /// </summary>
        public string Json { get; set; }

        /// <summary>
        /// Optional bytes associated with this Json. 
        /// </summary>
        public byte[] Bytes { get; set; }

        /// <summary>
        /// Resources stored under this entry this database. Each resource can itself have child resources. 
        /// </summary>
        internal List<Resource> Resources;
    }
}
