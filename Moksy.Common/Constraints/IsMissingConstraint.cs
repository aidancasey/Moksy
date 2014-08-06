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
    /// Is a property in the object missing?
    /// </summary>
    public class IsMissingConstraint : ConstraintBase 
    {
        public IsMissingConstraint()
        {
            SetupDefaultResponses();
        }

        public IsMissingConstraint(string propertyName)
        {
            PropertyName = propertyName;

            SetupDefaultResponses();
        }

        private void SetupDefaultResponses()
        {
            Response = MissingResponseTemplate;
        }

        [JsonProperty(PropertyName="propertyName")]
        public string PropertyName { get; set; }

        public override bool Evaluate(Newtonsoft.Json.Linq.JObject jobject)
        {
            if (null == PropertyName) return false;
            if (null == jobject) return false;

            var value = jobject[PropertyName];
            if (value == null) return true;

            return false;
        }

        public override string GetState(Newtonsoft.Json.Linq.JObject jobject)
        {
            var result = Response;
            Substitution s = new Substitution();
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs["PropertyName"] = PropertyName;
            pairs["PropertyValue"] = GetValue(jobject, PropertyName);
            pairs["PropertyHasValue"] = (jobject[PropertyName] != null).ToString().ToLower();
            result = s.Substitute(result, pairs);
            return result;
        }

        public const string MissingResponseTemplate = @"{""Name"":""IsMissing"",""PropertyName"":""{PropertyName}"",""PropertyValue"":{PropertyValue},""PropertyHasValue"":{PropertyHasValue},""Description"":""The property '{PropertyName}' was not expected to be in the Json. ""}";

        public string Response { get; set; }
    }
}
