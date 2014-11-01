using Moksy.Common.Swagger.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common.Swagger20
{
    /// <summary>
    /// Performs validaton on the Swagger class. 
    /// </summary>
    public static class SwaggerValidator
    {
        /// <summary>
        /// Validates the Swagger model and returns any violations. 
        /// </summary>
        /// <param name="swagger">The model. Must not be null. </param>
        /// <returns>A collection of zero or more violations. </returns>
        public static ViolationCollection Validate(Swagger swagger)
        {
            ViolationCollection result = new ViolationCollection();
            Validate(swagger, result);
            return result;
        }

        /// <summary>
        /// Validates the Swagger model and adds any violations to the collection. 
        /// </summary>
        /// <param name="swagger">The model to validate. Must not be null. </param>
        /// <param name="result">The collection that will contain any violations. Must not be null. </param>
        public static void Validate(Swagger swagger, ViolationCollection result)
        {
            if (null == swagger) throw new System.ArgumentNullException("swagger");
            if (null == result) throw new System.ArgumentNullException("result");

            // Swagger
            if (string.Compare(swagger.Version, "2.0", true) != 0) result.Add(new Violation() { Code = "Swagger", ViolationLevel = ViolationLevel.Error, Context = "Version", Description = @"Specifies the Swagger Specification version being used. It can be used by the Swagger UI and other clients to interpret the API listing. The value MUST be ""2.0"")" });

            // Host
            if (swagger.Host == null || !(swagger.Host.Length > 0)) result.Add(new Violation() { Code = "Host", ViolationLevel = ViolationLevel.Informational, Context = "Host", Description = @"The host (name or ip) serving the API. This MUST be the host only and does not include the scheme nor sub-paths. It MAY include a port. If the host is not included, the host serving the documentation is to be used (including the port). The host does not support path templating." });

            // Basepath
            if (swagger.BasePath == null || !swagger.BasePath.StartsWith("/")) result.Add(new Violation() { Code = "BasePath", ViolationLevel = ViolationLevel.Informational, Context = "BasePath", Description = @"Specifies the Swagger Specification version being used. It can be used by the Swagger UI and other clients to interpret the API listing. The value MUST be ""2.0"")" });

            // Schemes
            var schemesValid = (swagger.Schemes != null);
            if (schemesValid)
            {
                schemesValid = (swagger.Schemes.Count() != 0);
                if (schemesValid)
                {
                    var allowed = new string[4] { "http", "https", "ws", "wss" };
                    foreach (var scheme in swagger.Schemes)
                    {
                        var match = allowed.FirstOrDefault(f => string.Compare(f, scheme, false) == 0);
                        if (match == null)
                        {
                            schemesValid = false;
                            break;
                        }
                    }
                }
            }
            if (!schemesValid)
            {
                result.Add(new Violation() { Code = "Schemes", ViolationLevel = ViolationLevel.Informational, Context = "Schemes", Description = @"The transfer protocol of the API. Values MUST be from the list: ""http"", ""https"", ""ws"", ""wss"". If the schemes is not included, the default scheme to be used is the one used to access the specification." });
            }

            // ExternalDocs
            ValidateExternalDocs(swagger.ExternalDocs, "ExternalDocs", result);

            // Info
            var infoValid = (swagger.Info != null);
            if (!infoValid) result.Add(new Violation() { Code = "Info", ViolationLevel = ViolationLevel.Error, Context = "Info", Description = @"Required Provides the version of the application API (not to be confused by the specification version)." });
            if (infoValid)
            {
                ValidateRequiredProperty(swagger.Info.Title, "Info.Title", ViolationLevel.Error, @"Required. The title of the application.", result);
                ValidateRequiredProperty(swagger.Info.Version, "Info.Version", ViolationLevel.Error, @"Required. The title of the application.", result);
            }

            // Contact
            if (swagger.Contact != null)
            {
                ValidateNotRequiredUrl(swagger.Contact.Url, "Contact.Url", ViolationLevel.Error, @"The URL pointing to the contact information. MUST be in the format of a URL.", result);
            }

            // License
            if (swagger.License != null)
            {
                ValidateRequiredProperty(swagger.License.Name, "License.Name", ViolationLevel.Error, @"Required. The license name used for the API.", result);

                ValidateNotRequiredUrl(swagger.License.Url, "License.Url", ViolationLevel.Error, @"A URL to the license used for the API. MUST be in the format of a URL.", result);
            }

            // Tags
            if (swagger.Tags != null)
            {
                // We need to validate each of the Tags
                foreach (var tag in swagger.Tags)
                {
                    var index = swagger.Tags.ToList().IndexOf(tag);

                    ValidateTag(tag, string.Format("Tags[{0}]", index), @"Required. The name of the tag.", result);

                    // See if there is another tag by this name that is not us.
                    var match = swagger.Tags.FirstOrDefault(f => string.Compare(f.Name, tag.Name, true) == 0 && f != tag && swagger.Tags.ToList().IndexOf(f) > index);
                    if (match == null) continue;

                    var context = string.Format("Tags[{0}].Name", swagger.Tags.ToList().IndexOf(match));
                    result.Add(new Violation() { Code = context, ViolationLevel = ViolationLevel.Error, Context = context, Description = @"Required. The name of the tag. Must be unique. " });
                }
            }
        }



        /// <summary>
        /// Validate a Url that is NOT required. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <param name="level"></param>
        /// <param name="description"></param>
        /// <param name="result"></param>
        public static void ValidateNotRequiredUrl(string value, string context, ViolationLevel level, string description, ViolationCollection result)
        {
            if (value != null)
            {
                var urlValid = false;
                try
                {
                    Uri uri = new Uri(value);

                    urlValid = true;
                }
                catch
                {
                }

                if (!urlValid) result.Add(new Violation() { Code = context, ViolationLevel = level, Context = context, Description = description });
            }
        }

        /// <summary>
        /// Validate a Url that is NOT required. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <param name="level"></param>
        /// <param name="description"></param>
        /// <param name="result"></param>
        public static void ValidateRequiredUrl(string value, string context, ViolationLevel level, string description, ViolationCollection result)
        {
            try
            {
                if (value == null || value == "")
                {
                    result.Add(new Violation() { Code = context, ViolationLevel = level, Context = context, Description = description });
                }
                else
                {
                    Uri uri = new Uri(value);
                }
                // If we get to here, the uri is valid. 
            }
            catch
            {
                result.Add(new Violation() { Code = context, ViolationLevel = level, Context = context, Description = description  });
            }
        }

        /// <summary>
        /// Validates a required property. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <param name="level"></param>
        /// <param name="description"></param>
        /// <param name="result"></param>
        public static void ValidateRequiredProperty(string value, string context, ViolationLevel level, string description, ViolationCollection result)
        {
            var valid = value != null;
            if (!valid) result.Add(new Violation() { Code = context, ViolationLevel = level, Context = context, Description = description });
        }

        /// <summary>
        /// Validate an external docs object. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <param name="result"></param>
        public static void ValidateExternalDocs(ExternalDocumentationObject value, string context, ViolationCollection result)
        {
            ValidateExternalDocs(value, context, @"Allows referencing an external resource for extended documentation.", @"Required. The URL for the target documentation. Value MUST be in the format of a URL", result);
        }

        /// <summary>
        /// Validates the ExternalDocs property. No external Docs property is actually required; so null is an acceptable value. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <param name="level"></param>
        /// <param name="description"></param>
        /// <param name="result"></param>
        public static void ValidateExternalDocs(ExternalDocumentationObject value, string context, string description, string urlDescription, ViolationCollection result)
        {
            if (value == null) return;

            var externalDocsValid = (value != null);
            if (!externalDocsValid)
            {
                result.Add(new Violation() { Code = context, ViolationLevel = ViolationLevel.Informational, Context = context, Description = description });
            }
            else
            {
                // swagger.ExternalDocs is not null. 
                if (value.Description == null) result.Add(new Violation() { Code = string.Format("{0}.Description", context), ViolationLevel = ViolationLevel.Informational, Context = string.Format("{0}.Description", context), Description = urlDescription });

                ValidateRequiredUrl(value.Url, string.Format("{0}.Url", context), ViolationLevel.Error, urlDescription, result);
            }
        }

        /// <summary>
        /// Validate an individual tag object. 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="context"></param>
        /// <param name="nameDescription"></param>
        /// <param name="result"></param>
        public static void ValidateTag(TagObject tag, string context, string nameDescription, ViolationCollection result)
        {
            if (tag == null) return;

            var nameValid = tag.Name != null;
            if (!nameValid)
            {
                result.Add(new Violation() { Code = string.Format("{0}.Name", context), Context = string.Format("{0}.Name", context), Description = nameDescription, ViolationLevel = ViolationLevel.Error });
            }

            ValidateExternalDocs(tag.ExternalDocs, string.Format("{0}.ExternalDocs", context), result);
        }
    }
}
