using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Moksy.Common.Swagger.Common;

namespace Moksy.Common.Swagger12
{
    /// <summary>
    /// Properties. 
    /// </summary>
    public class Properties
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public Properties() : this(true)
        {
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="safe">If true, none-null values are given a safe default. </param>
        public Properties(bool safe)
        {
            Data = new Dictionary<string, object>();

            if (safe)
            {
            }
            else
            {
            }
        }

        /// <summary>
        /// Property information. 
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> Data { get; set; }



        /// <summary>
        /// Validate the properties.
        /// </summary>
        /// <param name="result"></param>
        public void Validate(ViolationCollection result)
        {
            if (null == result) return;

        }
    }
}
