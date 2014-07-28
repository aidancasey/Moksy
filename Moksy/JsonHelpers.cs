using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common
{
    /// <summary>
    /// Internal helper classes for working with Json.
    /// </summary>
    internal class JsonHelpers
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public JsonHelpers()
        {
        }

        /// <summary>
        /// Set a property in the jsonContent. This will serialize jsonContent into a JObject; and set propertyName to the given value. 
        /// </summary>
        /// <param name="propertyName">The property Name. </param>
        /// <param name="propertyValue">The property Value. </param>
        /// <param name="jsonContent">The existing json content. If Null will be coerced into {}</param>
        /// <returns></returns>
        public string SetProperty(string propertyName, string propertyValue, string jsonContent)
        {
            if (null == propertyName) return jsonContent;
            if (null == jsonContent) jsonContent = "{}";

            JObject j = JsonConvert.DeserializeObject(jsonContent) as JObject;
            if (null == j) throw new System.ArgumentException(string.Format("ERROR: Unable to parse jsonContent. Not valid Json: {0}", jsonContent));

            // We now try to work out whether propertyValue is actually a Json string. If so, we set the nested JObject instead of settings it as a string.
            bool isString = true;
            JObject propertyValueAsJobject = null;
            try
            {
                propertyValueAsJobject = JsonConvert.DeserializeObject(propertyValue) as JObject;
                isString = false;
            }
            catch (Exception ex)
            {
            }

            if (isString)
            {
                j[propertyName] = propertyValue;
            }
            else
            {
                j[propertyName] = propertyValueAsJobject;
            }

            var result = JsonConvert.SerializeObject(j);
            return result;
        }
    }
}
