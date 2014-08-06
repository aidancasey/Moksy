using Moksy.Common.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common
{
    /// <summary>
    /// Describes a Match. Returned by the SimulationManager as a result of a match. 
    /// </summary>
    public class Match
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public Match()
        {
            EvaluatedMatchingConstraints = new List<ConstraintBase>();
            EvaluatedNoneMatchingConstraints = new List<ConstraintBase>();
        }

        /// <summary>
        /// The simulation that was matched. Null otherwise. 
        /// </summary>
        public Simulation Simulation { get; set; }

        /// <summary>
        /// Any constraints that evaluted to true for this object. 
        /// </summary>
        public readonly List<ConstraintBase> EvaluatedMatchingConstraints;

        /// <summary>
        /// Any constraints that evaluated to false for this object. 
        /// </summary>
        public readonly List<ConstraintBase> EvaluatedNoneMatchingConstraints;
    }
}
