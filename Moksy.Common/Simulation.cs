using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common
{
    /// <summary>
    /// Represents an individual Simulation and all of its Rules. 
    /// </summary>
    /// <remarks>If RequestHeaders is empty, then no checking is done against the Request. 
    /// </remarks>
    public class Simulation : ICloneable
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public Simulation()
        {
            Name = "Simulation" + System.Guid.NewGuid().ToString();

            Condition = new SimulationCondition();
            Response = new SimulationResponse();
        }

        /// <summary>
        /// Simulation name. 
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The condition to match for this simulation. 
        /// </summary>
        [JsonProperty(PropertyName = "condition")]
        public SimulationCondition Condition
        {
            get
            {
                return ConditionStorage;
            }
            set
            {
                ConditionStorage = value;
                if (ConditionStorage != null)
                {
                    ConditionStorage.Simulation = this;
                }
            }
        }
        private SimulationCondition ConditionStorage;

        /// <summary>
        /// The response to return for this simulation. 
        /// </summary>
        [JsonProperty(PropertyName = "response")]
        public SimulationResponse Response
        {
            get
            {
                return ResponseStorage;
            }
            set
            {
                ResponseStorage = value;
                if (ResponseStorage != null)
                {
                    ResponseStorage.Simulation = this;
                }
            }
        }
        private SimulationResponse ResponseStorage;

        /// <summary>
        /// This Simulation is for a Get operation. 
        /// </summary>
        /// <returns></returns>
        public Simulation Get()
        {
            Condition.SimulationConditionContent.HttpMethod = HttpMethod.Get;
            return this;
        }

        /// <summary>
        /// This Simulation is for a Delete operation. 
        /// </summary>
        /// <returns></returns>
        public Simulation Delete()
        {
            Condition.SimulationConditionContent.HttpMethod = HttpMethod.Delete;
            return this;
        }

        /// <summary>
        /// This Simulation is for a Post operation. 
        /// </summary>
        /// <returns></returns>
        public Simulation Post()
        {
            Condition.SimulationConditionContent.HttpMethod = HttpMethod.Post;
            return this;
        }

        /// <summary>
        /// This Simulation is for a Put operation. 
        /// </summary>
        /// <returns></returns>
        public Simulation Put()
        {
            Condition.SimulationConditionContent.HttpMethod = HttpMethod.Put;
            return this;
        }

        /// <summary>
        /// This Simulation is for a Head operation. 
        /// </summary>
        /// <returns></returns>
        public Simulation Head()
        {
            Condition.SimulationConditionContent.HttpMethod = HttpMethod.Head;
            return this;
        }

        /// <summary>
        /// This Simulation is for a Options operation. 
        /// </summary>
        /// <returns></returns>
        public Simulation Options()
        {
            Condition.SimulationConditionContent.HttpMethod = HttpMethod.Options;
            return this;
        }

        /// <summary>
        /// This Simulation is for a Trace operation. 
        /// </summary>
        /// <returns></returns>
        public Simulation Trace()
        {
            Condition.SimulationConditionContent.HttpMethod = HttpMethod.Trace;
            return this;
        }

        /// <summary>
        /// Indicates the target of the operation. 
        /// </summary>
        /// <param name="path">The path (endpoint) to target.</param>
        /// <returns></returns>
        public SimulationCondition To(string path)
        {
            // If no method has been set, we throw an exception - we have to specify To() first. 
            if (Condition.SimulationConditionContent.HttpMethod == null)
            {
                throw new System.InvalidOperationException(@"ERROR: You can only call To(path) after you have specified one of the HttpVerbs. ie: When.I.Post().To(""Endpoint"")");
            }
            Condition.SimulationConditionContent.Pattern = path;
            return Condition;
        }

        /// <summary>
        /// Send information to the Imdb for the resource. 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public SimulationCondition ToImdb(string path)
        {
            To(path);
            Condition.SimulationConditionContent.IsImdb = true;
            Condition.SimulationConditionContent.ContentKind = ContentKind.Json;
            return Condition;
        }

        /// <summary>
        /// Send information to the Imdb for the resource. The Imdb will be grouped by the discriminator (the header). 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public SimulationCondition ToImdb(string path, string discriminator)
        {
            To(path);
            Condition.SimulationConditionContent.ImdbHeaderDiscriminator = discriminator;
            Condition.SimulationConditionContent.IsImdb = true;
            Condition.SimulationConditionContent.ContentKind = ContentKind.Json;
            return Condition;
        }


        /// <summary>
        /// Indicates the target of the operation. 
        /// </summary>
        /// <param name="path">The path (endpoint) to target.</param>
        /// <returns></returns>
        public SimulationCondition From(string path)
        {
            // If no method has been set, we throw an exception - we have to specify From() first. 
            if (Condition.SimulationConditionContent.HttpMethod == null)
            {
                throw new System.InvalidOperationException(@"ERROR: You can only call From(path) after you have specified one of the HttpVerbs. ie: When.I.Post().To(""Endpoint"")");
            }
            Condition.SimulationConditionContent.Pattern = path;
            return Condition;
        }

        /// <summary>
        /// Retrieve from Imdb. 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public SimulationCondition FromImdb(string path)
        {
            From(path);
            Condition.SimulationConditionContent.IsImdb = true;
            Condition.SimulationConditionContent.ContentKind = ContentKind.Json;
            return Condition;
        }

        /// <summary>
        /// Retrieve from Imdb. 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public SimulationCondition FromImdb(string path, string discriminator)
        {
            From(path);
            Condition.SimulationConditionContent.ImdbHeaderDiscriminator = discriminator;
            Condition.SimulationConditionContent.IsImdb = true;
            Condition.SimulationConditionContent.ContentKind = ContentKind.Json;
            return Condition;
        }

        /// <summary>
        /// Fluent property to return ourself; makes more readable simulations. 
        /// </summary>
        [JsonIgnore]
        public SimulationCondition With
        {
            get
            {
                return Condition;
            }
        }

        /// <summary>
        /// Fluent property to return ourself; makes more readable simulations. 
        /// </summary>
        [JsonIgnore]
        public SimulationResponse Return
        {
            get
            {
                return Response;
            }
        }

        /// <summary>
        /// Fluent property to return ourself; makes more readable simulations. 
        /// </summary>
        [JsonIgnore]
        public Simulation Use
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Fluent property to return ourself; makes more readable simulations. 
        /// </summary>
        [JsonIgnore]
        public Simulation I
        {
            get
            {
                return this;
            }
        }

        #region Cloneable

        /// <summary>
        /// Clone the result. 
        /// </summary>
        /// <returns></returns>
        public Simulation Clone()
        {
            // TODO: Optimize this. Easiest way to serialize and deserialize but this is NOT efficient!
            Simulation result = null;
            lock (this)
            {
                var json = JsonConvert.SerializeObject(this);
                result = JsonConvert.DeserializeObject<Simulation>(json);
            }
            return result;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
