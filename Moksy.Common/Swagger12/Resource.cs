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
    /// The Resource object describes a resource API endpoint in the application.
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public Resource() : this(true)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="safe">If true, the properites are given none-null values. </param>
        public Resource(bool safe)
        {
            Path = "/";

            if (safe)
            {
                Description = "";
            }
            else
            {

            }
        }

        /// <summary>
        /// Required. A relative path to the API declaration from the path used to retrieve this Resource Listing. This path does not necessarily have to correspond to the URL which actually serves this resource in the API but rather where the resource listing itself is served. The value SHOULD be in a relative (URL) path format.
        /// </summary>
        [Description(@"Required. A relative path to the API declaration from the path used to retrieve this Resource Listing. This path does not necessarily have to correspond to the URL which actually serves this resource in the API but rather where the resource listing itself is served. The value SHOULD be in a relative (URL) path format.")]
        [JsonProperty(PropertyName="path")]
        public string Path { get; set; }

        /// <summary>
        /// Recommended. A short description of the resource.
        /// </summary>
        [Description(@"Recommended. A short description of the resource.")]
        [JsonProperty(PropertyName="description")]
        public string Description { get; set; }



        /// <summary>
        /// Validate the Resource.
        /// </summary>
        /// <param name="result"></param>
        public void Validate(ViolationCollection result)
        {
            Validate("", result);
        }

        /// <summary>
        /// Validate the Resource.
        /// </summary>
        /// <param name="result"></param>
        public void Validate(string context, ViolationCollection result)
        {
            if (null == result) return;

            if (string.IsNullOrEmpty(Path)) result.Add(new Violation() { Code = string.Format("{0}Path", context), Context = string.Format("{0}Path", context), ViolationLevel = ViolationLevel.Error, Description = @"Required. A relative path to the API declaration from the path used to retrieve this Resource Listing. This path does not necessarily have to correspond to the URL which actually serves this resource in the API but rather where the resource listing itself is served. The value SHOULD be in a relative (URL) path format." });
        }
    }
}
