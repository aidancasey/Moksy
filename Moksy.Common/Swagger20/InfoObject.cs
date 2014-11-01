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
    /// The object provides metadata about the API. The metadata can be used by the clients if needed, and can be presented in the Swagger-UI for convenience.
    /// </summary>
    public class InfoObject
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public InfoObject() : this(true)
        {
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="safe">If true, all properties are given default (none-null) values. </param>
        public InfoObject(bool safe)
        {
            if (safe)
            {
                Title = "";
                Description = "";
                TermsOfService = "";
                Version = "";
            }
            else
            {
                Title = "";
                Version = "";
            }
        }

        [JsonProperty(PropertyName="title")]
        [Description(@"Required. The title of the application.")]
        public string Title { get; set; }

        [JsonProperty(PropertyName="description")]
        [Description(@"A short description of the application. GFM syntax can be used for rich text representation.")]
        public string Description { get; set; }

        [JsonProperty(PropertyName="termsOfService")]
        [Description(@"The Terms of Service for the API.")]
        public string TermsOfService { get; set; }

        [JsonProperty(PropertyName="version")]
        [Description(@"Required Provides the version of the application API (not to be confused by the specification version).")]
        public string Version { get; set; }
    }
}
