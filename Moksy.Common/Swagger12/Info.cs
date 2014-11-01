using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Moksy.Common.Swagger.Common;

namespace Moksy.Common.Swagger12
{
    /// <summary>
    /// The object provides metadata about the API. The metadata can be used by the clients if needed, and can be presented in the Swagger-UI for convenience.
    /// </summary>
    public class Info
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public Info() : this(true)
        {
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="safe">If true, all properties will be given a none-null value. </param>
        public Info(bool safe)
        {
            Title = "";
            Description = "";

            if (safe)
            {
                TermsOfServiceUrl = "";
                Contact = "";
                License = "";
                LicenseUrl = "";
            }
            else
            {
            }
        }

        /// <summary>
        /// Required. The title of the application.
        /// </summary>
        [Description(@"Required. The title of the application.")]
        [JsonProperty(PropertyName="title")]
        public string Title {get;set;}

        /// <summary>
        /// Required. A short description of the application.
        /// </summary>
        [Description(@"Required. A short description of the application.")]
        [JsonProperty(PropertyName="description")]
        public string Description {get;set;}

        /// <summary>
        /// A URL to the Terms of Service of the API.
        /// </summary>
        [Description(@"A URL to the Terms of Service of the API.")]
        [JsonProperty(PropertyName="termsOfServiceUrl")]
        public string TermsOfServiceUrl {get;set;}

        /// <summary>
        /// An email to be used for API-related correspondence.
        /// </summary>
        [Description(@"An email to be used for API-related correspondence.")]
        [JsonProperty(PropertyName="contact")]
        public string Contact {get;set;}

        /// <summary>
        /// The license name used for the API.
        /// </summary>
        [Description(@"The license name used for the API.")]
        [JsonProperty(PropertyName="license")]
        public string License {get;set;}

        /// <summary>
        /// A URL to the license used for the API.
        /// </summary>
        [Description(@"A URL to the license used for the API.")]
        [JsonProperty(PropertyName="licenseUrl")]
        public string LicenseUrl {get;set;}




        /// <summary>
        /// Validate the Info object. 
        /// </summary>
        /// <param name="result"></param>
        public void Validate(ViolationCollection result)
        {
            Validate("", result);
        }

        /// <summary>
        /// Validate the Info object. 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        public void Validate(string context, ViolationCollection result)
        {
            if (Title == null) result.Add(new Violation() { Code = string.Format("{0}Title", context), Context = string.Format("{0}Title", context), ViolationLevel = ViolationLevel.Error, Description = @"Required. The title of the application." });
            if (Description == null) result.Add(new Violation() { Code = string.Format("{0}Description", context), Context = string.Format("{0}Description", context), ViolationLevel = ViolationLevel.Error, Description = @"Required. A short description of the application." });
        }
    }
}
