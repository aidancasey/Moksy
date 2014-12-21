using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common.Swagger.Common
{
    /// <summary>
    /// Helpers for performing validation. 
    /// </summary>
    public static class ValidationHelpers
    {
        /// <summary>
        /// Validate that a Url is required. 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="context"></param>
        /// <param name="description"></param>
        /// <param name="result"></param>
        public static void ValidateRequiredUrl(string url, string context, string description, UriKind kind, ViolationCollection result)
        {
            if (null == result) return;

            var valid = false;

            try
            {
                Uri uri = new Uri(url, kind);

                valid = true;
            }
            catch (Exception)
            {

            }

            if (!valid)
            {
                result.Add(new Violation() { Code = context, Context = context, ViolationLevel = ViolationLevel.Error, Description = description });
            }
        }

        /// <summary>
        /// Is the NickName valid. Must be Alphanumeric, can contain underscores but not whitespace. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsValidNickName(string value)
        {
            if (null == value) return false;
            if (value.Length == 0) return false;
            if (value.Contains(" ")) return false;

            foreach (var c in value)
            {
                if (Char.IsLetterOrDigit(c) || c == '_') continue;

                return false;
            }

            return true;
        }



        /// <summary>
        /// Returns true if the parameter name is valid (based on the ParamType and other contexts). 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static bool IsParameterNameValid(Moksy.Common.Swagger12.Api api, Moksy.Common.Swagger12.Parameter parameter)
        {
            if (null == parameter) return false;
            
            if (api != null)
            {
                if (api.Path != parameter.Name)
                {
                    return false;
                }
            }

            if (parameter.ParamType == "query" && string.IsNullOrEmpty(parameter.Name)) return false;
            if (parameter.ParamType == "body" && parameter.Name != "body") return false;
            if (parameter.ParamType == "header" && string.IsNullOrEmpty(parameter.Name)) return false;
            if (parameter.ParamType == "form" && string.IsNullOrEmpty(parameter.Name)) return false;

            return true;
        }
    }
}
