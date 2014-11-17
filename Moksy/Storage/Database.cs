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
            Resources = new List<Resource>();
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

            for (int offset = 0; offset < tokens.Length; offset++)
            {
                // TODO: Refactor when we support nested resources. 
                var token = tokens[offset];
                if (token.Kind != RouteTokenKind.Resource) return false;

                var resource = NewResource(token);
                if (null == resource) return false;

                string propertyValue = null;
                JToken jobjectPropertyValue = null;
                if (propertyName != null)
                {
                    jobjectPropertyValue = jobject[propertyName];
                }
                if (jobjectPropertyValue != null)
                {
                    propertyValue = jobjectPropertyValue.ToString();
                }

                var existingIndex = FindIndexOf(resource, propertyName, propertyValue, discriminator);
                if (existingIndex != -1)
                {
                    resource.Data(discriminator).RemoveAt(existingIndex);
                }

                resource.Data(discriminator).Add(new Entry() { Json = data, Bytes = binaryContent });
            }

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

            // TODO: Refactor when we support nested resources. 
            var token = tokens[0];
            if (token.Kind != RouteTokenKind.Resource) return null;

            var match = Resources.FirstOrDefault(f => f.Name == token.Value);
            if (match == null) return null;

            var index = FindIndexOf(match, propertyName, value);
            if (index == -1) return null;

            return match.Data(discriminator)[index].Json;
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
            Resources.Clear();
        }



        public bool Remove(string path, string pattern, string propertyName, string value)
        {
            return Remove(path, pattern, propertyName, value, null);
        }

        /// <summary>
        /// Removes the first object that has a property name with the given value. See Lookup for parameter information. 
        /// </summary>
        /// <remarks>True true if the object was removed; false otherwise. </remarks>
        public bool Remove(string path, string pattern, string propertyName, string value, string discriminator)
        {
            if (null == discriminator) discriminator = "";

            var tokens = RouteParser.Parse(path, pattern).ToArray();
            if (tokens.Count() == 0) return false;

            // TODO: Refactor when we support nested resources. 
            var token = tokens[0];
            if (token.Kind != RouteTokenKind.Resource) return false;

            var match = Resources.FirstOrDefault(f => f.Name == token.Value);
            if (match == null) return false;

            var index = FindIndexOf(match, propertyName, value, discriminator);
            if (index == -1) return false;

            match.Data(discriminator).RemoveAt(index);
            return true;
        }



        /// <summary>
        /// Returns true if this Database contains an entry for the given resource. 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public bool ContainsResource(string path, string pattern)
        {
            var resource = LookupResource(path, pattern);
            return resource != null;
        }



        /// <summary>
        /// Looks up the resource based on the given path. 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public Resource LookupResource(string path, string pattern)
        {
            var tokens = RouteParser.Parse(path, pattern).ToArray();
            if (tokens.Count() == 0) return null;

            // TODO: Refactor when we support nested resources. 
            var token = tokens[0];
            if (token.Kind != RouteTokenKind.Resource) return null;

            var match = Resources.FirstOrDefault(f => f.Name == token.Value);
            if (match == null) return null;

            return match;
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

        /// <summary>
        /// Will create an entry for the resource 
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        protected Resource NewResource(RouteToken token)
        {
            if (token.Kind != RouteTokenKind.Resource) return null;

            var match = Resources.FirstOrDefault(f => f.Name == token.Value);
            if (match != null) return match;

            Resources.Add(new Resource(token.Value));

            return Resources.Last();
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

            foreach (var d in resource.Data(discriminator))
            {
                JObject jobject = JsonConvert.DeserializeObject(d.Json) as JObject;
                if (null == jobject) continue;

                var currentValue = jobject[propertyName];
                if (currentValue != null && currentValue.ToString() == value)
                {
                    return resource.Data(discriminator).IndexOf(d);
                }
                if (currentValue == null && value == null)
                {
                    return resource.Data(discriminator).IndexOf(d);
                }
            }

            return -1;
        }

        /// <summary>
        /// Resources stored in this database. Each resource can itself have child resources. 
        /// </summary>
        internal List<Resource> Resources;
    }
}
