using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common
{
    /// <summary>
    /// Represents a variable. 
    /// </summary>
    public class Variable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Variable()
        {
        }

        /// <summary>
        /// Creates a variable with the given name. 
        /// </summary>
        /// <param name="name">Name of the variable. Cannot be Null. Will be treated as a Guid. </param>
        public Variable(string name)
        {
            if (name == null) throw new System.ArgumentNullException("name");

            Name = name;
            Kind = VariableKind.Guid;
        }

        /// <summary>
        /// Creates a variable with the given name. 
        /// </summary>
        /// <param name="name">Name of the variable. Cannot be Null. Will be treated as a Guid. </param>
        public Variable(string name, string value)
        {
            if (name == null) throw new System.ArgumentNullException("name");

            Name = name;
            Value = value;
            Kind = VariableKind.Constant;
        }

        /// <summary>
        /// Variable Name. 
        /// </summary>
        [JsonProperty(PropertyName="name")]
        public  string Name {get;set;}

        /// <summary>
        /// Will contain the value (but only if Kind is Constant). 
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public string Value {get;set;}

        /// <summary>
        /// The kind of variable this is. If a Guid, the value is implicitl calculated on every response. 
        /// </summary>
        [JsonProperty(PropertyName = "kind")]
        public VariableKind Kind { get; set; }
    }
}
