using Moksy.Common.Constraints;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common
{
    /// <summary>
    /// Holds the content and variables for the SimulationCondition. 
    /// </summary>
    public class SimulationConditionContent
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public SimulationConditionContent()
        {
            Repeat = Int64.MaxValue;
        }



        /// <summary>
        /// If not null, then anything posted to the Imdb will be grouped by the value of the header provided. The header 
        /// must be part of the submission 
        /// </summary>
        [JsonProperty(PropertyName = "imdbDiscriminator")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ImdbHeaderDiscriminator { get; set; }

        /// <summary>
        /// If not null, contains the name of the property that is used for uniqueness on Post operations and as a key on Delete, Put, Patch and Get operations. 
        /// </summary>
        [JsonProperty("indexProperty")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IndexProperty { get; set; }

        /// <summary>
        /// The number of times this condition is to repeat before it is discarded. 
        /// </summary>
        [JsonProperty("repeat")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public long Repeat { get; set; }

        /// <summary>
        /// The kind of content this condition will send as. ie: Text, or Json. 
        /// </summary>
        [JsonProperty("contentKind")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ContentKind ContentKind { get; set; }

        [JsonProperty("persistence")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Persistence Persistence { get; set; }

        /// <summary>
        /// If true, the response includes adding this entry to the in memory database or removing it. 
        /// </summary>
        [JsonProperty(PropertyName = "isImdb")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsImdb { get; set; }

        /// <summary>
        /// The path / route / pattern that must be matched for this simulation to occur. ie: /Pet, or /Pet/{Kind} etc.
        /// </summary>
        [JsonProperty(PropertyName = "pattern")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Pattern { get; set; }

        /// <summary>
        /// The HttpMethod the Request applies to. Typically: Get, Post, Delete. 
        /// </summary>
        [JsonProperty("httpMethod")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public System.Net.Http.HttpMethod HttpMethod { get; set; }

        [JsonProperty("constraints")]
        public ConstraintBase[] ConstraintsForJson
        {
            get
            {
                return ConstraintStorage.ToArray();
            }
            set
            {
                ConstraintStorage = new List<ConstraintBase>();
                if (null == value) return;
                ConstraintStorage.AddRange(value);
            }
        }

        [JsonProperty("contains")]
        public ContentRule[] ContainsForJson
        {
            get
            {
                return ContainsStorage.ToArray();
            }
            set
            {
                ContainsStorage = new List<ContentRule>();
                if (null == value) return;
                ContainsStorage.AddRange(value);
            }
        }


        [JsonProperty("requestHeaders")]
        public Header[] RequestHeadersForJson
        {
            get
            {
                return RequestHeadersStorage.ToArray();
            }
            set
            {
                RequestHeadersStorage = new List<Header>();
                if (null == value) return;
                RequestHeadersStorage.AddRange(value);
            }
        }

        [JsonProperty("parameters")]
        public Parameter[] ParametersForJson
        {
            get
            {
                return ParametersStorage.ToArray();
            }
            set
            {
                ParametersStorage = new List<Parameter>();
                if (null == value) return;
                ParametersStorage.AddRange(value);
            }
        }

        /// <summary>
        /// Storage for all constraints. 
        /// </summary>
        protected internal List<ConstraintBase> ConstraintStorage = new List<ConstraintBase>();

        /// <summary>
        /// Storage for the header
        /// </summary>
        protected internal List<Header> RequestHeadersStorage = new List<Header>();

        /// <summary>
        /// Storage for the parameters. 
        /// </summary>
        protected internal List<Parameter> ParametersStorage = new List<Parameter>();

        /// <summary>
        /// Storage for all content rules
        /// </summary>
        protected internal List<ContentRule> ContainsStorage = new List<ContentRule>();

        [JsonProperty(PropertyName = "hasConstraintViolations")]
        public bool HasAnyConstraintViolations { get; set; }
    }
}
