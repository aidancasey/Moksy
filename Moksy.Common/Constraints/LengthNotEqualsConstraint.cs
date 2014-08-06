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
    /// Is a property not of the given length. 
    /// </summary>
    public class LengthNotEqualsConstraint : LengthLessThanOrGreaterThanConstraint 
    {
        public LengthNotEqualsConstraint()
        {
            SetupDefaultResponses();
        }

        public LengthNotEqualsConstraint(string propertyName, int length)
        {
            PropertyName = propertyName;
            ExpectedLength = length;

            SetupDefaultResponses();
        }

        private void SetupDefaultResponses()
        {
            NotEqualsResponse = NotEqualsResponseTemplate;
        }

        [JsonProperty(PropertyName="propertyName")]
        public string PropertyName { get; set; }

        [JsonProperty(PropertyName="expectedLength")]
        public int ExpectedLength { get; set; }



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
                return true;
            }
            if (value.Type.ToString() == "Null")
            {
                return true;
            }

            var length = value.ToString().Length;
            bool result = false;

            result = (length != ExpectedLength);

            ActualLength = length;

            return result;
        }

        public override string GetState(JObject jobject)
        {
            string result = "";
            
            result = NotEqualsResponse;

            Substitution s = new Substitution();
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs["PropertyName"] = PropertyName;
            pairs["PropertyValue"] = GetValue(jobject, PropertyName);
            pairs["PropertyHasValue"] = (jobject[PropertyName] != null).ToString().ToLower();
            pairs["ExpectedLength"] = ExpectedLength.ToString();
            pairs["ActualLength"] = ActualLength.ToString();
            pairs["Kind"] = "NotEquals";
            result = s.Substitute(result, pairs);
            return result;
        }

        public const string NotEqualsResponseTemplate = @"{""Name"":""Length"",""PropertyName"":""{PropertyName}"",""Kind"":""{Kind}"",""ExpectedLength"":{ExpectedLength},""ActualLength"":{ActualLength},""PropertyValue"":{PropertyValue},""PropertyHasValue"":{PropertyHasValue},""Description"":""The property '{PropertyName}' was expected to not be of length '{ExpectedLength}'.""}";

        public string NotEqualsResponse { get; set; }
    }
}
