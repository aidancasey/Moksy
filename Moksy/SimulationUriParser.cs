using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy
{
    /// <summary>
    /// Helper methods for parsing the Simulation Uri. 
    /// </summary>
    /// <remarks>TODO: This is a placeholder until I get a proper dispatching mechanism set up. </remarks>
    internal class SimulationUriParser
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="path">The path containing the end-point to parse.</param>
        /// <remarks>The path is typically the AbsolutePath from the request Uri. </remarks>
        public SimulationUriParser(string path)
        {
            Path = path;
        }

        public readonly string Path;

        /// <summary>
        /// Returns the Identity of the path (or null if it does not exist). 
        /// </summary>
        public string Identity
        {
            get
            {
                if (Path == null) return null;

                System.Text.RegularExpressions.Regex ex = new System.Text.RegularExpressions.Regex(string.Format("/{0}[(]'[A-Za-z0-9*]*'[)]", Moksy.Routes.SimulationRoute.SimulationName));
                var match = ex.Match(Path);

                if (match.Success)
                {
                    var configurationName = match.Value;
                    configurationName = configurationName.Substring(string.Format("/{0}", Moksy.Routes.SimulationRoute.SimulationName).Length);
                    configurationName = configurationName.Substring(2, configurationName.Length - 4);

                    return configurationName;
                }

                return null;
            }
        }

        /// <summary>
        /// Returns True if the path has an identity; false otherwise. 
        /// </summary>
        public bool HasIdentity
        {
            get
            {
                var id = Identity;
                if (null == id) return false;

                return true;
            }
        }

        /// <summary>
        /// True if the path is for a simulation. False otherwise. 
        /// </summary>
        public bool IsSimulation
        {
            get
            {
                if (null == Path) return false;

                if (!HasIdentity)
                {
                    if (string.Compare(string.Format("/{0}", Moksy.Routes.SimulationRoute.SimulationName), Path, true) == 0)
                    {
                        return true;
                    }

                    if (Path.StartsWith(string.Format("/{0}(", Moksy.Routes.SimulationRoute.SimulationName), StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
                else
                {
                    if (Path.StartsWith(string.Format("/{0}(", Moksy.Routes.SimulationRoute.SimulationName), StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
