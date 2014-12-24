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
        public Resource(string name) : this(name, false)
        {
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="name">Name of the resource. </param>
        /// <param name="isPropertyResource">If true, this resource is represented by a property. </param>
        public Resource(string name, bool isPropertyResource) : this(name, isPropertyResource, null)
        {
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="name">Name of the resource. </param>
        /// <param name="isPropertyResource">If true, this resource is represented by a property. </param>
        public Resource(string name, bool isPropertyResource, Resource owner)
        {
            this.Name = name;
            Storage = new Dictionary<string, List<Entry>>();
            Resources = new List<Resource>();
            IsPropertyResource = isPropertyResource;
            Owner = owner;
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
        private List<Entry> Data(string discriminator)
        {
            // Set as part of refactoring: the group/discriminator is now part of the path/pattern and not an individual resource. 
            discriminator = "Fixed";

            if (!Storage.ContainsKey(discriminator))
            {
                Storage[discriminator] = new List<Entry>();
            }

            return Storage[discriminator];
        }

        /// <summary>
        /// If True, this resource is represented as a property in a path and whose Name is subject to change. 
        /// If False, this resource Name is fixed. 
        /// </summary>
        /// <remarks>This is purely an implementation detail. For example: if we have a pattern like this:
        /// /Pet/{Kind}/Toy
        /// And a path like this:
        /// /Pet/Dog/Toy
        /// Then Pet is a Resource and IsPropertyResource == false;
        /// Then Dog is a Resource and IsPropertyResource == true;
        /// Then Toy is a Resource and IsPropertyResource == false;
        /// </remarks>
        public readonly bool IsPropertyResource;

        /// <summary>
        /// Name of this resource. 
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The owner resource (can be null if this is a 'top level' resource).
        /// </summary>
        public readonly Resource Owner;

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
