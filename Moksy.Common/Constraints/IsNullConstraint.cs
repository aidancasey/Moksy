using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common.Constraints
{
    /// <summary>
    /// Is a property in the object Null?
    /// </summary>
    public class IsNullConstraint : ConstraintBase 
    {
        public IsNullConstraint()
        {
        }

        public IsNullConstraint(string propertyName)
        {
            PropertyName = propertyName;
        }

        [JsonProperty(PropertyName="propertyName")]
        public string PropertyName { get; set; }

        public override bool Evaluate(Newtonsoft.Json.Linq.JObject jobject)
        {
            if (null == PropertyName) return false;
            if (null == jobject) return false;

            var value = jobject[PropertyName];
            if (value == null) return false;

            return value.Type.ToString() == "Null";
        }
    }
}
