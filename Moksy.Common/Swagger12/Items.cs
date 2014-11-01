using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Newtonsoft.Json;
using Moksy.Common.Swagger.Common;

namespace Moksy.Common.Swagger12
{
    /// <summary>
    /// This object is used to describe the value types used inside an array. Of the Data Type Fields, it can include either the type and format fields OR the $ref field (when referencing a model). The rest of the listed Data Type fields are not applicable.
    ///
    /// If the type field is included it MUST NOT have the value array. There's currently no support for containers within containers.
    /// </summary>
    public class Items
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public Items() : this(true)
        {
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="safe">If true, sensible defaults will be provided. </param>
        public Items(bool safe)
        {
            Type = "string";

            if (safe)
            {
            }
            else
            {

            }
        }



        /// <summary>
        /// Required (if $ref is not used). The return type of the operation. The value MUST be one of the Primitives, array or a model's id.
        /// </summary>
        [Description(@"Required (if $ref is not used). The return type of the operation. The value MUST be one of the Primitives, array or a model's id.")]
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Fine-tuned primitive type definition. See Primitives for further information. The value MUST be one that is defined under Primitives, corresponding to the right primitive type.
        /// </summary>
        [Description(@"Fine-tuned primitive type definition. See Primitives for further information. The value MUST be one that is defined under Primitives, corresponding to the right primitive type.")]
        [JsonProperty("format")]
        public string Format { get; set; }

        /// <summary>
        /// Required (if type is not used). The Model to be used. The value MUST be a model's id.
        /// </summary>
        [Description(@"Required (if type is not used). The Model to be used. The value MUST be a model's id.")]
        [JsonProperty(PropertyName = "$ref")]
        public string Reference { get; set; }



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
        public bool IsReference { get { return Type == null && Reference != null; } }



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



        public void Validate(ViolationCollection result)
        {
            Validate("", result);
        }

        /// <summary>
        /// Validate the Property. 
        /// </summary>
        /// <param name="result"></param>
        public void Validate(string context, ViolationCollection result)
        {
            if (null == result) return;

            var typeValid = true;
            if (Type == null && !IsReference) typeValid = false;
            if (typeValid)
            {
                var primitives = from T in PrimitiveTypeFormats select T.Value;
                var knownPrimitive = primitives.FirstOrDefault(f => string.Compare(f.Type, Type, false) == 0 && string.Compare(f.Format, Format, false) == 0) != null;
                var isId = !knownPrimitive && IsReference;

                if (knownPrimitive)
                {
                    var match = primitives.FirstOrDefault(f => string.Compare(f.Type, Type, false) == 0 && string.Compare(f.Format, Format, false) == 0);
                    typeValid = (string.Compare(match.Type, Type, false) == 0 && string.Compare(match.Format, Format, false) == 0);
                }
                else if (isId)
                {

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
        }
    }
}
