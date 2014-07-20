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
    /// This is a generic Route that will ALWAYS match any pattern that is NOT a __Simulation. This is used to dispatch the messge to our GenericHandler
    /// which populates the canned responses based on the currently available Simulations. 
    /// </summary>
    internal class GenericRoute : IHttpRoute
    {
        public GenericRoute()
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
            if (parser.IsSimulation) return null;

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
                GenericMessageHandler handler = new GenericMessageHandler();
                return handler;
            }
        }

        public string RouteTemplate
        {
            get { return null; }
        }
    }
}
