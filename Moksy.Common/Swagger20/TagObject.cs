using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common.Swagger20
{
    /// <summary>
    /// Allows adding meta data to a single tag that is used by the Operation Object. It is not mandatory to have a Tag Object per tag used there.
    /// </summary>
    public class TagObject
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public TagObject() : this(true)
        {
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="safe">If true, all values are given a non-</param>
        public TagObject(bool safe)
        {
            if (safe)
            {
                Name = "";
                Description = "";
                ExternalDocs = new ExternalDocumentationObject();
            }
            else
            {

            }
        }

        /// <summary>
        /// Required. The name of the tag.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// A short description for the tag. GFM syntax can be used for rich text representation.
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Additional external documentation for this tag.
        /// </summary>
        [JsonProperty(PropertyName="externalDocs")]
        public ExternalDocumentationObject ExternalDocs { get; set; }
    }
}
