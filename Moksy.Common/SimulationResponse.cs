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
    /// Represents a response to be constructed by Moksy when a SimulationCondition is met. 
    /// </summary>
    public class SimulationResponse
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public SimulationResponse()
        {
            HttpStatusCode = System.Net.HttpStatusCode.Unused;
            ResponseHeadersStorage = new List<Header>();
        }

        /// <summary>
        /// Specifies the text that will be returned in the body for this simulation. 
        /// </summary>
        /// <param name="content">The content. Can be null or empty string. The default is {value} which is the sensible default and will contain the value from 
        /// the Imdb if using ToImdb or FromImdb. </param>
        /// <returns></returns>
        /// <remarks>The content parameter can contain free text and {value}. For example: you can choose to wrap responses in additional Json. For example:
        /// Some text {value} that wraps the value. 
        /// </remarks>
        public SimulationResponse Body(string content)
        {
            Content = content;
            return this;
        }

        /// <summary>
        /// Specifies the object that will be returned in the body for this simulation. Must be serializable as Json. 
        /// </summary>
        /// <param name="content">The object to be serialized as Json. </param>
        /// <returns></returns>
        /// <remarks>The content parameter can contain free text and {value}. For example: you can choose to wrap responses in additional Json. For example:
        /// Some text {value} that wraps the value. 
        /// </remarks>
        public SimulationResponse Body(object o)
        {
            if (null == o)
            {
                Content = null;
            }
            else
            {
                var json = JsonConvert.SerializeObject(o);
                Content = json;
            }
            return this;
        }

        /// <summary>
        /// Adds a single header that will be returned as part of the response. 
        /// </summary>
        /// <param name="name">Must be none-null. For example: Content-Type</param>
        /// <param name="value">Can be empty or null (will be placed as empty in the request). </param>
        /// <returns></returns>
        public SimulationResponse Header(string name, string value)
        {
            Header header = new Header(name, value);
            ResponseHeadersStorage.Add(header);
            return this;
        }

        /// <summary>
        /// Adds a single header that will be returned as part of the response. 
        /// </summary>
        /// <param name="header">The name/value pair ot be returned as part of the response. </param>
        /// <returns></returns>
        public SimulationResponse Header(Header header)
        {
            if (null == header) return this;
            ResponseHeadersStorage.Add(header);
            return this;
        }

        /// <summary>
        /// Adds a number of headers to the response. 
        /// </summary>
        /// <param name="headers">A number of name/value pairs to be returned as part of the response. </param>
        /// <returns></returns>
        public SimulationResponse Headers(IEnumerable<Header> headers)
        {
            if (null == headers) return this;
            ResponseHeadersStorage.AddRange(headers);
            return this;
        }

        /// <summary>
        /// Specifies the status code that will be returned for this simulation. 
        /// </summary>
        /// <param name="statusCode">The status code to return. </param>
        /// <returns></returns>
        public SimulationResponse StatusCode(System.Net.HttpStatusCode statusCode)
        {
            this.HttpStatusCode = statusCode;
            return this;
        }

        /// <summary>
        /// A collection of headers that will be returned. 
        /// </summary>
        [JsonIgnore]
        public List<Header> ResponseHeaders
        {
            get
            {
                var result = new List<Header>(ResponseHeadersStorage);
                return result;
            }
            set
            {
                if (value == null)
                {
                    ResponseHeadersStorage = new List<Header>();
                    return;
                }

                ResponseHeadersStorage.Clear();
                ResponseHeadersStorage.AddRange(value);
            }
        }
        private List<Header> ResponseHeadersStorage;

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

        /// <summary>
        /// Indicates the Post data should be added to the in memory database. 
        /// </summary>
        /// <returns></returns>
        public SimulationResponse AddToImdb()
        {
            return AddToImdb(true);
        }

        /// <summary>
        /// Indicates whether the data should be added to the in memory database. 
        /// </summary>
        /// <param name="add"></param>
        /// <returns></returns>
        public SimulationResponse AddToImdb(bool add)
        {
            if (add == true)
            {
                if (Simulation.Condition.IsImdb == false)
                {
                    throw new System.InvalidOperationException(@"You can only specify AddToImdb() if the condition includes Imdb(). ie: When.I.Post().To(""/Endpoint"").With.Imdb().Then.Return.StatusCode(Created).And.AddToImdb()");
                }
            }
            AddImdb = add;
            return this;
        }

        /// <summary>
        /// The entry will be removed from the database if there is a match. Only makes sense if DELETE is called and Exists() is specified. 
        /// </summary>
        public SimulationResponse RemoveFromImdb()
        {
            return RemoveFromImdb(true);
        }

        /// <summary>
        /// Indicates whether the data should be removed from the in memory database. 
        /// </summary>
        /// <param name="remove"></param>
        /// <returns></returns>
        public SimulationResponse RemoveFromImdb(bool remove)
        {
            if (remove == true)
            {
                if (Simulation.Condition.IsImdb == false)
                {
                    throw new System.InvalidOperationException(@"You can only specify RemoveFromImdb() if the condition includes Imdb(). ie: When.I.Delete().FromImdb(""/Endpoint"").With.Imdb().Then.Return.StatusCode(Created).And.RemoveFromImdb()");
                }
            }
            RemoveImdb = remove;
            return this;
        }

        /// <summary>
        /// Contains the text that will be returned in the response body for this simulation. 
        /// </summary>
        /// <remarks>We need much better control of this - currently, it is just serialized as a string. We need extra parameters such as returning raw
        /// bytes; Unicode-encoded; and so forth. 
        /// </remarks>
        [JsonProperty(PropertyName = "content")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Content { get; set; }

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
        /// Return ourselves; means we can write more fluent conditions. 
        /// </summary>
        [JsonIgnore]
        public SimulationResponse With
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Return ourselves; means we can write more fluent conditions. 
        /// </summary>
        [JsonIgnore]
        public SimulationResponse And
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Return ourselves; means we can write more fluent conditions. 
        /// </summary>
        [JsonIgnore]
        public SimulationResponse Then
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Return ourselves; means we can write more fluent conditions. 
        /// </summary>
        [JsonIgnore]
        public SimulationResponse Return
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Return ourselves. 
        /// </summary>
        [JsonIgnore]
        public SimulationResponse Use
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// The simulation that this response belongs to. 
        /// </summary>
        [JsonIgnore]
        public Simulation Simulation
        {
            get
            {
                return SimulationStorage;
            }
            set
            {
                if (SimulationStorage != null) { throw new System.InvalidOperationException("The Simulation has already been set on this Response. A Simulation can only be set once. "); }

                SimulationStorage = value;
            }
        }
        private Simulation SimulationStorage;
    }
}
