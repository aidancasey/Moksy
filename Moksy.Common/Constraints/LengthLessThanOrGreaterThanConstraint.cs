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
    /// Is a property less than or greater than a given length. 
    /// </summary>
    public class LengthLessThanOrGreaterThanConstraint : ConstraintBase 
    {
        public LengthLessThanOrGreaterThanConstraint()
        {
            TreatMissingAsLengthZero = true;
            TreatNullAsLengthZero = true;

            SetupDefaultResponses();
        }

        public LengthLessThanOrGreaterThanConstraint(string propertyName, int minimum, int maximum)
        {
            TreatMissingAsLengthZero = true;
            TreatNullAsLengthZero = true;

            PropertyName = propertyName;
            MinimumLength = minimum;
            MaximumLength = maximum;

            SetupDefaultResponses();
        }

        public LengthLessThanOrGreaterThanConstraint(string propertyName, int minimum, int maximum, bool treatMissingAsLengthZero, bool treatNullAsLengthZero)
        {
            TreatMissingAsLengthZero = treatMissingAsLengthZero;
            TreatNullAsLengthZero = treatNullAsLengthZero;

            PropertyName = propertyName;
            MinimumLength = minimum;
            MaximumLength = maximum;

            SetupDefaultResponses();
        }

        private void SetupDefaultResponses()
        {
            EqualsLessThanOrGreaterThanResponse = EqualsLessThanOrGreaterThanResponseTemplate;
        }

        [JsonProperty(PropertyName="propertyName")]
        public string PropertyName { get; set; }

        [JsonProperty(PropertyName="minimumLength")]
        public int MinimumLength { get; set; }

        [JsonProperty(PropertyName="maximumLength")]
        public int MaximumLength { get; set; }

        [JsonProperty(PropertyName = "treatMissingAsLengthZero")]
        public bool TreatMissingAsLengthZero { get; set; }

        [JsonProperty(PropertyName = "treatNullAsLengthZero")]
        public bool TreatNullAsLengthZero { get; set; }



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
            if (value == null)
            {
                if (TreatMissingAsLengthZero && MinimumLength >= 0)
                {
                    return true;
                }

                return false;
            }
            if (value.Type.ToString() == "Null")
            {
                if (TreatNullAsLengthZero && MinimumLength >= 0)
                {
                    return true;
                }

                return false;
            }

            var length = value.ToString().Length;
            bool result = false;

            result = (length < MinimumLength || length > MaximumLength);

            ActualLength = length;

            return result;
        }

        public override string GetState(JObject jobject)
        {
            string result = "";
            
            result = EqualsLessThanOrGreaterThanResponse;

            Substitution s = new Substitution();
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs["PropertyName"] = PropertyName;
            pairs["PropertyValue"] = GetValue(jobject, PropertyName);
            pairs["PropertyHasValue"] = (jobject[PropertyName] != null).ToString().ToLower();

            pairs["MinimumLength"] = MinimumLength.ToString();
            pairs["MaximumLength"] = MaximumLength.ToString();

            pairs["ActualLength"] = ActualLength.ToString();
            pairs["Kind"] = "LessThanOrGreaterThan";
            result = s.Substitute(result, pairs);
            return result;
        }

        public const string EqualsLessThanOrGreaterThanResponseTemplate = @"{""Name"":""Length"",""PropertyName"":""{PropertyName}"",""Kind"":""{Kind}"",""MinimumLength"":{MinimumLength},""MaximumLength"":{MaximumLength},""ActualLength"":{ActualLength},""PropertyValue"":{PropertyValue},""PropertyHasValue"":{PropertyHasValue},""Description"":""The property '{PropertyName}' was expected to be less than '{MinimumLength}' characters or greater than '{MaximumLength}' characters in length.""}";

        public string EqualsLessThanOrGreaterThanResponse { get; set; }
    }
}
