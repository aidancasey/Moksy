using Moksy.Common.Swagger.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common.Swagger12
{
    /// <summary>
    /// The Resource Listing serves as the root document for the API description. It contains general information about the API and an inventory of the available resources.
    ///
    /// By default, this document SHOULD be served at the /api-docs path.
    /// </summary>
    public class ResourceListing
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ResourceListing() : this(true)
        {
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="safe">If true, a safe object (with no none-null values) is created. </param>
        public ResourceListing(bool safe)
        {
            SwaggerVersion = "1.2";
            Apis = new Resource[0];

            if (safe)
            {
                ApiVersion = "";
                Info = new Info();
            }
            else
            {
            }
        }



        /// <summary>
        /// Required. Specifies the Swagger Specification version being used. It can be used by the Swagger UI and other clients to interpret the API listing. The value MUST be an existing Swagger specification version. 
        /// Currently, "1.0", "1.1", "1.2" are valid values. The field is a string type for possible non-numeric versions in the future (for example, "1.2a").
        /// </summary>
        [Description(@"Required. Specifies the Swagger Specification version being used. It can be used by the Swagger UI and other clients to interpret the API listing. The value MUST be an existing Swagger specification version. Currently, ""1.0"", ""1.1"", ""1.2"" are valid values. The field is a string type for possible non-numeric versions in the future (for example, ""1.2a"").")]
        [JsonProperty(PropertyName="swaggerVersion")]
        public string SwaggerVersion { get; set; }

        /// <summary>
        /// Required. Lists the resources to be described by this specification implementation. The array can have 0 or more elements.
        /// </summary>
        [Description(@"Required. Lists the resources to be described by this specification implementation. The array can have 0 or more elements.")]
        [JsonProperty(PropertyName = "apis")]
        public Resource[] Apis { get; set; }

        /// <summary>
        /// Provides the version of the application API (not to be confused by the specification version).
        /// </summary>
        [Description(@"Provides the version of the application API (not to be confused by the specification version).")]
        [JsonProperty(PropertyName = "apiVersion")]
        public string ApiVersion { get; set; }

        /// <summary>
        /// Provides metadata about the API. The metadata can be used by the clients if needed, and can be presented in the Swagger-UI for convenience.
        /// </summary>
        [Description(@"Provides metadata about the API. The metadata can be used by the clients if needed, and can be presented in the Swagger-UI for convenience.")]
        [JsonProperty(PropertyName = "info")]
        public Info Info { get; set; }


        /// <summary>
        /// Validate the Resource.
        /// </summary>
        /// <param name="result"></param>
        public void Validate(ViolationCollection result)
        {
            if (null == result) return;

            if (string.IsNullOrEmpty(SwaggerVersion)) result.Add(new Violation() { Code = "SwaggerVersion", Context = "SwaggerVersion", ViolationLevel = ViolationLevel.Error, Description = @"Required. Specifies the Swagger Specification version being used. It can be used by the Swagger UI and other clients to interpret the API listing. The value MUST be an existing Swagger specification version. Currently, ""1.0"", ""1.1"", ""1.2"" are valid values. The field is a string type for possible non-numeric versions in the future (for example, ""1.2a"")." });
            if (Apis == null) result.Add(new Violation() { Code = "Apis", Context = "Apis", ViolationLevel = ViolationLevel.Error, Description = @"Required. Lists the resources to be described by this specification implementation. The array can have 0 or more elements." });

            if (Apis != null)
            {
                foreach (var api in Apis)
                {
                    api.Validate(string.Format("Apis[{0}].", Apis.ToList().IndexOf(api)), result);
                }
            }

            if (Info != null)
            {
                Info.Validate("Info.", result);
            }
        }
    }
}
