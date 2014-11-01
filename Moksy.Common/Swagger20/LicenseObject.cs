using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common.Swagger20
{
    /// <summary>
    /// License information for the exposed API.
    /// </summary>
    public class LicenseObject
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public LicenseObject() : this(true)
        {
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="safe">If true, all values will be set to none-null. </param>
        public LicenseObject(bool safe)
        {
            Name = "";

            if (safe)
            {
                Url = "http://localhost/license.html";
            }
            else
            {

            }
        }

        /// <summary>
        /// Required. The license name used for the API.
        /// </summary>
        [JsonProperty(PropertyName="name")]
        [Description(@"Required. The license name used for the API.")]
        public string Name { get; set; }

        /// <summary>
        /// A URL to the license used for the API. MUST be in the format of a URL.
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        [Description(@"A URL to the license used for the API. MUST be in the format of a URL.")]
        public string Url { get; set; }
    }
}
