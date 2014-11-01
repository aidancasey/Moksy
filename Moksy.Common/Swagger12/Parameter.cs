using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moksy.Common.Swagger.Common;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Moksy.Common.Swagger12
{
    /// <summary>
    /// The Parameter Object describes a single parameter to be sent in an operation and maps to the parameters field in the Operation Object.
    /// This object includes the Data Type Fields in order to describe the type of this parameter. The type field MUST be used to link to other models.
    ///
    /// If type is File, the consumes field MUST be "multipart/form-data", and the paramType MUST be "form".
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public Parameter() : this(true)
        {
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="safe">If true, all properties will be given a none-null value. </param>
        public Parameter(bool safe)
        {
            ParamType = "path";
            Name = "/";
            Required = true;

            if (safe)
            {
                Description = "";
            }
            else
            {
            }
        }

        /// <summary>
        /// Required. The type of the parameter (that is, the location of the parameter in the request). The value MUST be one of these values: "path", "query", "body", "header", "form". Note that the values MUST be lower case.
        /// </summary>
        [Description(@"Required. The type of the parameter (that is, the location of the parameter in the request). The value MUST be one of these values: ""path"", ""query"", ""body"", ""header"", ""form"". Note that the values MUST be lower case.")]
        [JsonProperty(PropertyName="paramType")]
        public string ParamType { get; set; }

        /// <summary>
        /// Required. The unique name for the parameter. Each name MUST be unique, even if they are associated with different paramType values. Parameter names are case sensitive.
        /// If paramType is "path", the name field MUST correspond to the associated path segment from the path field in the API Object.
        /// If paramType is "query", the name field corresponds to the query parameter name.
        /// If paramType is "body", the name is used only for Swagger-UI and Swagger-Codegen. In this case, the name MUST be "body".
        /// If paramType is "form", the name field corresponds to the form parameter key.
        /// If paramType is "header", the name field corresponds to the header parameter key.
        /// </summary>
        [Description(@"Required. The unique name for the parameter. Each name MUST be unique, even if they are associated with different paramType values. Parameter names are case sensitive. If paramType is ""path"", the name field MUST correspond to the associated path segment from the path field in the API Object. If paramType is ""query"", the name field corresponds to the query parameter name. If paramType is ""body"", the name is used only for Swagger-UI and Swagger-Codegen. In this case, the name MUST be ""body"". If paramType is ""form"", the name field corresponds to the form parameter key. If paramType is ""header"", the name field corresponds to the header parameter key.")]
        [JsonProperty(PropertyName="name")]
        public string Name { get; set; }

        /// <summary>
        /// Recommended. A brief description of this parameter.
        /// </summary>
        [Description(@"Recommended. A brief description of this parameter.")]
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        ///A flag to note whether this parameter is required. If this field is not included, it is equivalent to adding this field with the value false. If paramType is "path" then this field MUST be included and have the value true.
        [Description(@"A flag to note whether this parameter is required. If this field is not included, it is equivalent to adding this field with the value false. If paramType is ""path"" then this field MUST be included and have the value true.")]
        [JsonProperty("required")]
        public bool Required { get; set; }

        /// <summary>
        /// Another way to allow multiple values for a "query" parameter. If used, the query parameter may accept comma-separated values. The field may be used only if paramType is "query", "header" or "path".
        /// </summary>
        [Description(@"Another way to allow multiple values for a ""query"" parameter. If used, the query parameter may accept comma-separated values. The field may be used only if paramType is ""query"", ""header"" or ""path"".")]
        [JsonProperty("allowMultiple")]
        public bool AllowMultiple { get; set; }



        /// <summary>
        /// The valid parameter types. Case sensitive. 
        /// </summary>
        public static string[] ValidParamTypes = new string[] { "path", "query", "body", "header", "form" };



        /// <summary>
        /// Validate the Parameter. 
        /// </summary>
        /// <param name="result"></param>
        public void Validate(ViolationCollection result)
        {
            if (null == result) return;

            if (ParamType == null || ValidParamTypes.FirstOrDefault(f => string.Compare(f, ParamType, false) == 0) == null)
            {
                result.Add(new Violation() { Code = "ParamType", Context = "ParamType", Description = @"Required. The type of the parameter (that is, the location of the parameter in the request). The value MUST be one of these values: ""path"", ""query"", ""body"", ""header"", ""form"". Note that the values MUST be lower case.", ViolationLevel = ViolationLevel.Error });
            }

            if (!ValidationHelpers.IsParameterNameValid(null, this))
            {
                result.Add(new Violation() { Code = "Name", Context = "Name", Description = @"Required. The unique name for the parameter. Each name MUST be unique, even if they are associated with different paramType values. Parameter names are case sensitive. If paramType is ""path"", the name field MUST correspond to the associated path segment from the path field in the API Object. If paramType is ""query"", the name field corresponds to the query parameter name. If paramType is ""body"", the name is used only for Swagger-UI and Swagger-Codegen. In this case, the name MUST be ""body"". If paramType is ""form"", the name field corresponds to the form parameter key. If paramType is ""header"", the name field corresponds to the header parameter key.", ViolationLevel = ViolationLevel.Error });
            }

            if (ParamType == "path" && !Required)
            {
                result.Add(new Violation() { Code = "Required", Context = "Required", Description = @"A flag to note whether this parameter is required. If this field is not included, it is equivalent to adding this field with the value false. If paramType is ""path"" then this field MUST be included and have the value true.", ViolationLevel = ViolationLevel.Error });
            }
        }
    }
}
