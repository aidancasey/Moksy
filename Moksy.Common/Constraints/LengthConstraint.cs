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
    /// Is a property of the given length. 
    /// </summary>
    public class LengthConstraint : ConstraintBase 
    {
        public LengthConstraint()
        {
            Kind = LengthKind.Equals;
        }

        public LengthConstraint(string propertyName, int length)
        {
            PropertyName = propertyName;
            MinimumLength = length;
            Kind = LengthKind.Equals;
        }

        public LengthConstraint(string propertyName, int length, LengthKind kind)
        {
            PropertyName = propertyName;
            MinimumLength = length;
            Kind = kind;
        }

        public LengthConstraint(string propertyName, int minimum, int maximum)
        {
            PropertyName = propertyName;
            MinimumLength = minimum;
            MaximumLength = maximum;
            Kind = LengthKind.LessThanOrGreaterThan;
        }

        [JsonProperty(PropertyName="propertyName")]
        public string PropertyName { get; set; }

        [JsonProperty(PropertyName="length")]
        public int MinimumLength { get; set; }

        [JsonProperty(PropertyName="upperLength")]
        public int MaximumLength { get; set; }

        [JsonProperty(PropertyName="kind")]
        public LengthKind Kind { get; set; }



        /// <summary>
        /// Will hold the Actual Length of the string after evaluation. 
        /// </summary>
        [JsonIgnore]
        public int ActualLength { get; set; }



        public override bool Evaluate(Newtonsoft.Json.Linq.JObject jobject)
        {
            ActualLength = 0;

            if (null == PropertyName) return false;
            if (null == jobject) return false;

            var value = jobject[PropertyName];
            if (value == null) return false;
            if (value.Type.ToString() == "Null") return false;

            var length = value.ToString().Length;
            bool result = false;

            if (Kind == LengthKind.Equals)
            {
                result = (length == MinimumLength);
            }
            else if (Kind == LengthKind.LessThan)
            {
                result = (length < MinimumLength);
            }
            else if (Kind == LengthKind.GreaterThan)
            {
                result = (length > MinimumLength);
            }
            else if (Kind == LengthKind.LessThanOrGreaterThan)
            {
                result = (length < MinimumLength || length > MaximumLength);
            }
            else if (Kind == LengthKind.NotEquals)
            {
                result = (length != MinimumLength);
            }

            ActualLength = length;

            return result;
        }
    }
}
