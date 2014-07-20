using Moksy.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing;

namespace Moksy.Routes
{
    /// <summary>
    /// The Simulation Route. 
    /// </summary>
    internal class SimulationRoute : IHttpRoute
    {
        /// <summary>
        /// Name of the end-point to access a Simulation. 
        /// </summary>
        public const string SimulationName = "__Simulation";

        public SimulationRoute()
        {
        }

        public IDictionary<string, object> Constraints
        {
            get
            {
                return new Dictionary<string, object>();
            }
        }

        public IDictionary<string, object> DataTokens
        {
            get
            {
                return new Dictionary<string, object>();
            }
        }

        public IDictionary<string, object> Defaults
        {
            get
            {
                return new Dictionary<string, object>();
            }
        }

        public IHttpRouteData GetRouteData(string virtualPathRoot, System.Net.Http.HttpRequestMessage request)
        {
            if (request == null) return null;

            SimulationUriParser parser = new SimulationUriParser(request.RequestUri.AbsolutePath);
            if (!parser.IsSimulation) return null;

            HttpRouteData d = new HttpRouteData(this);
            return d;
        }

        public IHttpVirtualPathData GetVirtualPath(System.Net.Http.HttpRequestMessage request, IDictionary<string, object> values)
        {
            return null;
        }

        public System.Net.Http.HttpMessageHandler Handler
        {
            get
            {
                SimulationMessageHandler handler = new SimulationMessageHandler();
                return handler;
            }
        }

        public string RouteTemplate
        {
            get { return null; }
        }
    }
}
