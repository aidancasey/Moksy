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
    /// Is a property greater than the given length. 
    /// </summary>
    public class LengthGreaterThanConstraint : ConstraintBase 
    {
        public LengthGreaterThanConstraint()
        {
            SetupDefaultResponses();
        }

        public LengthGreaterThanConstraint(string propertyName, int length)
        {
            PropertyName = propertyName;
            MinimumLength = length;

            SetupDefaultResponses();
        }

        private void SetupDefaultResponses()
        {
            Response = GreaterThanResponseTemplate;
        }

        [JsonProperty(PropertyName="propertyName")]
        public string PropertyName { get; set; }

        [JsonProperty(PropertyName="minimumLength")]
        public int MinimumLength { get; set; }



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
                return false;
            }
            if (value.Type.ToString() == "Null")
            {
                return false;
            }

            var length = value.ToString().Length;
            bool result = false;

            result = (length > MinimumLength);

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
            pairs["MinimumLength"] = MinimumLength.ToString();
            pairs["ActualLength"] = ActualLength.ToString();
            pairs["Kind"] = "GreaterThan";
            result = s.Substitute(result, pairs);
            return result;
        }

        public const string GreaterThanResponseTemplate = @"{""Name"":""Length"",""PropertyName"":""{PropertyName}"",""Kind"":""{Kind}"",""MinimumLength"":{MinimumLength},""ActualLength"":{ActualLength},""PropertyValue"":{PropertyValue},""PropertyHasValue"":{PropertyHasValue},""Description"":""The property '{PropertyName}' was expected to be longer than '{MinimumLength}' characters.""}";

        public string Response { get; set; }
    }
}
