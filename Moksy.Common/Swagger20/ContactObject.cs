using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common.Swagger20
{
    /// <summary>
    /// Contact information for the exposed API.
    /// </summary>
    public class ContactObject
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public ContactObject() : this(true)
        {
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="safe">If true, all values are set to none-null values.</param>
        public ContactObject(bool safe)
        {
            if (safe)
            {
                Name = "";
                Url = "http://localhost/contact.html";
                Email = "a@b.com";
            }
            else
            {

            }
        }

        [JsonProperty(PropertyName="name")]
        [Description(@"The identifying name of the contact person/organization.")]
        public string Name {get;set;}

        [JsonProperty(PropertyName="url")]
        [Description(@"The URL pointing to the contact information. MUST be in the format of a URL.")]
        public string Url {get;set;}

        [JsonProperty(PropertyName="email")]
        [Description(@"The email address of the contact person/organization. MUST be in the format of an email address.")]
        public string Email {get;set;}
    }
}
