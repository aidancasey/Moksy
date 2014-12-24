using Moksy.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Storage
{
    /// <summary>
    /// Used for storing in-memory databases. 
    /// </summary>
    internal class Database
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public Database()
        {
            Groups = new List<Group>();
        }

        public bool AddJson(string path, string pattern, string propertyName, string data)
        {
            return AddJson(path, pattern, propertyName, data, new byte[0], null);
        }

        public bool AddJson(string path, string pattern, string propertyName, string data, string discriminator)
        {
            return AddJson(path, pattern, propertyName, data, new byte[0], discriminator);
        }

        /// <summary>
        /// Add an entry to the database for the given path and pattern. That data will be validated as Json. 
        /// </summary>
        /// <param name="path">The path. ie: /Pet, /Pet/Dog</param>
        /// <param name="pattern">ie: /Pet, /Pet/{Kind}</param>
        /// <param name="propertyName">The property name (and its value) to use as the unique identifier in the object. Both the property name and contents are case sensitive. </param>
        /// <param name="data">The data to add. Can be null. </param>
        /// <param name="discriminator">Optional discriminator if multiple databases are allowed (ie: via the header)</param>
        /// <returns>true - Added; false - not added. </returns>
        public bool AddJson(string path, string pattern, string propertyName, string data, byte[] binaryContent, string discriminator)
        {
            if (discriminator == null) discriminator = "";

            var tokens = RouteParser.Parse(path, pattern).ToArray();
            if (tokens.Count() == 0) return false;

            JObject jobject = JsonConvert.DeserializeObject(data) as JObject;
            
            // 1. We need to 'walk' the path (and therefore the database) until we get to the Resource that will hold our value. 
            //    As we walk the path, we create the resources we need to store the data as we go. 
            // 2. Add the entry to the database. 


            // 1.
            List<Resource> currentResourceList = GetResources(discriminator);
            Resource resource = null;
            string propertyValue = null;
            JToken jobjectPropertyValue = null;
            Resource owner = null;
            for (int offset = 0; offset < tokens.Length; offset++)
            {
                propertyValue = null;
                jobjectPropertyValue = null;

                var token = tokens[offset];
                if (token.Kind == RouteTokenKind.Resource)
                {
                    resource = currentResourceList.FirstOrDefault(f => string.Compare(token.Name, f.Name, false) == 0 && !f.IsPropertyResource);
                    if (resource == null)
                    {
                        resource = new Resource(token.Name, false, owner);
                        currentResourceList.Add(resource);
                    }
                    owner = resource;
                    currentResourceList = resource.Resources;
                    continue;
                }
                if (token.Kind == RouteTokenKind.Property)
                {
                    if (offset == tokens.Length - 1)
                    {
                        // The property at the end of the path is assumed to reference an existing object; so we are finished "walking"
                        continue;

                    }
 
                    var existingPropertyResourse = resource.Resources.FirstOrDefault(f => string.Compare(f.Name, token.Value, false) == 0 && f.IsPropertyResource);
                    if (existingPropertyResourse == null)
                    {
                        var nr = new Resource(token.Value, true, owner);
                        resource.Resources.Add(nr);

                        var jsonRaw = JsonConvert.DeserializeObject("{}") as JObject;
                        jsonRaw[token.Name] = token.Value;
                        var json = JsonConvert.SerializeObject(jsonRaw);

                        nr.Data().Add(new Entry() { Json = json, Bytes = binaryContent });

                        currentResourceList = resource.Resources.Last().Resources;
                    }
                    else
                    {
                        currentResourceList = existingPropertyResourse.Resources;
                    }
                }
            }

            if (null == resource)
            {
                return false;
            }

            // ASSERTION: We have found the resource we need to add the entry. 
            if (propertyName != null)
            {
                jobjectPropertyValue = jobject[propertyName];
            }
            if (jobjectPropertyValue != null)
            {
                propertyValue = jobjectPropertyValue.ToString();
            }

            var existingPropertyIndex = FindIndexOf(resource, propertyName, propertyValue, discriminator);
            if (existingPropertyIndex != -1)
            {
                resource.Resources.RemoveAt(existingPropertyIndex);
            }

            var existingPropertyResourse2 = resource.Resources.FirstOrDefault(f => string.Compare(f.Name, propertyValue, false) == 0 && f.IsPropertyResource);
            if (existingPropertyResourse2 != null)
            {
                resource.Resources.Remove(existingPropertyResourse2);
            }

            var newResource = new Resource(propertyValue, true, owner);
            newResource.Data().Add(new Entry() { Json = data, Bytes = binaryContent });
            resource.Resources.Add(newResource);

            return true;
        }



        public string Lookup(string path, string pattern, string propertyName, string value)
        {
            return Lookup(path, pattern, propertyName, value, null);
        }

        /// <summary>
        /// Lookup the data stored at the given path using the specified propertyName and value (case sensitive).
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pattern"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="discriminator"></param>
        /// <returns></returns>
        public string Lookup(string path, string pattern, string propertyName, string value, string discriminator)
        {
            if (null == discriminator) discriminator = "";

            var tokens = RouteParser.Parse(path, pattern).ToArray();
            if (tokens.Count() == 0) return null;

            List<Resource> currentResourceList = GetResources(discriminator);
            Resource resource = null;
            for (int offset = 0; offset < tokens.Length; offset++)
            {
                var token = tokens[offset];

                // NOTE: It doesn't matter what kind of token we are; we are 'walking' the Resource list to find our match. 
                if (token.Kind == RouteTokenKind.Resource)
                {
                    resource = currentResourceList.FirstOrDefault(f => string.Compare(f.Name, token.Name, true) == 0 && !f.IsPropertyResource);
                }
                else
                {
                    resource = currentResourceList.FirstOrDefault(f => string.Compare(f.Name, token.Value, true) == 0 && f.IsPropertyResource);
                }
                if (resource == null) return null;

                currentResourceList = resource.Resources;
            }

            var index = FindIndexOf(resource, propertyName, value);
            if (index == -1) return null;

            return resource.Resources[index].Data()[0].Json;
        }




        public bool Exists(string path, string pattern, string propertyName, string value)
        {
            return Exists(path, pattern, propertyName, value, null);
        }

        /// <summary>
        /// Returns true if a record exists; false otherwise. See Lookup for parameter information. 
        /// </summary>
        public bool Exists(string path, string pattern, string propertyName, string value, string discriminator)
        {
            if (discriminator == null) discriminator = "";

            return Lookup(path, pattern, propertyName, value, discriminator) != null;
        }



        /// <summary>
        /// Remove all resources from the database (including nested ones). 
        /// </summary>
        public void RemoveAll()
        {
            Groups.Clear();
        }



        public bool Remove(string path, string pattern, string propertyName, string value)
        {
            return Remove(path, pattern, propertyName, value, null);
        }

        /// <summary>
        /// Removes the first object that has a property name with the given value. See Lookup for parameter information. 
        /// </summary>
        /// <remarks>True true if the object was removed; false if the object was not within the resources. False implies that no state has taken place;
        /// techncially, if an object does not already exist, then it has already been removed but true/false are included for sanity reasons. </remarks>
        public bool Remove(string path, string pattern, string propertyName, string value, string discriminator)
        {
            if (null == discriminator) discriminator = "";

            var tokens = RouteParser.Parse(path, pattern).ToArray();
            if (tokens.Count() == 0) return false;

            Stack<Resource> resources = new Stack<Resource>();

            List<Resource> currentResourceList = GetResources(discriminator);
            Resource resource = null;
            for (int offset = 0; offset < tokens.Length; offset++)
            {
                var token = tokens[offset];

                // NOTE: It doesn't matter what kind of token we are; we are 'walking' the Resource list to find our match. 
                if (token.Kind == RouteTokenKind.Resource)
                {
                    resource = currentResourceList.FirstOrDefault(f => string.Compare(f.Name, token.Name, true) == 0 && !f.IsPropertyResource);
                }
                else
                {
                    if (offset == tokens.Length - 1)
                    {
                        // We are a 'value' at the end. ie: we are deleting /Pet/{Kind}
                        continue;
                    }
                    resource = currentResourceList.FirstOrDefault(f => string.Compare(f.Name, token.Value, true) == 0 && f.IsPropertyResource);
                }
                if (resource == null) return false;

                currentResourceList = resource.Resources;
                resources.Push(resource);
            }

            // NOTE: If the object does not already exist; then by definition it has been removed. 
            var match = currentResourceList.FirstOrDefault(f => f.Name == value);
            if (match == null) return false;

            if (resources.Count == 0) return false;

            var existingMatch = match; 

            // To remove a resource, we need to 'go back one' to remove it. 
            match = resources.Pop();

            var index = FindIndexOf(match, propertyName, value, discriminator);
            if (index == -1) return false;

            match.Resources.RemoveAt(index);
            return true;
        }



        /// <summary>
        /// Returns true if this Database contains an entry for the given resource. 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public bool ContainsResource(string path, string pattern, string discriminator)
        {
            var resource = LookupResource(path, pattern, discriminator);
            return resource != null;
        }

        /// <summary>
        /// Looks up the resource based on the given path. 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pattern"></param>
        /// <param name="ignoreFinalPropertyResource">If true, the final property resource will be ignored. This is an implementation hack until the Imdb is improved. </param>
        /// <returns></returns>
        public Resource LookupResource(string path, string pattern, string discriminator)
        {
            var tokens = RouteParser.Parse(path, pattern).ToArray();
            if (tokens.Count() == 0) return null;

            Stack<Resource> resources = new Stack<Resource>();

            List<Resource> currentResourceList = GetResources(discriminator);
            Resource resource = null;
            for (int offset = 0; offset < tokens.Length; offset++)
            {
                var token = tokens[offset];

                // NOTE: It doesn't matter what kind of token we are; we are 'walking' the Resource list to find our match. 
                if (token.Kind == RouteTokenKind.Resource)
                {
                    resource = currentResourceList.FirstOrDefault(f => string.Compare(f.Name, token.Name, true) == 0 && !f.IsPropertyResource);
                }
                else
                {
                    var index = FindIndexOf(resource, token.Name, token.Value, discriminator);
                    if (index == -1)
                    {
                        return null;
                    }
                    resource = resource.Resources[index];
                }
                if (resource == null) return null;

                currentResourceList = resource.Resources;
                resources.Push(resource);
            }

            return resource;
        }



        /// <summary>
        /// Remove the given resource from the Database. All data will be purged for that resource. Will return true if remove; false if it does not exist. 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public bool RemoveResource(string path, string pattern)
        {
            var tokens = RouteParser.Parse(path, pattern).ToArray();
            if (tokens.Count() == 0) return false;

            // TODO: Refactor when we support nested resources. 
            var token = tokens[0];
            if (token.Kind != RouteTokenKind.Resource) return false;

            var match = Resources.FirstOrDefault(f => f.Name == token.Value);
            if (match == null) return false;

            Resources.Remove(match);
            return true;
        }



        /// <summary>
        /// Return the value of propertyName in the json string. If the property does not exist in the string, this will return the value null. 
        /// </summary>
        /// <param name="json"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected string GetPropertyValue(string json, string propertyName)
        {
            JObject job = JsonConvert.DeserializeObject(json) as JObject;
            if (null == job) return null;

            return GetPropertyValue(job, propertyName);
        }

        /// <summary>
        /// Return the value of propertyName in JObject. If the property does not exist in the object, this wil return the value null. 
        /// </summary>
        /// <param name="jobject"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected string GetPropertyValue(JObject jobject, string propertyName)
        {
            var result = jobject[propertyName];
            if (null == result) return null;

            return result.ToString();
        }

        protected int FindIndexOf(Resource resource, string propertyName, string value)
        {
            return FindIndexOf(resource, propertyName, value, null);
        }

        /// <summary>
        /// Returns the 0-based index of the data in the resource where propertyName is equal to value (case sensitive). 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="discriminator"></param>
        /// <returns></returns>
        protected int FindIndexOf(Resource resource, string propertyName, string value, string discriminator)
        {
            if (null == discriminator) discriminator = "";
            if (null == propertyName) return -1;

            foreach (var r in resource.Resources)
            {
                foreach (var d in r.Data())
                {
                    JObject jobject = JsonConvert.DeserializeObject(d.Json) as JObject;
                    if (null == jobject) continue;

                    var currentValue = jobject[propertyName];
                    if (currentValue != null && currentValue.ToString() == value)
                    {
                        return resource.Resources.IndexOf(r);
                    }
                    if (currentValue == null && value == null)
                    {
                        return resource.Resources.IndexOf(r);
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Return the resources using the default Discriminator. 
        /// </summary>
        [Obsolete("Use GetResources() instead")]
        internal List<Resource> Resources
        {
            get
            {
                return GetResources();
            }
        }

        /// <summary>
        /// Get all of the resources for the default discriminator: ""
        /// </summary>
        /// <returns></returns>
        internal List<Resource> GetResources()
        {
            return GetResources("");
        }

        /// <summary>
        /// Get the resources for the 
        /// </summary>
        /// <param name="discriminator"></param>
        /// <returns></returns>
        internal List<Resource> GetResources(string discriminator)
        {
            if (discriminator == null) discriminator = "";

            var existing = Groups.FirstOrDefault(f => string.Compare(f.Name, discriminator, true) == 0);
            if (existing == null)
            {
                existing = new Group(discriminator);
                Groups.Add(existing);
            }
            return existing.Resources;
        }

        /// <summary>
        /// Groups of data associated with this database. 
        /// </summary>
        internal List<Group> Groups;
    }
}
