using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
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
            SimulationResponseContent = new Common.SimulationResponseContent();
        }

        /// <summary>
        /// Specifies the text that will be returned in the body for this simulation. 
        /// </summary>
        /// <param name="content">The content. Can be null or empty string. The default is {value} which is the sensible default and will contain the value from 
        /// the Imdb if using ToImdb or FromImdb. </param>
        /// <returns></returns>
        /// <remarks>Content can contain free text, the {value} placeholder (which is the value of the object that was added to the Imdb) or a {variable}. For example:
        /// To return the object that was added to the database, use Body("{value}"). 
        /// To return a dynamically created GUID associated with an object added to the Imdb, do this:
        /// ...Then.Return.With.Variable("identity").OverrideProperty("Id", "{identity}").Body("{identity}");
        /// [That would also asign an Id property to the value of identity]
        /// </remarks>
        public SimulationResponse Body(string content)
        {
            SimulationResponseContent.Content = content;
            return this;
        }

        /// <summary>
        /// Specifies the text that will be returned in the body for this simulation. 
        /// </summary>
        /// <param name="content">The byte content to use. </param>
        /// </remarks>
        public SimulationResponse Body(byte[] content)
        {
            SimulationResponseContent.ContentKind = Common.ContentKind.File;
            SimulationResponseContent.ContentAsBytes = content;
            return this;
        }

        /// <summary>
        /// Specifies the object that will be returned in the body for this simulation. Must be serializable as Json. 
        /// </summary>
        /// <param name="content">The object to be serialized as Json. </param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        public SimulationResponse Body(object o)
        {
            if (null == o)
            {
                SimulationResponseContent.Content = null;
            }
            else
            {
                var json = JsonConvert.SerializeObject(o);
                SimulationResponseContent.Content = json;
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
            SimulationResponseContent.ResponseHeadersStorage.Add(header);
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
            SimulationResponseContent.ResponseHeadersStorage.Add(header);
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
            SimulationResponseContent.ResponseHeadersStorage.AddRange(headers);
            return this;
        }

        /// <summary>
        /// Specifies the status code that will be returned for this simulation. 
        /// </summary>
        /// <param name="statusCode">The status code to return. </param>
        /// <returns></returns>
        public SimulationResponse StatusCode(System.Net.HttpStatusCode statusCode)
        {
            SimulationResponseContent.HttpStatusCode = statusCode;
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
                var result = new List<Header>(SimulationResponseContent.ResponseHeadersStorage);
                return result;
            }
            set
            {
                if (value == null)
                {
                    SimulationResponseContent.ResponseHeadersStorage = new List<Header>();
                    return;
                }

                SimulationResponseContent.ResponseHeadersStorage.Clear();
                SimulationResponseContent.ResponseHeadersStorage.AddRange(value);
            }
        }


        [JsonIgnore]
        public List<Variable> Variables
        {
            get
            {
                var result = new List<Variable>(SimulationResponseContent.VariablesStorage);
                return result;
            }
            set
            {
                if (value == null)
                {
                    SimulationResponseContent.VariablesStorage = new List<Variable>();
                    return;
                }

                SimulationResponseContent.VariablesStorage.Clear();
                SimulationResponseContent.VariablesStorage.AddRange(value);
            }
        }




        /// <summary>
        /// Set up a new variable as part of the response. This variable can be used as a placeholder in any content that is returned or stored in the Imdb. 
        /// </summary>
        /// <param name="name">Name of the variable. If null, will return safely. </param>
        /// <returns></returns>
        public SimulationResponse Variable(string name)
        {
            if (null == name) return this;
            Variable v = new Variable(name);
            SimulationResponseContent.VariablesStorage.Add(v);
            return this;
        }

        /// <summary>
        /// Add a variable. The variable will be created as a Constant value. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SimulationResponse Variable(string name, string value)
        {
            if (null == name) return this;
            Variable v = new Variable(name, value);
            SimulationResponseContent.VariablesStorage.Add(v);
            return this;
        }



        /// <summary>
        /// For a POST/PUT, indicates that property name will be set to value BEFORE the object is committed to the Imdb. 
        /// </summary>
        /// <param name="name">Property name. Cannot be Null. </param>
        /// <param name="value">Property value. Can be Null. </param>
        /// <remarks>Properties relate to changes that will be made to an object BEFORE it is added to the Imdb. For example: objects often have internal identifiers / Guids so
        /// we would call .OverrideProperty("Id"). in the Response. This will create a new Guid (default property type is Guid) and push it into the Json as an Id property. 
        /// </remarks>
        /// <returns></returns>
        public SimulationResponse OverrideProperty(string name, string value)
        {
            if (null == name) return this;
            Property p = new Property(name, value);
            SimulationResponseContent.PropertiesStorage.Add(p);
            return this;
        }



        [JsonIgnore]
        public List<Property> Properties
        {
            get
            {
                var result = new List<Property>(SimulationResponseContent.PropertiesStorage);
                return result;
            }
            private set
            {
                if (value == null)
                {
                    SimulationResponseContent.PropertiesStorage = new List<Property>();
                    return;
                }

                SimulationResponseContent.PropertiesStorage.Clear();
                SimulationResponseContent.PropertiesStorage.AddRange(value);
            }
        }


        /// <summary>
        /// Calculate the values for the variables. Call this even if the variables are constant; this will ensure that any placeholders are expanded. 
        /// </summary>
        /// <returns>A (possibly empty) collection of values to be used as the variables. </returns>
        public Dictionary<string, string> CalculateVariables()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var v in SimulationResponseContent.VariablesStorage)
            {
                if (v.Kind == VariableKind.Guid)
                {
                    result[v.Name] = System.Guid.NewGuid().ToString();
                }
                if (v.Kind == VariableKind.Constant)
                {
                    result[v.Name] = v.Value;
                }
            }
            return result;
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
                if (Simulation.Condition.SimulationConditionContent.IsImdb == false)
                {
                    throw new System.InvalidOperationException(@"You can only specify AddToImdb() if the condition includes Imdb(). ie: When.I.Post().ToImdb(""/Endpoint"").Then.Return.StatusCode(Created).And.AddToImdb()");
                }
                if (Simulation.Condition.SimulationConditionContent.HttpMethod != HttpMethod.Post && Simulation.Condition.SimulationConditionContent.HttpMethod != HttpMethod.Put)
                {
                    throw new System.InvalidOperationException(@"You can only specify AddToImdb() for Post() and Put() Conditions. ");
                }
            }
            SimulationResponseContent.AddImdb = add;
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
                if (Simulation.Condition.SimulationConditionContent.IsImdb == false)
                {
                    throw new System.InvalidOperationException(@"You can only specify RemoveFromImdb() if the condition includes Imdb(). ie: When.I.Delete().FromImdb(""/Endpoint"").With.Imdb().Then.Return.StatusCode(Created).And.RemoveFromImdb()");
                }
            }
            SimulationResponseContent.RemoveImdb = remove;
            return this;
        }

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
            protected internal set
            {
                if (SimulationStorage != null) { throw new System.InvalidOperationException("The Simulation has already been set on this Response. A Simulation can only be set once. "); }

                SimulationStorage = value;
            }
        }
        private Simulation SimulationStorage;



        [JsonProperty(PropertyName="content")]
        public SimulationResponseContent SimulationResponseContent {get;set;}
    }

   
}
