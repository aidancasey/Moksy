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
    /// This is the root document object for the API specification. It combines what previously was the Resource Listing and API Declaration (version 1.2 and earlier) together into one document.
    /// </summary>
    public class Swagger
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Swagger() : this(true)
        {

        }

        /// <summary>
        /// Construct a Swagger object. 
        /// </summary>
        /// <param name="safe">If true, all properties are given none-null values. If false, only Required properties are populated. </param>
        public Swagger(bool safe)
        {
            if (safe)
            {
                Version = "2.0";
                Host = "localhost";
                BasePath = "/";

                Schemes = new string[1] { "http" };
                Consumes = new string[0];
                Produces = new string[0];

                ExternalDocs = new ExternalDocumentationObject();
                Info = new InfoObject();
                Contact = new ContactObject();
                License = new LicenseObject();
                Tags = new TagObject[0];
            }
            else
            {
                Version = "2.0";

                Info = new InfoObject(safe);
            }
        }



        /// <summary>
        /// Specifies the Swagger Specification version being used. It can be used by the Swagger UI and other clients to interpret the API listing. The value MUST be "2.0"
        /// </summary>
        [JsonProperty(PropertyName="swagger")]
        [Description(@"Specifies the Swagger Specification version being used. It can be used by the Swagger UI and other clients to interpret the API listing. The value MUST be ""2.0"")")]
        public string Version { get; set; }

        /// <summary>
        /// Required. Provides metadata about the API. The metadata can be used by the clients if needed.
        /// </summary>
        [JsonProperty(PropertyName = "info")]
        [Description(@"Required. Provides metadata about the API. The metadata can be used by the clients if needed.")]
        public InfoObject Info { get; set; }

        /// <summary>
        /// The license information for the exposed API.
        /// </summary>
        [JsonProperty(PropertyName = "license")]
        [Description(@"The license information for the exposed API.")]
        public LicenseObject License { get; set; }

        /// <summary>
        /// The host (name or ip) serving the API. This MUST be the host only and does not include the scheme nor sub-paths. It MAY include a port. If the host is not included, the host serving the documentation is to be used (including the port). The host does not support path templating.
        /// </summary>
        [JsonProperty(PropertyName ="host")]
        [Description(@"The host (name or ip) serving the API. This MUST be the host only and does not include the scheme nor sub-paths. It MAY include a port. If the host is not included, the host serving the documentation is to be used (including the port). The host does not support path templating.")]
        public string Host { get; set; }

        /// <summary>
        /// The base path on which the API is served, which is relative to the host. If it is not included, the API is served directly under the host. The value MUST start with a leading slash (/). The basePath does not support path templating.
        /// </summary>
        [JsonProperty(PropertyName="basePath")]
        [Description(@"The base path on which the API is served, which is relative to the host. If it is not included, the API is served directly under the host. The value MUST start with a leading slash (/). The basePath does not support path templating.")]
        public string BasePath { get; set; }

        /// <summary>
        /// The transfer protocol of the API. Values MUST be from the list: "http", "https", "ws", "wss". If the schemes is not included, the default scheme to be used is the one used to access the specification.
        /// </summary>
        [JsonProperty(PropertyName = "schemes")]
        [Description(@"The transfer protocol of the API. Values MUST be from the list: ""http"", ""https"", ""ws"", ""wss"". If the schemes is not included, the default scheme to be used is the one used to access the specification.")]
        public string[] Schemes { get; set; }

        /// <summary>
        /// A list of MIME types the APIs can consume. This is global to all APIs but can be overridden on specific API calls. Value MUST be as described under Mime Types.
        /// </summary>
        [JsonProperty(PropertyName="consumes")]
        [Description(@"A list of MIME types the APIs can consume. This is global to all APIs but can be overridden on specific API calls. Value MUST be as described under Mime Types.")]
        public string[] Consumes { get; set; }

        /// <summary>
        /// A list of MIME types the APIs can produce. This is global to all APIs but can be overridden on specific API calls. Value MUST be as described under Mime Types.
        /// </summary>
        [JsonProperty(PropertyName = "produces")]
        [Description(@"A list of MIME types the APIs can produce. This is global to all APIs but can be overridden on specific API calls. Value MUST be as described under Mime Types.")]
        public string[] Produces { get; set; }

        /// <summary>
        /// Additional external documentation.
        /// </summary>
        [JsonProperty(PropertyName="externalDocs")]
        [Description(@"Additional external documentation.")]
        public ExternalDocumentationObject ExternalDocs { get; set; }

        /// <summary>
        /// The contact information for the exposed API.
        /// </summary>
        [JsonProperty(PropertyName="contact")]
        [Description(@"The contact information for the exposed API.")]
        public ContactObject Contact { get; set; }

        /// <summary>
        /// A list of tags used by the specification with additional metadata. The order of the tags can be used to reflect on their order by the parsing tools. Not all tags that are used by the Operation Object must be declared. The tags that are not declared may be organized randomly or based on the tools' logic. Each tag name in the list MUST be unique.
        /// </summary>
        [JsonProperty(PropertyName = "tags")]
        [Description(@"A list of tags used by the specification with additional metadata. The order of the tags can be used to reflect on their order by the parsing tools. Not all tags that are used by the Operation Object must be declared. The tags that are not declared may be organized randomly or based on the tools' logic. Each tag name in the list MUST be unique.")]
        public TagObject[] Tags { get; set; }
    }
}
