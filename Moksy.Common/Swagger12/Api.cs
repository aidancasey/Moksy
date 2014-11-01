using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel;
using Moksy.Common.Swagger.Common;

namespace Moksy.Common.Swagger12
{
    /// <summary>
    /// The API Object describes one or more operations on a single path. In the apis array, there MUST be only one API Object per path.
    /// </summary>
    public class Api
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public Api() : this(true)
        {
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="safe">If true, default none-null values will be provided. </param>
        public Api(bool safe)
        {
            Path = "/";

            if (safe)
            {
                Description = "";
                Operations = new Operation[0];
                Models = new Models();
            }
            else
            {
            }
        }



        /// <summary>
        /// Required. The relative path to the operation, from the basePath, which this operation describes. The value SHOULD be in a relative (URL) path format.
        /// </summary>
        [Description(@"Required. The relative path to the operation, from the basePath, which this operation describes. The value SHOULD be in a relative (URL) path format.")]
        [JsonProperty(PropertyName="path")]
        public string Path { get; set; }

        /// <summary>
        /// Recommended. A short description of the resource.
        /// </summary>
        [Description(@"Recommended. A short description of the resource.")]
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Required. A list of the API operations available on this path. The array may include 0 or more operations. There MUST NOT be more than one Operation Object per method in the array.
        /// </summary>
        [Description(@"Required. A list of the API operations available on this path. The array may include 0 or more operations. There MUST NOT be more than one Operation Object per method in the array.")]
        [JsonProperty(PropertyName = "operations")]
        public Operation[] Operations { get; set; }

        /// <summary>
        /// A list of the models available to this resource. Note that these need to be exposed separately for each API Declaration.
        /// </summary>
        [Description(@"A list of the models available to this resource. Note that these need to be exposed separately for each API Declaration.")]
        [JsonProperty(PropertyName="models")]
        public Models Models { get; set; }



        /// <summary>
        /// Validte the Api. 
        /// </summary>
        /// <param name="result"></param>
        public void Validate(ViolationCollection result)
        {
            Validate("", result);
        }

        /// <summary>
        /// Validate the Api object. 
        /// </summary>
        /// <param name="result"></param>
        public void Validate(string context, ViolationCollection result)
        {
            if (result == null) return;

            ValidationHelpers.ValidateRequiredUrl(Path, string.Format("{0}Path", context), @"Required. The relative path to the operation, from the basePath, which this operation describes. The value SHOULD be in a relative (URL) path format.", UriKind.Relative, result);
        }
    }
}
