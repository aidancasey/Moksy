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
    /// Represents the condition required for a simulation to take place. 
    /// </summary>
    public class SimulationCondition
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public SimulationCondition()
        {
            Repeat = Int64.MaxValue;
        }


        /// <summary>
        /// Adds a single conditional parameter to the request. The parameter must exist (default: in the body, as a name=value pair. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SimulationCondition Parameter(string name, string value)
        {
            Parameter p = new Parameter(name, value);
            ParametersStorage.Add(p);
            return this;
        }

        /// <summary>
        /// Sets up a partial match / content rule. 
        /// </summary>
        /// <param name="content">The content to match. Can be null. </param>
        /// <returns></returns>
        public SimulationCondition Contains(string content)
        {
            ContentRule rule = new ContentRule(content);
            ContainsStorage.Add(rule);
            return this;
        }

        /// <summary>
        /// Sets up a partial match / content rule. 
        /// </summary>
        /// <param name="content">The content to match. Can be null. </param>
        /// <param name="caseSensitive">If true, performs a case sensitive match. </param>
        /// <returns></returns>
        public SimulationCondition Contains(string content, bool caseSensitive)
        {
            ContentRule rule = new ContentRule(content, caseSensitive);
            ContainsStorage.Add(rule);
            return this;
        }

        /// <summary>
        /// Adds a single header to the request. This becomes part of the condition: the header must be present in the Request for the Simulation to be performed. 
        /// </summary>
        /// <param name="name">Must be none-null. For example: Content-Type</param>
        /// <param name="value">Can be empty or null (will be placed as empty in the request). </param>
        /// <returns></returns>
        public SimulationCondition Header(string name, string value)
        {
            Header header = new Header(name, value);
            RequestHeadersStorage.Add(header);
            return this;
        }

        /// <summary>
        /// Adds a single header to the request. This becomes part of the condition: the header must be present in the Request for the Simulation to be performed. 
        /// </summary>
        /// <param name="name">Must be none-null. For example: Content-Type</param>
        /// <returns></returns>
        public SimulationCondition Header(string name)
        {
            Header header = new Header(name);
            RequestHeadersStorage.Add(header);
            return this;
        }

        /// <summary>
        /// Adds a single header to the request. This becomes part of the condition: the header must be present in the Request for the Simulation to be performed. 
        /// </summary>
        /// <param name="name">Must be none-null. For example: Content-Type</param>
        /// <param name="persistence">Indicates whether the header must be present or not for the condition to evaluate. </param>
        /// <returns></returns>
        public SimulationCondition Header(string name, Moksy.Common.Persistence persistence)
        {
            Header header = new Header(name, persistence);
            RequestHeadersStorage.Add(header);
            return this;
        }

        /// <summary>
        /// Adds a single header to the request. This becomes part of the condition: the header must be present in the Request for the Simulation to be performed. 
        /// </summary>
        /// <param name="name">Must be none-null. For example: Content-Type</param>
        /// <param name="value">Can be empty or null (will be placed as empty in the request). </param>
        /// <param name="persistence">Indicates whether the header must be present or not for the condition to evaluate. </param>
        /// <returns></returns>
        public SimulationCondition Header(string name, string value, Moksy.Common.Persistence persistence)
        {
            Header header = new Header(name, value, persistence);
            RequestHeadersStorage.Add(header);
            return this;
        }

        /// <summary>
        /// Adds a single header to the request. This becomes part of the condition: the header must be present in the Request for the Simulation to be performed. 
        /// </summary>
        /// <param name="header">The header (name/value pair) that must be present in the request for the response to occur. </param>
        /// <returns></returns>
        public SimulationCondition Header(Header header)
        {
            if (null == header) return this;
            RequestHeadersStorage.Add(header);
            return this;
        }

        /// <summary>
        /// Adds a number of headers to the request. This becomes part of the condition: the headers must be present in the Request for the Simulation to be performed. 
        /// </summary>
        /// <param name="headers">The headers (name/value pairs) that must be present in the request for the response to occur. </param>
        /// <returns></returns>
        public SimulationCondition Headers(IEnumerable<Header> headers)
        {
            if (null == headers) return this;
            RequestHeadersStorage.AddRange(headers);
            return this;
        }

        /// <summary>
        /// This Simulation is for a Get operation. 
        /// </summary>
        /// <param name="path">The path. This can include placeholder such as /TheEndpoint({id})</param>
        /// <returns></returns>
        public SimulationCondition Get(string path)
        {
            Pattern = path;
            HttpMethod = System.Net.Http.HttpMethod.Get;
            return this;
        }

        /// <summary>
        /// This Simulation is for a Delete operation. 
        /// </summary>
        /// <param name="path">The path. This can include placeholder such as /TheEndpoint({id})</param>
        /// <returns></returns>
        public SimulationCondition Delete(string path)
        {
            Pattern = path;
            HttpMethod = System.Net.Http.HttpMethod.Delete;
            return this;
        }

        /// <summary>
        /// This Simulation is for a Post operation. 
        /// </summary>
        /// <param name="path">The path. ie: /TheEndpoint</param>
        /// <returns></returns>
        public SimulationCondition Post(string path)
        {
            Pattern = path;
            HttpMethod = System.Net.Http.HttpMethod.Post;
            return this;
        }

        /// <summary>
        /// This Simulation is for a Put operation. 
        /// </summary>
        /// <param name="path">The path. This can include placeholder such as /TheEndpoint({id})</param>
        /// <returns></returns>
        public SimulationCondition Put(string path)
        {
            Pattern = path;
            HttpMethod = System.Net.Http.HttpMethod.Put;
            return this;
        }

        /// <summary>
        /// This Simulation is for a Trace operation. 
        /// </summary>
        /// <param name="path">The path. ie: /TheEndpoint</param>
        /// <returns></returns>
        public SimulationCondition Trace(string path)
        {
            Pattern = path;
            HttpMethod = System.Net.Http.HttpMethod.Trace;
            return this;
        }

        /// <summary>
        /// This Simulation is for a Options operation. 
        /// </summary>
        /// <param name="path">The path. ie: /TheEndpoint</param>
        /// <returns></returns>
        public SimulationCondition Options(string path)
        {
            Pattern = path;
            HttpMethod = System.Net.Http.HttpMethod.Options;
            return this;
        }

        /// <summary>
        /// This Simulation is for a Head operation. 
        /// </summary>
        /// <param name="path">The path. ie: /TheEndpoint</param>
        /// <returns></returns>
        public SimulationCondition Head(string path)
        {
            Pattern = path;
            HttpMethod = System.Net.Http.HttpMethod.Head;
            return this;
        }

        /// <summary>
        /// If not null, then anything posted to the Imdb will be grouped by the value of the header provided. The header 
        /// must be part of the submission 
        /// </summary>
        [JsonProperty(PropertyName = "imdbDiscriminator")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ImdbHeaderDiscriminator { get; set; }

        /// <summary>
        /// Returns true if this condition is grouped by a header value. If true, then ImdbHeaderGroup will contain the header that 
        /// groups any posted data. This allows you to logically group your Imdbs based on a discriminator (ie: the value of the given
        /// header). 
        /// </summary>
        [JsonIgnore]
        public bool IsGroupedByImdbHeaderDiscriminator
        {
            get
            {
                if (!IsImdb) return false;

                return ImdbHeaderDiscriminator != null && ImdbHeaderDiscriminator != "";
            }
        }

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

        /// <summary>
        /// A collection of headers that must match on the Request. 
        /// </summary>
        [JsonIgnore]
        public List<Header> RequestHeaders
        {
            get
            {
                var result = new List<Header>(RequestHeadersStorage);
                return result;
            }
            set
            {
                if (value == null)
                {
                    RequestHeadersStorage = new List<Header>();
                    return;
                }

                RequestHeadersStorage.Clear();
                RequestHeadersStorage.AddRange(value);
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

        [JsonIgnore]
        public List<Parameter> Parameters
        {
            get
            {
                var result = new List<Parameter>(ParametersStorage);
                return result;
            }
            set
            {
                if (value == null)
                {
                    ParametersStorage = new List<Parameter>();
                    return;
                }

                ParametersStorage.Clear();
                ParametersStorage.AddRange(value);
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



        [JsonIgnore]
        public List<ContentRule> ContentRules
        {
            get
            {
                var result = new List<ContentRule>(ContainsStorage);
                return result;
            }
            set
            {
                if (value == null)
                {
                    ContainsStorage = new List<ContentRule>();
                    return;
                }

                ContainsStorage.Clear();
                ContainsStorage.AddRange(value);
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



        /// <summary>
        /// Add an assertion to the collection. 
        /// </summary>
        /// <param name="b">The constraint. </param>
        /// <returns></returns>
        public SimulationCondition Constraint(ConstraintBase b)
        {
            if (null == b) return this;
            ConstraintStorage.Add(b);
            return this;
        }

        /// <summary>
        /// Add zero or more assertions to the collection. 
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        public SimulationCondition Constraint(IEnumerable<ConstraintBase> bs)
        {
            if (null == bs) return this;
            foreach (var b in bs)
            {
                if (null == b) continue;
                ConstraintStorage.Add(b);
            }
            return this;
        }



        /// <summary>
        /// Indicates that this rule will be matched only if there are one or more violations in the Constraint evaluations. This can be used to return a collection of error
        /// messages from the service. 
        /// </summary>
        /// <returns></returns>
        public SimulationCondition HasConstraintViolations()
        {
            HasAnyConstraintViolations = true;
            return this;
        }
        


        /// <summary>
        /// Any constraints to be applied to this condition. 
        /// </summary>
        [JsonIgnore]
        public List<ConstraintBase> Constraints
        {
            get
            {
                var result = new List<ConstraintBase>(ConstraintStorage);
                return result;
            }
            set
            {
                if (value == null)
                {
                    ConstraintStorage = new List<ConstraintBase>();
                    return;
                }

                ConstraintStorage.Clear();
                ConstraintStorage.AddRange(value);
            }
        }

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

        [JsonProperty(PropertyName = "hasConstraintViolations")]
        public bool HasAnyConstraintViolations { get; set; }

        /// <summary>
        /// Indicates that the object referenced using a {placeholder} in the Path exists as part of the condition. 
        /// </summary>
        /// <returns></returns>
        public SimulationCondition Exists()
        {
            if (ContentKind != Common.ContentKind.Json) { throw new System.InvalidOperationException("ERROR: Exists can only be used with Json. Use .AsJson(). before the .Exists call. "); }
            
            Substitution s = new Substitution();
            var vars = s.GetVariables(Pattern);
            if (vars.Count() != 1) { throw new System.InvalidOperationException(@"ERROR: To use Exists(), the Path parameter must contain at least one placeholder. ie: When.I.Get().From(""/Endpoint({id})"")"); }

            IndexProperty = string.Format("{0}", vars.First().Name);

            Persistence = Common.Persistence.Exists;
            return this;
        }

        /// <summary>
        /// Indicates that the object referenced using a {placeholder} in the Path does not exist as part of the condition. 
        /// </summary>
        /// <returns></returns>
        public SimulationCondition NotExists()
        {
            if (ContentKind != Common.ContentKind.Json) { throw new System.InvalidOperationException("ERROR: NotExists can only be used with Json. Use .AsJson(). before the .NotExists call. "); }

            Substitution s = new Substitution();
            var vars = s.GetVariables(Pattern);
            if (vars.Count() != 1) { throw new System.InvalidOperationException(@"ERROR: To use Exists(), the Path parameter must contain at least one placeholder. ie: When.I.Get().From(""/Endpoint({id})"")"); }

            IndexProperty = string.Format("{0}", vars.First().Name);

            Persistence = Common.Persistence.NotExists;
            return this;
        }

        /// <summary>
        /// Indicates the content will be sent as Raw text - nothing else. This means that Imdb and so forth cannot be used; you must use .AsJson() to use Imdb. 
        /// </summary>
        /// <returns></returns>
        public SimulationCondition AsText()
        {
            if (IsImdb) { throw new System.InvalidOperationException("ERROR: The operation is associated with an Imdb. AsJson() is inferred. You cannot use AsText(). "); }

            ContentKind = Common.ContentKind.Text;
            return this;
        }

        /// <summary>
        /// Indicates the content will be sent as Json. It must be valid Json that is submitted. 
        /// </summary>
        /// <returns></returns>
        public SimulationCondition AsJson()
        {
            ContentKind = Common.ContentKind.Json;
            return this;
        }

        /// <summary>
        /// Indicates which property is to used for the uniqueness constraint. If null, removes the constraint. 
        /// NOTE: This implies Exists()
        /// </summary>
        /// <param name="propertyName">The property name to be used as an index. If an object exists with this property, the condition will not match. </param>
        /// <returns></returns>
        public SimulationCondition NotExists(string propertyName)
        {
            if (ContentKind != Common.ContentKind.Json) { throw new System.InvalidOperationException("ERROR: NotExists can only be used with Json. Use .AsJson(). before the .NotExists call. "); }

            Persistence = Common.Persistence.NotExists;
            IndexProperty = propertyName;
            return this;
        }

        /// <summary>
        /// Indicates the object by the given property must exist. 
        /// </summary>
        /// <param name="propertyName">The property name to be used as an index. If an object exists with this property, the condition will match. </param>
        /// <returns></returns>
        public SimulationCondition Exists(string propertyName)
        {
            if (ContentKind != Common.ContentKind.Json) { throw new System.InvalidOperationException("ERROR: Exists can only be used with Json. Use .AsJson(). before the .Exists call. "); }

            Persistence = Common.Persistence.Exists;
            IndexProperty = propertyName;
            return this;
        }

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
        /// The condition will evaluate exactly once. The simulation will then be removed from Moksy. 
        /// </summary>
        /// <returns></returns>
        public SimulationCondition Once()
        {
            Repeat = 1;
            return this;
        }

        /// <summary>
        /// The condition will evaluate twice. The simulation will then be removed from Moksy. 
        /// </summary>
        /// <returns></returns>
        public SimulationCondition Twice()
        {
            Repeat = 2;
            return this;
        }

        /// <summary>
        /// The condition will evaluate until it is removed manually. 
        /// </summary>
        /// <returns></returns>
        public SimulationCondition Forever()
        {
            Repeat = Int64.MaxValue;
            return this;
        }

        /// <summary>
        /// The condition will evaluate count Times before it expires and is removed from the service. 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public SimulationCondition Times(long count)
        {
            Repeat = count;
            return this;
        }



        /// <summary>
        /// Return ourselves; means we can write more fluent conditions. 
        /// </summary>
        [JsonIgnore]
        public SimulationCondition With
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
        public SimulationCondition And
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
        public SimulationCondition To
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
        public SimulationCondition From
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
                return this.Simulation.Response;
            }
        }



        /// <summary>
        /// Return the Response associated with this Condition. This is provided so that we can write entire conditions and responses in a single line
        /// with the conditions at the start and the response parameters at the end. 
        /// </summary>
        [JsonIgnore]
        public SimulationResponse Return
        {
            get
            {
                if (null == SimulationStorage) { throw new System.InvalidOperationException("This SimulationCondition is not owned by a Simulation. It must be so that the .Return property can walk back to the Simulation and retrieve the Response. "); }

                return SimulationStorage.Response;
            }
        }



        /// <summary>
        /// The simulation that this condition belongs to. 
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
                if (SimulationStorage != null) { throw new System.InvalidOperationException("The Simulation has already been set on this Condition. A Simulation can only be set once. "); }

                SimulationStorage = value;
            }
        }
        private Simulation SimulationStorage;

        /// <summary>
        /// Storage for the header
        /// </summary>
        private List<Header> RequestHeadersStorage = new List<Header>();

        /// <summary>
        /// Storage for the parameters. 
        /// </summary>
        private List<Parameter> ParametersStorage = new List<Parameter>();

        /// <summary>
        /// Storage for all constraints. 
        /// </summary>
        private List<ConstraintBase> ConstraintStorage = new List<ConstraintBase>();

        /// <summary>
        /// Storage for all content rules
        /// </summary>
        private List<ContentRule> ContainsStorage = new List<ContentRule>();
    }
}
