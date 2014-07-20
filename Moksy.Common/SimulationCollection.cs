using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common
{
    /// <summary>
    /// A collection of Simulations. 
    /// </summary>
    public class SimulationCollection : List<Simulation>, ICloneable
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public SimulationCollection()
        {
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        public SimulationCollection(IEnumerable<Simulation> simulators) : base(simulators)
        {
        }

        /// <summary>
        /// SyncRoot. 
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Clone the entire collection. 
        /// </summary>
        /// <returns></returns>
        public SimulationCollection Clone()
        {
            lock (SyncRoot)
            {
                var result = new SimulationCollection();
                foreach (var s in this)
                {
                    result.Add(s.Clone());
                }
                return result;
            }
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
