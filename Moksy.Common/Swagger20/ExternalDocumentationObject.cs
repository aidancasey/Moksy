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
    /// Allows referencing an external resource for extended documentation.
    /// </summary>
    public class ExternalDocumentationObject
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public ExternalDocumentationObject()
        {
            Description = "";
            Url = "http://swagger.io";
        }

        /// <summary>
        /// A short description of the target documentation. GFM syntax can be used for rich text representation.
        /// </summary>
        [JsonProperty(PropertyName="description")]
        [Description(@"A short description of the target documentation. GFM syntax can be used for rich text representation.")]
        public string Description { get; set; }

        /// <summary>
        /// Required. The URL for the target documentation. Value MUST be in the format of a URL
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        [Description(@"Required. The URL for the target documentation. Value MUST be in the format of a URL.")]
        public string Url { get; set; }
    }
}
