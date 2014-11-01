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
    /// The Operation Object describes a single operation on a path.
    ///
    /// In the operations array, there MUST be only one Operation Object per method.
    ///
    /// This object includes the Data Type Fields in order to describe the return value of the operation. The type field MUST be used to link to other models.
    ///
    /// This is the only object where the type MAY have the value of void to indicate that the operation returns no value.
    /// </summary>
    public class Operation
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public Operation() : this(true)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="safe">If true, default (none-null) values will be used for all properties. </param>
        public Operation(bool safe)
        {
            Method = "POST";
            NickName = "Name" + new Random().Next(1, 1000);
            Parameters = new Parameter[0];

            if (safe)
            {
                Summary = "";
                Notes = "";
                Produces = new string[0];
                Consumes = new string[0];
                Deprecated = "false";
            }
            else
            {
            }
        }



        /// <summary>
        /// Required. The HTTP method required to invoke this operation. The value MUST be one of the following values: "GET", "HEAD", "POST", "PUT", "PATCH", "DELETE", "OPTIONS". The values MUST be in uppercase.
        /// </summary>
        [Description(@"Required. The HTTP method required to invoke this operation. The value MUST be one of the following values: ""GET"", ""HEAD"", ""POST"", ""PUT"", ""PATCH"", ""DELETE"", ""OPTIONS"". The values MUST be in uppercase.")]
        [JsonProperty(PropertyName="method")]
        public string Method { get; set; }
        
        /// <summary>
        /// A short summary of what the operation does. For maximum readability in the swagger-ui, this field SHOULD be less than 120 characters.
        /// </summary>
        [Description(@"A short summary of what the operation does. For maximum readability in the swagger-ui, this field SHOULD be less than 120 characters.")]
        [JsonProperty(PropertyName="summary")]
        public string Summary { get; set; }

        /// <summary>
        /// A verbose explanation of the operation behavior.
        /// </summary>
        [Description(@"A verbose explanation of the operation behavior.")]
        [JsonProperty(PropertyName="notes")]
        public string Notes { get; set; }

        /// <summary>
        /// Required. A unique id for the operation that can be used by tools reading the output for further and easier manipulation. For example, Swagger-Codegen will use the nickname as the method name of the operation in the client it generates. The value MUST be alphanumeric and may include underscores. Whitespace characters are not allowed.
        /// </summary>
        [Description(@"Required. A unique id for the operation that can be used by tools reading the output for further and easier manipulation. For example, Swagger-Codegen will use the nickname as the method name of the operation in the client it generates. The value MUST be alphanumeric and may include underscores. Whitespace characters are not allowed.")]
        [JsonProperty(PropertyName="nickName")]
        public string NickName { get; set; }

        /// <summary>
        /// A list of MIME types this operation can produce. This is overrides the global produces definition at the root of the API Declaration. Each string value SHOULD represent a MIME type.
        /// </summary>
        [Description(@"A list of MIME types this operation can produce. This is overrides the global produces definition at the root of the API Declaration. Each string value SHOULD represent a MIME type.")]
        [JsonProperty(PropertyName="produces")]
        public string[] Produces { get; set; }

        /// <summary>
        /// A list of MIME types this operation can consume. This is overrides the global consumes definition at the root of the API Declaration. Each string value SHOULD represent a MIME type.
        /// </summary>
        [Description(@"A list of MIME types this operation can consume. This is overrides the global consumes definition at the root of the API Declaration. Each string value SHOULD represent a MIME type.")]
        [JsonProperty(PropertyName="consumes")]
        public string[] Consumes { get; set; }

        /// <summary>
        /// Declares this operation to be deprecated. Usage of the declared operation should be refrained. Valid value MUST be either "true" or "false". Note: This field will change to type boolean in the future.
        /// </summary>
        [Description(@"Declares this operation to be deprecated. Usage of the declared operation should be refrained. Valid value MUST be either ""true"" or ""false"". Note: This field will change to type boolean in the future.")]
        [JsonProperty(PropertyName ="deprecated")]
        public string Deprecated { get; set; }

        /// <summary>
        /// Required. The inputs to the operation. If no parameters are needed, an empty array MUST be included.
        /// </summary>
        [Description(@"Required. The inputs to the operation. If no parameters are needed, an empty array MUST be included.")]
        [JsonProperty(PropertyName="parameters")]
        public Parameter[] Parameters { get; set; }


        /// <summary>
        /// Valid methods. 
        /// </summary>
        public static string[] ValidMethods = new string[] { "GET", "HEAD", "POST", "PUT", "PATCH", "DELETE", "OPTIONS" };


        /// <summary>
        /// Validate the Operation. 
        /// </summary>
        /// <param name="result"></param>
        public void Validate(ViolationCollection result)
        {
            if (result == null) return;

            if (Method == null || ValidMethods.FirstOrDefault(f => string.Compare(f, Method, false) == 0) == null)
            {
                result.Add(new Violation() { Code ="Method", Context = "Method", Description = @"Required. The HTTP method required to invoke this operation. The value MUST be one of the following values: ""GET"", ""HEAD"", ""POST"", ""PUT"", ""PATCH"", ""DELETE"", ""OPTIONS"". The values MUST be in uppercase.", ViolationLevel = ViolationLevel.Error });
            }
            if (!ValidationHelpers.IsValidNickName(NickName))
            {
                result.Add(new Violation() { Code = "NickName", Context = "NickName", Description = @"Required. A unique id for the operation that can be used by tools reading the output for further and easier manipulation. For example, Swagger-Codegen will use the nickname as the method name of the operation in the client it generates. The value MUST be alphanumeric and may include underscores. Whitespace characters are not allowed.", ViolationLevel = ViolationLevel.Error });
            }
            if (Deprecated != null)
            {
                if (Deprecated != "false" && Deprecated != "true")
                {
                    result.Add(new Violation() { Code = "Deprecated", Context = "Deprecated", Description = @"Declares this operation to be deprecated. Usage of the declared operation should be refrained. Valid value MUST be either ""true"" or ""false"". Note: This field will change to type boolean in the future.", ViolationLevel = ViolationLevel.Error });
                }
            }
            if (Parameters == null)
            {
                result.Add(new Violation() { Code = "Parameters", Context = "Parameters", Description = @"Required. The inputs to the operation. If no parameters are needed, an empty array MUST be included.", ViolationLevel = ViolationLevel.Error });
            }
        }
    }
}
