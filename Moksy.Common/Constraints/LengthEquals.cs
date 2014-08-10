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
    public class LengthEquals : LengthLessThanOrGreaterThan 
    {
        public LengthEquals()
        {
            TreatMissingAsLengthZero = true;
            TreatNullAsLengthZero = true;

            SetupDefaultResponses();
        }

        public LengthEquals(string propertyName, int length)
        {
            TreatMissingAsLengthZero = true;
            TreatNullAsLengthZero = true;

            PropertyName = propertyName;
            ExpectedLength = length;
            
            SetupDefaultResponses();
        }

        public LengthEquals(string propertyName, int length, bool treatMissingAsLengthZero, bool treatNullAsLengthZero)
        {
            TreatMissingAsLengthZero = treatMissingAsLengthZero;
            TreatNullAsLengthZero = treatNullAsLengthZero;

            PropertyName = propertyName;
            ExpectedLength = length;

            SetupDefaultResponses();
        }

        private void SetupDefaultResponses()
        {
            Response = EqualsResponseTemplate;
        }

        [JsonProperty(PropertyName="propertyName")]
        public string PropertyName { get; set; }

        [JsonProperty(PropertyName="expectedLength")]
        public int ExpectedLength { get; set; }

        [JsonProperty(PropertyName="treatMissingAsLengthZero")]
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
                if (ExpectedLength == 0 && TreatMissingAsLengthZero)
                {
                    return true;
                }

                return false;
            }

            if (value.Type.ToString() == "Null")
            {
                if (ExpectedLength == 0 && TreatNullAsLengthZero)
                {
                    return true;
                }

                return false;
            }

            var length = value.ToString().Length;
            bool result = false;

            result = (length == ExpectedLength);
       
            ActualLength = length;

            return result;
        }

        public override string GetState(JObject jobject)
        {
            string result = "";

            result = Response;
         
            Substitution s = new Substitution();
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs["PropertyName"] = PropertyName;
            pairs["PropertyValue"] = GetValue(jobject, PropertyName);
            pairs["PropertyHasValue"] = (jobject[PropertyName] != null).ToString().ToLower();
            pairs["ExpectedLength"] = ExpectedLength.ToString();
            pairs["ActualLength"] = ActualLength.ToString();
            pairs["Kind"] = "Equals";
            result = s.Substitute(result, pairs);
            return result;
        }

        public const string EqualsResponseTemplate = @"{""Name"":""Length"",""PropertyName"":""{PropertyName}"",""Kind"":""{Kind}"",""ExpectedLength"":{ExpectedLength},""ActualLength"":{ActualLength},""PropertyValue"":{PropertyValue},""PropertyHasValue"":{PropertyHasValue},""Description"":""The property '{PropertyName}' was expected to be of length '{ExpectedLength}'.""}";

        public string Response { get; set; }
    }
}
