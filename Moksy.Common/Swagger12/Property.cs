using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Moksy.Common.Swagger.Common;
using Newtonsoft.Json;

namespace Moksy.Common.Swagger12
{
    /// <summary>
    /// A Property Object holds the definition of a new property for a model.
    ///
    /// This object includes the Data Type Fields in order to describe the type of this property. The $ref field MUST be used when linking to other models.
    ///
    /// Properties MUST NOT contain other properties. If there's a need for an internal object hierarchy, additional models MUST be created and linked to a flat structure.
    /// </summary>
    public class Property
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public Property() : this(true)
        {
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="safe">If true, all properties are given a default (safe) value. </param>
        public Property(bool safe)
        {
            Type = "string";
            Format = null;
            Items = new Items();
            
            if (safe)
            {
                Description = "";
                Enums = new string[0];
            }
            else
            {
            }
        }



        /// <summary>
        /// Recommended. A brief description of this property.
        /// </summary>
        [Description(@"Recommended. A brief description of this property.")]
        [JsonProperty(PropertyName="description")]
        public string Description { get; set; }

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
        /// The default value to be used for the field. The value type MUST conform with the primitive's type value.
        /// </summary>
        [Description(@"The default value to be used for the field. The value type MUST conform with the primitive's type value.")]
        [JsonProperty("defaultValue")]
        public object DefaultValue { get; set; }

        /// <summary>
        /// A flag to note whether the container allows duplicate values or not. If the value is set to true, then the array acts as a set.
        /// </summary>
        [Description(@"A flag to note whether the container allows duplicate values or not. If the value is set to true, then the array acts as a set.")]
        [JsonProperty("uniqueItems")]
        public bool UniqueItems { get; set; }

        /// <summary>
        /// The minimum valid value for the type, inclusive. If this field is used in conjunction with the defaultValue field, then the default value MUST be higher than or equal to this value. The value type is string and should represent the minimum numeric value. Note: This will change to a numeric value in the future.
        /// </summary>
        [Description(@"The minimum valid value for the type, inclusive. If this field is used in conjunction with the defaultValue field, then the default value MUST be higher than or equal to this value. The value type is string and should represent the minimum numeric value. Note: This will change to a numeric value in the future.")]
        [JsonProperty("minimum")]
        public string Minimum { get; set; }

        /// <summary>
        /// The maximum valid value for the type, inclusive. If this field is used in conjunction with the defaultValue field, then the default value MUST be lower than or equal to this value. The value type is string and should represent the maximum numeric value. Note: This will change to a numeric value in the future.
        /// </summary>
        [Description(@"The maximum valid value for the type, inclusive. If this field is used in conjunction with the defaultValue field, then the default value MUST be lower than or equal to this value. The value type is string and should represent the maximum numeric value. Note: This will change to a numeric value in the future.")]
        [JsonProperty("maximum")]
        public string Maximum { get; set; }

        /// <summary>
        /// A fixed list of possible values. If this field is used in conjunction with the defaultValue field, then the default value MUST be one of the values defined in the enum.
        /// </summary>
        [Description(@"A fixed list of possible values. If this field is used in conjunction with the defaultValue field, then the default value MUST be one of the values defined in the enum.")]
        [JsonProperty("enum")]
        public string[] Enums { get; set; }

        /// <summary>
        /// Required (if type is not used). The Model to be used. The value MUST be a model's id.
        /// </summary>
        [Description(@"Required (if type is not used). The Model to be used. The value MUST be a model's id.")]
        [JsonProperty(PropertyName="$ref")]
        public string Reference { get; set; }

        /// <summary>
        /// Required. The type definition of the values in the container. A container MUST NOT be nested in another container.
        /// </summary>
        [Description(@"Required. The type definition of the values in the container. A container MUST NOT be nested in another container.")]
        [JsonProperty(PropertyName = "items")]
        public Items Items { get; set; }



        #region Type Recognition

        [JsonIgnore()]
        public bool IsInt32 { get { return IsType("integer", "int32"); }}

        [JsonIgnore()]
        public bool IsInt64 { get { return IsType("integer", "int64"); } }

        [JsonIgnore()]
        public bool IsFloat { get { return IsType("number", "float"); } }

        [JsonIgnore()]
        public bool IsDouble { get { return IsType("number", "double"); } }

        [JsonIgnore()]
        public bool IsString{ get { return IsType("string", null); } }

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

        [JsonIgnore()]
        public bool IsEnum { get { return Enums != null && Enums.Length > 0; } }

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

        #endregion



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
        public static Dictionary<string, PrimitiveTypeFormat> PrimitiveTypeFormats = new Dictionary<string,PrimitiveTypeFormat>(StringComparer.InvariantCulture) { 
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



        /// <summary>
        /// Validate the Property. 
        /// </summary>
        /// <param name="result"></param>
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
            if (IsReference)
            {
                // We cannot validate the refernece from the Property context; the Model context has to validate.
            }
            else
            {
                if (Type == null) typeValid = false;
                if (typeValid && !IsReference)
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
                else
                {
                    typeValid = typeValid && ValidatePropertyIsInCorrectDataType(context, DefaultValue, "DefaultValue", result);
                    typeValid = typeValid && ValidatePropertyIsInCorrectDataType(context, Minimum, "Minimum", result);
                    typeValid = typeValid && ValidatePropertyIsInCorrectDataType(context, Maximum, "Maximum", result);

                    if (typeValid)
                    {
                        typeValid = ValidateDefaultValueIsWithinRange(context, DefaultValue, Minimum, Maximum, result);
                    }
                }

                if (typeValid && IsEnum && DefaultValue != null)
                {
                    var match = Enums.FirstOrDefault(f => string.Compare(f, System.Convert.ToString(DefaultValue), false) == 0);
                    if (match == null)
                    {
                        result.Add(new Violation() { Code = "DefaultValue", Context = string.Format("{0}DefaultValue", context), Description = @"Required (if $ref is not used). The return type of the operation. The value MUST be one of the Primitives, array or a model's id.", ViolationLevel = ViolationLevel.Error });

                    }
                }
            }

            if (Items != null)
            {
                Items.Validate(string.Format("{0}Items.", context), result);
            }
        }

        protected bool ValidatePropertyIsInCorrectDataType(string context, object value, string propertyName, ViolationCollection result)
        {
            try
            {
                if (value == null) return true;

                if (IsInt32) { var v32 = System.Convert.ToInt32(value); }
                if (IsInt64) { var v32 = System.Convert.ToInt64(value); }

                if (IsFloat) { var v32 = System.Convert.ToSingle(value); }
                if (IsDouble) { var v32 = System.Convert.ToDouble(value); }

                if (IsByte) { var v32 = System.Convert.ToByte(value); }

                if (propertyName == "DefaultValue")
                {
                    if (IsBoolean) { var v32 = System.Convert.ToBoolean(value); }
                }

                return true;
            }
            catch (Exception)
            {
                //  The property was not in the correct format. 
                result.Add(new Violation() { Code = propertyName, Context = string.Format("{0}{1}", context, propertyName), Description = string.Format(@"The {0} value is not in the correct format based on the Type.", propertyName), ViolationLevel = ViolationLevel.Error });

                return false;
            }
        }

        protected bool ValidateDefaultValueIsWithinRange(string context, object defaultValue, string minimumValue, string maximumValue, ViolationCollection result)
        {
            if (defaultValue == null) return true;

            try
            {
                if (IsInt32)
                {
                    var d = System.Convert.ToInt32(defaultValue);
                    var min = Int32.MinValue;
                    var max = Int32.MaxValue;
                    if (minimumValue != null) min = System.Convert.ToInt32(minimumValue);
                    if (maximumValue != null) max = System.Convert.ToInt32(maximumValue);
                    if (d < min || d > max)
                    {
                        result.Add(new Violation() { Code = "DefaultValue", Context = string.Format("{0}DefaultValue", context), Description = string.Format(@"The {0} default value is not lie within the Minimum and Maximum range.", "DefaultValue"), ViolationLevel = ViolationLevel.Error });
                    }
                    return true;
                }
                else if (IsInt64)
                {
                    var d = System.Convert.ToInt64(defaultValue);
                    var min = Int64.MinValue;
                    var max = Int64.MaxValue;
                    if (minimumValue != null) min = System.Convert.ToInt64(minimumValue);
                    if (maximumValue != null) max = System.Convert.ToInt64(maximumValue);
                    if (d < min || d > max)
                    {
                        result.Add(new Violation() { Code = "DefaultValue", Context = string.Format("{0}DefaultValue", context), Description = string.Format(@"The {0} default value is not lie within the Minimum and Maximum range.", "DefaultValue"), ViolationLevel = ViolationLevel.Error });
                    }
                    return true;

                }
                else if (IsFloat)
                {
                    var d = System.Convert.ToSingle(defaultValue);
                    var min = Single.MinValue;
                    var max = Single.MaxValue;
                    if (minimumValue != null) min = System.Convert.ToSingle(minimumValue);
                    if (maximumValue != null) max = System.Convert.ToSingle(maximumValue);
                    if (d < min || d > max)
                    {
                        result.Add(new Violation() { Code = "DefaultValue", Context = string.Format("{0}DefaultValue", context), Description = string.Format(@"The {0} default value is not lie within the Minimum and Maximum range.", "DefaultValue"), ViolationLevel = ViolationLevel.Error });
                    }
                    return true;
                }
                else if (IsDouble)
                {
                    var d = System.Convert.ToDouble(defaultValue);
                    var min = Double.MinValue;
                    var max = Double.MaxValue;
                    if (minimumValue != null) min = System.Convert.ToDouble(minimumValue);
                    if (maximumValue != null) max = System.Convert.ToDouble(maximumValue);
                    if (d < min || d > max)
                    {
                        result.Add(new Violation() { Code = "DefaultValue", Context = string.Format("{0}DefaultValue", context), Description = @"A fixed list of possible values. If this field is used in conjunction with the defaultValue field, then the default value MUST be one of the values defined in the enum.", ViolationLevel = ViolationLevel.Error });
                    }
                    return true;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                // This exception should not have been reached - the default, minimum and maximum should already have been validated in a previous call. 
            }

            return false;
        }
    }
}
