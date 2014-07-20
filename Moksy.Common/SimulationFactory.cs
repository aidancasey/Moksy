using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common
{
    /// <summary>
    /// Factory for manufacturing simulations. 
    /// </summary>
    public class SimulationFactory
    {
        /// <summary>
        /// Create a new simulation. You will rarely use this method; instead, use one or more of the OnVerb calls. 
        /// </summary>
        /// <returns></returns>
        public static Simulation New()
        {
            var result = new Simulation();
            return result;
        }

        /// <summary>
        /// Creates a new simulation. Is the same as New().
        /// </summary>
        public static Simulation When
        {
            get
            {
                return New();
            }
        }

        /// <summary>
        /// Create a new simulation. You will rarely use this method; instead, use one or more of the OnVerb calls. 
        /// </summary>
        /// <returns></returns>
        public static Simulation New(string name)
        {
            var result = new Simulation() { Name = name };
            return result;
        }
    }
}
