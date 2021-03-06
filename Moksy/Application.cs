﻿using Moksy.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.SelfHost;

namespace Moksy
{
    /// <summary>
    /// The Moksy application. Call Start to launch the service on the given port. 
    /// </summary>
    public class Application
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="port"></param>
        public Application(int port)
        {
            Port = port;
            Parameters = new ApplicationDirectives();
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="parameters"></param>
        public Application(int port, ApplicationDirectives parameters)
        {
            Port = port;
            if (parameters == null) parameters = new ApplicationDirectives();

            Parameters = parameters;
        }

        /// <summary>
        /// Port number the application is bound to. 
        /// </summary>
        public readonly int Port;

        /// <summary>
        /// Parameters. 
        /// </summary>
        public readonly ApplicationDirectives Parameters;

        /// <summary>
        /// Start Moksy. 
        /// </summary>
        public void Start()
        {
            Stop();

            Configuration = new HttpSelfHostConfiguration(string.Format("http://localhost:{0}", Port));
            Server = new HttpSelfHostServer(Configuration);
            Configuration.Routes.Add(Moksy.Routes.SimulationRoute.SimulationName, new SimulationRoute());
            Configuration.Routes.Add("/EverythingElse", new GenericRoute(Parameters));

            Server.OpenAsync().Wait();
        }

        /// <summary>
        /// Stop Moksy. 
        /// </summary>
        public void Stop()
        {
            if (null != Configuration)
            {
                Configuration = null;
            }
            if (null != Server)
            {
                Server.CloseAsync().Wait();
                Server = null;
            }
        }

        private HttpSelfHostConfiguration Configuration;
        private HttpSelfHostServer Server;
    }
}
