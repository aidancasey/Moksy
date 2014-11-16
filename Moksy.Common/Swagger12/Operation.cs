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
            Type = "string";
            Format = null;

            if (safe)
            {
                Summary = "";
                Notes = "";
                Produces = new string[0];
                Consumes = new string[0];
                Deprecated = "false";
                Items = new Items(safe);
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
        /// Required (if $ref is not used). The return type of the operation. The value MUST be one of the Primitives, array or a model's id.
        /// </summary>
        [Description(@"Required (if $ref is not used). The return type of the operation. The value MUST be one of the Primitives, array or a model's id.")]
        [JsonProperty(PropertyName="type")]
        public string Type { get; set; }

        /// <summary>
        /// Fine-tuned primitive type definition. See Primitives for further information. The value MUST be one that is defined under Primitives, corresponding to the right primitive type.
        /// </summary>
        [Description(@"Fine-tuned primitive type definition. See Primitives for further information. The value MUST be one that is defined under Primitives, corresponding to the right primitive type.")]
        [JsonProperty(PropertyName="format")]
        public string Format { get; set; }

        /// <summary>
        /// Required. The type definition of the values in the container. A container MUST NOT be nested in another container.
        /// </summary>
        [Description(@"Required. The type definition of the values in the container. A container MUST NOT be nested in another container.")]
        [JsonProperty(PropertyName = "items")]
        public Items Items { get; set; }



        #region Type Recognition

        /// <summary>
        /// Is the return type void? (NOTE: How to detect this? Not sure. )
        /// </summary>
        [JsonIgnore()]
        public bool IsVoid { get { return string.Compare(Type, "void", false) == 0; } }
        
        [JsonIgnore()]
        public bool IsInt32 { get { return IsType("integer", "int32"); } }

        [JsonIgnore()]
        public bool IsInt64 { get { return IsType("integer", "int64"); } }

        [JsonIgnore()]
        public bool IsFloat { get { return IsType("number", "float"); } }

        [JsonIgnore()]
        public bool IsDouble { get { return IsType("number", "double"); } }

        [JsonIgnore()]
        public bool IsString { get { return IsType("string", null); } }

        [JsonIgnore()]
        public bool IsByte { get { return IsType("string", "byte"); } }

        [JsonIgnore()]
        public bool IsBoolean { get { return IsType("boolean", null); } }

        [JsonIgnore()]
        public bool IsDate { get { return IsType("string", "date"); } }

        [JsonIgnore()]
        public bool IsDateTime { get { return IsType("string", "date-time"); } }

        [JsonIgnore()]
        public bool IsArray { get { return string.Compare("array", Type, false) == 0 && Format == null; } }

        [JsonIgnore()]
        public bool IsModelReference
        {
            get
            {
                var knownPrimitive = Primitives.FirstOrDefault(f => string.Compare(f, Type, false) == 0) != null;
                var isArray = IsArray;

                if (!knownPrimitive && !isArray && Format == null && Type.Length > 0)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Helper method to work out if this property is of the given type. 
        /// </summary>
        /// <param name="type">The type. ie: integer. </param>
        /// <param name="format">The format. ie: int32. </param>
        /// <returns></returns>
        protected bool IsType(string type, string format)
        {
            if (type == null) return false;
            if (type == "array" && Type == "array") return true;

            return (string.Compare(type, Type, false) == 0 && string.Compare(format, Format, false) == 0);
        }

        /// <summary>
        /// List of all primitive datatypes allowed on a property. 
        /// </summary>
        public static string[] Primitives = new string[] { "integer", "long", "float", "double", "string", "byte", "boolean", "date", "dateTime" };

        /// <summary>
        /// Lookup table for all primitive types. 
        /// </summary>
        public static Dictionary<string, PrimitiveTypeFormat> PrimitiveTypeFormats = new Dictionary<string, PrimitiveTypeFormat>(StringComparer.InvariantCulture) { 
            { "integer", new PrimitiveTypeFormat("integer", "integer", "int32" ) },
            { "long", new PrimitiveTypeFormat("long", "integer", "int64" ) },
            { "float", new PrimitiveTypeFormat("float", "number", "float" ) },
            { "double", new PrimitiveTypeFormat("double", "number", "double" ) },
            { "string", new PrimitiveTypeFormat("string", "string", null ) },
            { "byte", new PrimitiveTypeFormat("byte", "string", "byte" ) },
            { "boolean", new PrimitiveTypeFormat("boolean", "boolean", null ) },
            { "date", new PrimitiveTypeFormat("date", "string", "date" ) },
            { "dateTime", new PrimitiveTypeFormat("dateTime", "string", "date-time" ) }
        };

        #endregion




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
            Validate("", result);
        }

        /// <summary>
        /// Validate the Operation. 
        /// </summary>
        /// <param name="result"></param>
        public void Validate(string context, ViolationCollection result)
        {
            if (result == null) return;

            if (Method == null || ValidMethods.FirstOrDefault(f => string.Compare(f, Method, false) == 0) == null)
            {
                result.Add(new Violation() { Code ="Method", Context = string.Format("{0}Method", context), Description = @"Required. The HTTP method required to invoke this operation. The value MUST be one of the following values: ""GET"", ""HEAD"", ""POST"", ""PUT"", ""PATCH"", ""DELETE"", ""OPTIONS"". The values MUST be in uppercase.", ViolationLevel = ViolationLevel.Error });
            }
            if (!ValidationHelpers.IsValidNickName(NickName))
            {
                result.Add(new Violation() { Code = "NickName", Context = string.Format("{0}NickName", context), Description = @"Required. A unique id for the operation that can be used by tools reading the output for further and easier manipulation. For example, Swagger-Codegen will use the nickname as the method name of the operation in the client it generates. The value MUST be alphanumeric and may include underscores. Whitespace characters are not allowed.", ViolationLevel = ViolationLevel.Error });
            }
            if (Deprecated != null)
            {
                if (Deprecated != "false" && Deprecated != "true")
                {
                    result.Add(new Violation() { Code = "Deprecated", Context = string.Format("{0}Deprecated", context), Description = @"Declares this operation to be deprecated. Usage of the declared operation should be refrained. Valid value MUST be either ""true"" or ""false"". Note: This field will change to type boolean in the future.", ViolationLevel = ViolationLevel.Error });
                }
            }
            if (Parameters == null)
            {
                result.Add(new Violation() { Code = "Parameters", Context = string.Format("{0}Parameters", context), Description = @"Required. The inputs to the operation. If no parameters are needed, an empty array MUST be included.", ViolationLevel = ViolationLevel.Error });
            }

            bool typeValid = true;
            if (Type == null) typeValid = false;
            if (typeValid)
            {
                var primitives = from T in PrimitiveTypeFormats select T.Value;
                var knownPrimitive = primitives.FirstOrDefault(f => string.Compare(f.Type, Type, false) == 0 && string.Compare(f.Format, Format, false) == 0) != null;
                var arrayOrId = !knownPrimitive && string.Compare("array", Type, false) == 0 || Type.Length > 0;

                if (knownPrimitive)
                {
                    var match = primitives.FirstOrDefault(f => string.Compare(f.Type, Type, false) == 0 && string.Compare(f.Format, Format, false) == 0);
                    typeValid = (string.Compare(match.Type, Type, false) == 0 && string.Compare(match.Format, Format, false) == 0);
                }
                else if (arrayOrId)
                {
                    if (Format != null)
                    {
                        typeValid = false;
                    }
                }
                else
                {
                    typeValid = false;
                }
            }

            if (!typeValid)
            {
                result.Add(new Violation() { Code = "Type", Context = string.Format("{0}Type", context), Description = @"Required (if $ref is not used). The return type of the operation. The value MUST be one of the Primitives, array or a model's id.", ViolationLevel = ViolationLevel.Error });
            }

            if (Items != null)
            {
                Items.Validate(string.Format("{0}Items.", context), result);
            }
        }
    }
}
