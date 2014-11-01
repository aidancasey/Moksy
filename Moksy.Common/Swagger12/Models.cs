using Moksy.Common.Swagger.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common.Swagger12
{
    /// <summary>
    /// The Models Object holds a field per model definition, and this is different than the structure of the other objects in the spec. It follows a subset of the JSON-Schema specification.
    ///
    /// Please note that the Models Object is an object containing other object definitions and as such is structured as follows:
    /// </summary>
    public class Models
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public Models()
        {
            Data = new Dictionary<string, object>();
        }

        /// <summary>
        /// Models information. 
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> Data { get; set; }



        /// <summary>
        /// Models validation. 
        /// </summary>
        /// <param name="result"></param>
        public void Validate(ViolationCollection result)
        {
            if (null == result) return;
            if (null == Data) return;

            foreach (var pair in Data)
            {
                Model model = pair.Value as Model;
                if (null != model)
                {
                    model.Validate(string.Format(@"[""{0}""].", pair.Key), result);
                }

                bool valid = true;

                if (pair.Value == null || !(pair.Value is Model)) valid = false;

                if(valid)
                {
                    if (string.Compare( (pair.Value as Model).Id, pair.Key, false) != 0)
                    {
                        valid = false;
                    }
                }

                if (!valid)
                {
                    var context = string.Format(@"[""{0}""].Id", pair.Key);

                    result.Add(new Violation() { Code = "Id", Context = context, Description = @"The Models.Id property must be the same as the Model.Id property. ", ViolationLevel = ViolationLevel.Error });
                }

                if (null != model)
                {
                    if (model.Properties != null && model.Properties.Data != null)
                    {
                        foreach (var propertyPair in model.Properties.Data)
                        {
                            var property = propertyPair.Value as Property;
                            if (null == property) continue;

                            if (property.IsReference)
                            {
                                var refMatchExists = Data.ContainsKey(property.Reference);
                                if (!refMatchExists)
                                {
                                    var c = string.Format(@"[""{0}""].Properties[""{1}""].Reference", pair.Key, propertyPair.Key);
                                    result.Add(new Violation() { Code = "Reference", Context = c, Description = @"The Models.Id property must be the same as the Model.Id property. ", ViolationLevel = ViolationLevel.Error });
                                }
                            }
                            if (property.Items != null && property.Items.IsReference)
                            {
                                var refMatchExists = Data.ContainsKey(property.Items.Reference);
                                if (!refMatchExists)
                                {
                                    var c = string.Format(@"[""{0}""].Properties[""{1}""].Items.Reference", pair.Key, propertyPair.Key);
                                    result.Add(new Violation() { Code = "Reference", Context = c, Description = @"Required (if type is not used). The Model to be used. The value MUST be a model's id. ", ViolationLevel = ViolationLevel.Error });
                                }
                            }
                        }
                    }

                    if (model.SubTypes != null)
                    {
                        var description = @"List of the model ids that inherit from this model. Sub models inherit all the properties of the parent model. Since inheritance is transitive, if the parent of a model inherits from another model, its sub-model will include all properties. As such, if you have Foo->Bar->Baz, then Baz will inherit the properties of Bar and Foo. There MUST NOT be a cyclic definition of inheritance. For example, if Foo -> ... -> Bar, having Bar -> ... -> Foo is not allowed. There also MUST NOT be a case of multiple inheritance. For example, Foo -> Baz <- Bar is not allowed. A sub-model definition MUST NOT override the properties of any of its ancestors. All sub-models MUST be defined in the same API Declaration.";

                        foreach (var subType in model.SubTypes)
                        {
                            var isMe = string.Compare(subType, model.Id, false) == 0;
                            if (isMe)
                            {
                                var self = string.Format("A SubType cannot reference the same model. \r\n\r\n{0}", description);

                                var c = string.Format(@"[""{0}""].SubTypes[""{1}""]", model.Id, subType);
                                result.Add(new Violation() { Code = "SubTypes", Context = c, Description = self, ViolationLevel = ViolationLevel.Error });

                                continue;
                            }
                            if (subType == null || !Data.ContainsKey(subType))
                            {
                                var missing = string.Format("A SubType must reference an existing model. \r\n\r\n{0}", description);

                                var c = string.Format(@"[""{0}""].SubTypes[""{1}""]", model.Id, subType);
                                result.Add(new Violation() { Code = "SubTypes", Context = c, Description = missing, ViolationLevel = ViolationLevel.Error });
                            }
                            if (subType != null)
                            {
                                var subTypeModel = SubtypesMe(model.Id);
                                if (subTypeModel != null)
                                {
                                    // This means that a subtype (or a subtype of a subtype) is referencing me. A circular reference is not allowed.
                                    var c = string.Format(@"[""{1}""].SubTypes[""{0}""]", subTypeModel, model.Id);
                                    result.Add(new Violation() { Code = "SubTypes", Context = c, Description = "A circular reference is not allowed. A SubType of a model either references the model or one or its descendents. ", ViolationLevel = ViolationLevel.Error });
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the name of a model whose 
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public string SubtypesMe(string me)
        {
            return SubtypesMe(me, new string[0]);
        }

        /// <summary>
        /// Returns the name of a model whose 
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public string SubtypesMe(string me, IEnumerable<string> descendents)
        {
            var model = Data[me] as Model;
            if (null == model) return null;

            return SubtypesMe(me, model.SubTypes, descendents);
        }

        /// <summary>
        /// Returns the model whose subType references me. 
        /// </summary>
        /// <param name="me"></param>
        /// <param name="subTypes"></param>
        /// <returns></returns>
        public string SubtypesMe(string me, string[] subTypes, IEnumerable<string> descendents)
        {
            if (null == subTypes) return null;

            foreach (var subType in subTypes)
            {
                // We cannot be a subType of ourself. 
                if (subType == me) return me;

                // We are are anywhere the descendents; bail. 
                if(descendents.FirstOrDefault(f => f == subType) != null)
                {
                    return subType;
                }

                if (!Data.ContainsKey(subType)) continue;

                var subTypeModel = Data[subType] as Model;
                if(subTypeModel == null) continue;

                List<string> untilNow = new List<string>(descendents);
                untilNow.Add(subType);

                var result = SubtypesMe(me, subTypeModel.SubTypes, untilNow);
                if (result != null) return subType;
            }

            return null;
        }
    }
}
