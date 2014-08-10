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
    /// Is a property less than the given length. 
    /// </summary>
    public class LengthLessThan : ConstraintBase 
    {
        public LengthLessThan()
        {
            TreatMissingAsLengthZero = true;
            TreatNullAsLengthZero = true;

            SetupDefaultResponses();
        }

        public LengthLessThan(string propertyName, int length)
        {
            TreatMissingAsLengthZero = true;
            TreatNullAsLengthZero = true;

            PropertyName = propertyName;
            MinimumLength = length;

            SetupDefaultResponses();
        }

        public LengthLessThan(string propertyName, int length, bool treatMissingAsLengthZero, bool treatNullAsLengthZero)
        {
            TreatMissingAsLengthZero = treatMissingAsLengthZero;
            TreatNullAsLengthZero = treatNullAsLengthZero;

            PropertyName = propertyName;
            MinimumLength = length;

            SetupDefaultResponses();
        }

        private void SetupDefaultResponses()
        {
            LessThanResponse = LessThanResponseTemplate;
        }

        [JsonProperty(PropertyName="propertyName")]
        public string PropertyName { get; set; }

        [JsonProperty(PropertyName="minimumLength")]
        public int MinimumLength { get; set; }

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
                if (TreatMissingAsLengthZero)
                {
                    ActualLength = 0;
                    return 0 < MinimumLength;
                }

                return false;
            }
            if (value.Type.ToString() == "Null")
            {
                return false;
            }

            var length = value.ToString().Length;
            bool result = false;

            result = (length < MinimumLength);
          
            ActualLength = length;

            return result;
        }

        public override string GetState(JObject jobject)
        {
            string result = "";
            
            result = LessThanResponse;
            
            Substitution s = new Substitution();
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs["PropertyName"] = PropertyName;
            pairs["PropertyValue"] = GetValue(jobject, PropertyName);
            pairs["PropertyHasValue"] = (jobject[PropertyName] != null).ToString().ToLower();
            pairs["MinimumLength"] = MinimumLength.ToString();
            pairs["ActualLength"] = ActualLength.ToString();
            pairs["Kind"] = "LessThan";
            result = s.Substitute(result, pairs);
            return result;
        }

        public const string LessThanResponseTemplate = @"{""Name"":""Length"",""PropertyName"":""{PropertyName}"",""Kind"":""{Kind}"",""MinimumLength"":{MinimumLength},""ActualLength"":{ActualLength},""PropertyValue"":{PropertyValue},""PropertyHasValue"":{PropertyHasValue},""Description"":""The property '{PropertyName}' was expected to be less than '{MinimumLength}' characters.""}";

        public string LessThanResponse { get; set; }
    }
}
