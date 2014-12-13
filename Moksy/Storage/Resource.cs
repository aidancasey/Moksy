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
            Storage = new Dictionary<string, List<Entry>>();
            Resources = new List<Resource>();
        }

        /// <summary>
        /// Return the Data for the default (null, "") resource. 
        /// </summary>
        /// <returns></returns>
        public List<Entry> Data()
        {
            return Data(null);
        }

        /// <summary>
        /// Return the data based on the given discriminator. 
        /// </summary>
        /// <param name="discriminator"></param>
        /// <returns></returns>
        public List<Entry> Data(string discriminator)
        {
            if (discriminator == null) discriminator = "";
            if (!Storage.ContainsKey(discriminator))
            {
                Storage[discriminator] = new List<Entry>();
            }

            return Storage[discriminator];
        }

        /// <summary>
        /// Name of this resource. 
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Any nested resources. 
        /// </summary>
        public readonly List<Resource> Resources;

        /// <summary>
        /// List of entries associated with this resource (Json)
        /// </summary>
        private readonly Dictionary<string, List<Entry>> Storage;
    }
}
