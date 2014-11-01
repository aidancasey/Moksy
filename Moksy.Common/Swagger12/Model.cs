using Moksy.Common.Swagger.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common.Swagger12
{
    /// <summary>
    /// A Model Object holds the definition of a new model for this API Declaration.
    /// 
    /// Models in Swagger allow for inheritance. The inheritance is controlled by two fields - subTypes to give the name of the models extending this definition, and discriminator to support polymorphism.
    /// </summary>
    public class Model
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public Model() : this(true)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="safe">If true, default (safe) values are used. </param>
        public Model(bool safe)
        {
            Id = string.Format("New{0}", new Random().Next(1, 100));
            Properties = new Properties();

            if (safe)
            {
                Description = "";
                SubTypes = new string[0];
                Required = new string[0];
            }
            else
            {
            }
        }



        /// <summary>
        /// Required. A unique identifier for the model. This MUST be the name given to {Model Name}.
        /// </summary>
        [Description(@"Required. A unique identifier for the model. This MUST be the name given to {Model Name}.")]
        [JsonProperty(PropertyName="id")]
        public string Id { get; set; }

        /// <summary>
        /// A brief description of this model.
        /// </summary>
        [Description(@"A brief description of this model.")]
        [JsonProperty(PropertyName="description")]
        public string Description { get; set; }

        /// <summary>
        /// Required. A list of properties (fields) that are part of the model
        /// </summary>
        [Description(@"Required. A list of properties (fields) that are part of the model")]
        [JsonProperty("properties")]
        public Properties Properties { get; set; }

        /// <summary>
        /// List of the model ids that inherit from this model. Sub models inherit all the properties of the parent model. Since inheritance is transitive, if the parent of a model inherits from another model, its sub-model will include all properties. As such, if you have Foo->Bar->Baz, then Baz will inherit the properties of Bar and Foo. There MUST NOT be a cyclic definition of inheritance. For example, if Foo -> ... -> Bar, having Bar -> ... -> Foo is not allowed. There also MUST NOT be a case of multiple inheritance. For example, Foo -> Baz <- Bar is not allowed. A sub-model definition MUST NOT override the properties of any of its ancestors. All sub-models MUST be defined in the same API Declaration.
        /// </summary>
        [Description(@"List of the model ids that inherit from this model. Sub models inherit all the properties of the parent model. Since inheritance is transitive, if the parent of a model inherits from another model, its sub-model will include all properties. As such, if you have Foo->Bar->Baz, then Baz will inherit the properties of Bar and Foo. There MUST NOT be a cyclic definition of inheritance. For example, if Foo -> ... -> Bar, having Bar -> ... -> Foo is not allowed. There also MUST NOT be a case of multiple inheritance. For example, Foo -> Baz <- Bar is not allowed. A sub-model definition MUST NOT override the properties of any of its ancestors. All sub-models MUST be defined in the same API Declaration.")]
        [JsonProperty(PropertyName="subTypes")]
        public string[] SubTypes { get; set; }

        /// <summary>
        /// A definition of which properties MUST exist when a model instance is produced. The values MUST be the {Property Name} of one of the properties.
        /// </summary>
        [Description(@"A definition of which properties MUST exist when a model instance is produced. The values MUST be the {Property Name} of one of the properties.")]
        [JsonProperty(PropertyName="required")]
        public string[] Required { get; set; }



        /// <summary>
        /// Validate the model. 
        /// </summary>
        /// <param name="result"></param>
        public void Validate(ViolationCollection result)
        {
            Validate("", result);
        }

        /// <summary>
        /// Validate the Model. 
        /// </summary>
        /// <param name="result"></param>
        public void Validate(string context, ViolationCollection result)
        {
            if (null == result) return;

            if (string.IsNullOrEmpty(Id) || string.IsNullOrWhiteSpace(Id) || Id.Length < 1)
            {
                result.Add(new Violation() { Code = "Id", Context = string.Format("{0}Id", context), Description = @"Required. A unique identifier for the model. This MUST be the name given to {Model Name}.", ViolationLevel = ViolationLevel.Error });
            }

            if (Properties == null)
            {
                result.Add(new Violation() { Code = "Properties", Context = string.Format("{0}Properties", context), Description = @"Required. A list of properties (fields) that are part of the model", ViolationLevel = ViolationLevel.Error });
            }
            else
            {
                foreach (var p in Properties.Data)
                {
                    if (!(p.Value is Property)) continue;

                    var c = string.Format(@"{0}Properties[""{1}""].", context, p.Key);

                    (p.Value as Property).Validate(c, result);
                }
            }

            if (Required != null)
            {
                foreach (var required in Required)
                {
                    if (Properties != null && Properties.Data != null && !Properties.Data.ContainsKey(required))
                    {
                        result.Add(new Violation() { Code = "Required", Context = string.Format(@"{0}Required[""{1}""]", context, required), Description = @"Required. A list of properties (fields) that are part of the model", ViolationLevel = ViolationLevel.Error });
                    }
                }
            }
        }
    }
}
