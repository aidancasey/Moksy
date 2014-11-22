using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common
{
    public class SimulationResponseContent
    {
        public SimulationResponseContent()
        {
            HttpStatusCode = System.Net.HttpStatusCode.Unused;
            ResponseHeadersStorage = new List<Header>();
            VariablesStorage = new List<Variable>();
            PropertiesStorage = new List<Property>();
        }

        /// <summary>
        /// Contains the text that will be returned in the response body for this simulation if ContentType == Text
        /// </summary>
        /// <remarks>We need much better control of this - currently, it is just serialized as a string. We need extra parameters such as returning raw
        /// bytes; Unicode-encoded; and so forth. 
        /// </remarks>
        [JsonProperty(PropertyName = "content")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Content { get; set; }

        /// <summary>
        /// Contains the bytes that will be returned in the response body for this simulation if ContentType == Byte
        /// </summary>
        /// <remarks>We need much better control of this - currently, it is just serialized as a string. We need extra parameters such as returning raw
        /// bytes; Unicode-encoded; and so forth. 
        /// </remarks>
        [JsonProperty(PropertyName = "contentAsBytes")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Byte[] ContentAsBytes { get; set; }

        /// <summary>
        /// The status code that will be set by this response. 
        /// </summary>
        [JsonProperty(PropertyName = "statusCode")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public System.Net.HttpStatusCode HttpStatusCode { get; set; }

        /// <summary>
        /// If true, then the value that is Posted will be added to Imdb. It does not make sense to set this property if the operation is NOT post. 
        /// </summary>
        [JsonProperty(PropertyName = "addToImdb")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool AddImdb { get; set; }

        /// <summary>
        /// If true, then the value that is Deleted will be removed from the Imdb. It does not make sense to set this property if the operation is NOT delete. 
        /// </summary>
        [JsonProperty(PropertyName = "removeFromImdb")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool RemoveImdb { get; set; }

        /// <summary>
        /// The type of content that is to be returned. Currently, if not Octet, it is assumed to be a string type. 
        /// </summary>
        [JsonProperty("contentKind")]
        public ContentKind ContentKind { get; set; }

        protected internal List<Header> ResponseHeadersStorage;

        [JsonProperty("requestHeaders")]
        public Header[] ResponseHeadersForJson
        {
            get
            {
                return ResponseHeadersStorage.ToArray();
            }
            set
            {
                ResponseHeadersStorage = new List<Header>();
                if (null == value) return;
                ResponseHeadersStorage.AddRange(value);
            }
        }

        [JsonProperty("properties")]
        public Property[] PropertiesForJson
        {
            get
            {
                return PropertiesStorage.ToArray();
            }
            set
            {
                PropertiesStorage = new List<Property>();
                if (null == value) return;
                PropertiesStorage.AddRange(value);
            }
        }
        protected internal List<Property> PropertiesStorage;


        [JsonProperty("variables")]
        public Variable[] VariablesForJson
        {
            get
            {
                return VariablesStorage.ToArray();
            }
            set
            {
                VariablesStorage = new List<Variable>();
                if (null == value) return;
                VariablesStorage.AddRange(value);
            }
        }
        protected internal List<Variable> VariablesStorage;
    }
}
