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
            SimulationConditionContent = new Common.SimulationConditionContent();
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
            SimulationConditionContent.ParametersStorage.Add(p);
            return this;
        }

        /// <summary>
        /// Adds a single conditional parameter to the request. The parameter must exist (default: in the body, as a name=value pair. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="parameterType"></param>
        /// <returns></returns>
        public SimulationCondition Parameter(string name, string value, ParameterType parameterType)
        {
            Parameter p = new Parameter(name, value) { ParameterType = parameterType };
            SimulationConditionContent.ParametersStorage.Add(p);
            return this;
        }

        /// <summary>
        /// Adds a single conditional parameter to the request. The parameter must exist (default: in the body, as a name=value pair. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SimulationCondition Parameter(string name, string value, ComparisonType comparison)
        {
            if ((comparison & ComparisonType.NotContains) != 0) throw new System.InvalidOperationException("You must use NotExists instead of NotContains for Parameter checking. ");

            Parameter p = new Parameter(name, value, comparison);
            SimulationConditionContent.ParametersStorage.Add(p);
            return this;
        }

        /// <summary>
        /// Adds a single conditional parameter to the request. The parameter must exist (default: in the body, as a name=value pair. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <param name="parameterType"></param>
        /// <returns></returns>
        public SimulationCondition Parameter(string name, string value, ComparisonType comparison, ParameterType parameterType)
        {
            if ((comparison & ComparisonType.NotContains) != 0) throw new System.InvalidOperationException("You must use NotExists instead of NotContains for Parameter checking. ");

            Parameter p = new Parameter(name, value, comparison) { ParameterType = parameterType };
            SimulationConditionContent.ParametersStorage.Add(p);
            return this;
        }

        /// <summary>
        /// Adds a single conditional parameter to the request. The parameter must exist (default: in the body, as a name=value pair. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SimulationCondition Parameter(string name)
        {
            Parameter p = new Parameter(name);
            SimulationConditionContent.ParametersStorage.Add(p);
            return this;
        }

        /// <summary>
        /// Adds a single conditional parameter to the request. The parameter must exist (default: in the body, as a name=value pair. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SimulationCondition Parameter(string name, ParameterType parameterType)
        {
            Parameter p = new Parameter(name, parameterType);
            SimulationConditionContent.ParametersStorage.Add(p);
            return this;
        }

        /// <summary>
        /// Adds a single conditional parameter to the request. The parameter must exist (default: in the body, as a name=value pair. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SimulationCondition Parameter(string name, ComparisonType comparison, ParameterType parameterType)
        {
            Parameter p = new Parameter(name, comparison, parameterType);
            SimulationConditionContent.ParametersStorage.Add(p);
            return this;
        }

        /// <summary>
        /// Adds a single conditional parameter to the request. The parameter must exist (default: in the body, as a name=value pair. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="comparison">Is the value encoded or case sensitive when the name is compared?</param>
        /// <returns></returns>
        public SimulationCondition Parameter(string name, ComparisonType comparison)
        {
            if ((comparison & ComparisonType.NotContains) != 0) throw new System.InvalidOperationException("You must use NotExists instead of NotContains for Parameter checking. ");

            Parameter p = new Parameter(name, comparison);
            SimulationConditionContent.ParametersStorage.Add(p);
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
            SimulationConditionContent.ContainsStorage.Add(rule);
            return this;
        }

        /// <summary>
        /// Sets up a partial match / content rule. 
        /// </summary>
        /// <param name="content">The content to match. Can be null. </param>
        /// <returns></returns>
        public SimulationCondition Contains(string content, ComparisonType comparison)
        {
            if ((comparison & ComparisonType.NotExists) != 0) throw new System.InvalidOperationException("You must use NotContains instead of NotExists for Contains checking. ");

            ContentRule rule = new ContentRule(content, comparison);
            SimulationConditionContent.ContainsStorage.Add(rule);
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
            SimulationConditionContent.ContainsStorage.Add(rule);
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
            SimulationConditionContent.RequestHeadersStorage.Add(header);
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
            SimulationConditionContent.RequestHeadersStorage.Add(header);
            return this;
        }

        /// <summary>
        /// Adds a single header to the request. This becomes part of the condition: the header must be present in the Request for the Simulation to be performed. 
        /// </summary>
        /// <param name="name">Must be none-null. For example: Content-Type</param>
        /// <param name="persistence">Indicates whether the header must be present or not for the condition to evaluate. </param>
        /// <returns></returns>
        [Obsolete("Use the ComparisonType overload instead")]
        public SimulationCondition Header(string name, Moksy.Common.Persistence persistence)
        {
            Header header = new Header(name, persistence);
            SimulationConditionContent.RequestHeadersStorage.Add(header);
            return this;
        }

        /// <summary>
        /// Adds a single header to the request. This becomes part of the condition: the header must be present in the Request for the Simulation to be performed. 
        /// </summary>
        /// <param name="name">Must be none-null. For example: Content-Type</param>
        /// <param name="comparison">How to compare the header. </param>
        /// <returns></returns>
        public SimulationCondition Header(string name, ComparisonType comparison)
        {
            Header header = new Header(name, comparison);
            SimulationConditionContent.RequestHeadersStorage.Add(header);
            return this;
        }

        /// <summary>
        /// Adds a single header to the request. This becomes part of the condition: the header must be present in the Request for the Simulation to be performed. 
        /// </summary>
        /// <param name="name">Must be none-null. For example: Content-Type</param>
        /// <param name="value">Can be empty or null (will be placed as empty in the request). </param>
        /// <param name="persistence">Indicates whether the header must be present or not for the condition to evaluate. </param>
        /// <returns></returns>
        [Obsolete("Use the ComparisonType overload instead")]
        public SimulationCondition Header(string name, string value, Moksy.Common.Persistence persistence)
        {
            Header header = new Header(name, value, persistence);
            SimulationConditionContent.RequestHeadersStorage.Add(header);
            return this;
        }

        /// <summary>
        /// Adds a single header to the request. This becomes part of the condition: the header must be present in the Request for the Simulation to be performed. 
        /// </summary>
        /// <param name="name">Must be none-null. For example: Content-Type</param>
        /// <param name="value">Can be empty or null (will be placed as empty in the request). </param>
        /// <param name="comparison">The comparison type. </param>
        /// <returns></returns>
        public SimulationCondition Header(string name, string value, ComparisonType comparison)
        {
            Header header = new Header(name, value, comparison);
            SimulationConditionContent.RequestHeadersStorage.Add(header);
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
            SimulationConditionContent.RequestHeadersStorage.Add(header);
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
            SimulationConditionContent.RequestHeadersStorage.AddRange(headers);
            return this;
        }

        /// <summary>
        /// This Simulation is for a Get operation. 
        /// </summary>
        /// <param name="path">The path. This can include placeholder such as /TheEndpoint({id})</param>
        /// <returns></returns>
        public SimulationCondition Get(string path)
        {
            SimulationConditionContent.Pattern = path;
            SimulationConditionContent.HttpMethod = System.Net.Http.HttpMethod.Get;
            return this;
        }

        /// <summary>
        /// This Simulation is for a Delete operation. 
        /// </summary>
        /// <param name="path">The path. This can include placeholder such as /TheEndpoint({id})</param>
        /// <returns></returns>
        public SimulationCondition Delete(string path)
        {
            SimulationConditionContent.Pattern = path;
            SimulationConditionContent.HttpMethod = System.Net.Http.HttpMethod.Delete;
            return this;
        }

        /// <summary>
        /// This Simulation is for a Post operation. 
        /// </summary>
        /// <param name="path">The path. ie: /TheEndpoint</param>
        /// <returns></returns>
        public SimulationCondition Post(string path)
        {
            SimulationConditionContent.Pattern = path;
            SimulationConditionContent.HttpMethod = System.Net.Http.HttpMethod.Post;
            return this;
        }

        /// <summary>
        /// This Simulation is for a Put operation. 
        /// </summary>
        /// <param name="path">The path. This can include placeholder such as /TheEndpoint({id})</param>
        /// <returns></returns>
        public SimulationCondition Put(string path)
        {
            SimulationConditionContent.Pattern = path;
            SimulationConditionContent.HttpMethod = System.Net.Http.HttpMethod.Put;
            return this;
        }

        /// <summary>
        /// This Simulation is for a Trace operation. 
        /// </summary>
        /// <param name="path">The path. ie: /TheEndpoint</param>
        /// <returns></returns>
        public SimulationCondition Trace(string path)
        {
            SimulationConditionContent.Pattern = path;
            SimulationConditionContent.HttpMethod = System.Net.Http.HttpMethod.Trace;
            return this;
        }

        /// <summary>
        /// This Simulation is for a Options operation. 
        /// </summary>
        /// <param name="path">The path. ie: /TheEndpoint</param>
        /// <returns></returns>
        public SimulationCondition Options(string path)
        {
            SimulationConditionContent.Pattern = path;
            SimulationConditionContent.HttpMethod = System.Net.Http.HttpMethod.Options;
            return this;
        }

        /// <summary>
        /// This Simulation is for a Head operation. 
        /// </summary>
        /// <param name="path">The path. ie: /TheEndpoint</param>
        /// <returns></returns>
        public SimulationCondition Head(string path)
        {
            SimulationConditionContent.Pattern = path;
            SimulationConditionContent.HttpMethod = System.Net.Http.HttpMethod.Head;
            return this;
        }

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
                if (!SimulationConditionContent.IsImdb) return false;

                return SimulationConditionContent.ImdbHeaderDiscriminator != null && SimulationConditionContent.ImdbHeaderDiscriminator != "";
            }
        }

        /// <summary>
        /// A collection of headers that must match on the Request. 
        /// </summary>
        [JsonIgnore]
        public List<Header> RequestHeaders
        {
            get
            {
                var result = new List<Header>(SimulationConditionContent.RequestHeadersStorage);
                return result;
            }
            private set
            {
                if (value == null)
                {
                    SimulationConditionContent.RequestHeadersStorage = new List<Header>();
                    return;
                }

                SimulationConditionContent.RequestHeadersStorage.Clear();
                SimulationConditionContent.RequestHeadersStorage.AddRange(value);
            }
        }

        [JsonIgnore]
        public List<Parameter> Parameters
        {
            get
            {
                var result = new List<Parameter>(SimulationConditionContent.ParametersStorage);
                return result;
            }
            private set
            {
                if (value == null)
                {
                    SimulationConditionContent.ParametersStorage = new List<Parameter>();
                    return;
                }

                SimulationConditionContent.ParametersStorage.Clear();
                SimulationConditionContent.ParametersStorage.AddRange(value);
            }
        }

        [JsonIgnore]
        public List<ContentRule> ContentRules
        {
            get
            {
                var result = new List<ContentRule>(SimulationConditionContent.ContainsStorage);
                return result;
            }
            private set
            {
                if (value == null)
                {
                    SimulationConditionContent.ContainsStorage = new List<ContentRule>();
                    return;
                }

                SimulationConditionContent.ContainsStorage.Clear();
                SimulationConditionContent.ContainsStorage.AddRange(value);
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
            SimulationConditionContent.ConstraintStorage.Add(b);
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
                SimulationConditionContent.ConstraintStorage.Add(b);
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
            SimulationConditionContent.HasAnyConstraintViolations = true;
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
                var result = new List<ConstraintBase>(SimulationConditionContent.ConstraintStorage);
                return result;
            }
            private set
            {
                if (value == null)
                {
                    SimulationConditionContent.ConstraintStorage = new List<ConstraintBase>();
                    return;
                }

                SimulationConditionContent.ConstraintStorage.Clear();
                SimulationConditionContent.ConstraintStorage.AddRange(value);
            }
        }

        /// <summary>
        /// Indicates that the object referenced using a {placeholder} in the Path exists as part of the condition. 
        /// </summary>
        /// <returns></returns>
        public SimulationCondition Exists()
        {
            if (SimulationConditionContent.ContentKind != Common.ContentKind.Json) { throw new System.InvalidOperationException("ERROR: Exists can only be used with Json. Use .AsJson(). before the .Exists call. "); }
            
            Substitution s = new Substitution();
            var vars = s.GetVariables(SimulationConditionContent.Pattern);
            if (vars.Count() != 1) { throw new System.InvalidOperationException(@"ERROR: To use Exists(), the Path parameter must contain at least one placeholder. ie: When.I.Get().From(""/Endpoint({id})"")"); }

            SimulationConditionContent.IndexProperty = string.Format("{0}", vars.Last().Name);

            SimulationConditionContent.Persistence = Common.Persistence.Exists;
            return this;
        }

        /// <summary>
        /// Indicates that the object referenced using a {placeholder} in the Path does not exist as part of the condition. 
        /// </summary>
        /// <returns></returns>
        public SimulationCondition NotExists()
        {
            if (SimulationConditionContent.ContentKind != Common.ContentKind.Json) { throw new System.InvalidOperationException("ERROR: NotExists can only be used with Json. Use .AsJson(). before the .NotExists call. "); }

            Substitution s = new Substitution();
            var vars = s.GetVariables(SimulationConditionContent.Pattern);
            if (vars.Count() != 1) { throw new System.InvalidOperationException(@"ERROR: To use Exists(), the Path parameter must contain at least one placeholder. ie: When.I.Get().From(""/Endpoint({id})"")"); }

            SimulationConditionContent.IndexProperty = string.Format("{0}", vars.Last().Name);

            SimulationConditionContent.Persistence = Common.Persistence.NotExists;
            return this;
        }

        /// <summary>
        /// Indicates the content will be sent as Raw text - nothing else. This means that Imdb and so forth cannot be used; you must use .AsJson() to use Imdb. 
        /// </summary>
        /// <returns></returns>
        public SimulationCondition AsText()
        {
            if (SimulationConditionContent.IsImdb) { throw new System.InvalidOperationException("ERROR: The operation is associated with an Imdb. AsJson() is inferred. You cannot use AsText(). "); }

            SimulationConditionContent.ContentKind = Common.ContentKind.Text;
            return this;
        }

        /// <summary>
        /// Indicates the content will be sent as Json. It must be valid Json that is submitted. 
        /// </summary>
        /// <returns></returns>
        public SimulationCondition AsJson()
        {
            SimulationConditionContent.ContentKind = Common.ContentKind.Json;
            return this;
        }

        /// <summary>
        /// The content will be posted as Body Parameters. Use this where the parameters are send via a Form Post (for example). 
        /// </summary>
        /// <returns></returns>
        public SimulationCondition AsBodyParameters()
        {
            SimulationConditionContent.ContentKind = Common.ContentKind.BodyParameters;
            return this;
        }

        /// <summary>
        /// The content will be posted as raw binary. Use this where files need to be uploaded and downloaded. 
        /// </summary>
        /// <returns></returns>
        public SimulationCondition AsBinary()
        {
            SimulationConditionContent.ContentKind = Common.ContentKind.File;
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
            if (SimulationConditionContent.ContentKind != Common.ContentKind.Json && SimulationConditionContent.ContentKind != Common.ContentKind.BodyParameters && SimulationConditionContent.ContentKind != Common.ContentKind.File) { throw new System.InvalidOperationException("ERROR: NotExists can only be used with Json or BodyParameters. Use .AsJson() or .AsBodyParameters() or .AsBinary(). before the .NotExists call. "); }

            SimulationConditionContent.Persistence = Common.Persistence.NotExists;
            SimulationConditionContent.IndexProperty = propertyName;
            return this;
        }

        /// <summary>
        /// Indicates the object by the given property must exist. 
        /// </summary>
        /// <param name="propertyName">The property name to be used as an index. If an object exists with this property, the condition will match. </param>
        /// <returns></returns>
        public SimulationCondition Exists(string propertyName)
        {
            if (SimulationConditionContent.ContentKind != Common.ContentKind.Json && SimulationConditionContent.ContentKind != Common.ContentKind.BodyParameters && SimulationConditionContent.ContentKind != Common.ContentKind.File) { throw new System.InvalidOperationException("ERROR: Exists can only be used with Json or BodyParameters or Binary. Use .AsJson() or .AsBodyParameters() or .AsBinary() before the .Exists call. "); }

            SimulationConditionContent.Persistence = Common.Persistence.Exists;
            SimulationConditionContent.IndexProperty = propertyName;
            return this;
        }

        /// <summary>
        /// The condition will evaluate exactly once. The simulation will then be removed from Moksy. 
        /// </summary>
        /// <returns></returns>
        public SimulationCondition Once()
        {
            SimulationConditionContent.Repeat = 1;
            return this;
        }

        /// <summary>
        /// The condition will evaluate twice. The simulation will then be removed from Moksy. 
        /// </summary>
        /// <returns></returns>
        public SimulationCondition Twice()
        {
            SimulationConditionContent.Repeat = 2;
            return this;
        }

        /// <summary>
        /// The condition will evaluate until it is removed manually. 
        /// </summary>
        /// <returns></returns>
        public SimulationCondition Forever()
        {
            SimulationConditionContent.Repeat = Int64.MaxValue;
            return this;
        }

        /// <summary>
        /// The condition will evaluate count Times before it expires and is removed from the service. 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public SimulationCondition Times(long count)
        {
            SimulationConditionContent.Repeat = count;
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
            protected internal set
            {
                if (SimulationStorage != null) { throw new System.InvalidOperationException("The Simulation has already been set on this Condition. A Simulation can only be set once. "); }

                SimulationStorage = value;
            }
        }
        private Simulation SimulationStorage;

        [JsonProperty("content")]
        public SimulationConditionContent SimulationConditionContent { get; set; }
    }
}
