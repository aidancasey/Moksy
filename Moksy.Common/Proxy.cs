using Moksy.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Moksy
{
    /// <summary>
    /// Represents a proxy around a running Moksy service. 
    /// </summary>
    /// <remarks>Your integration tests will instantiate an instance of Proxy and specify the port of the running Moksy instance. You then call .Add() to 
    /// add your simulations. 
    /// </remarks>
    public class Proxy
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="port">The port of the running Moksy service. </param>
        public Proxy(int port)
        {
            Port = port;
        }

        public readonly int Port;
        public readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects };


        /// <summary>
        /// Start Moksy on the given port. It is safe to call this method even if the service is running. 
        /// If this method fails, troubleshoot by launching Moksy.Host.exe <port> from an elevated command prompt.
        /// </summary>
        public bool Start()
        {
            return Start(5);
        }

        /// <summary>
        /// Start Moksy on the given port. It is safe to call this method even if the service is running. 
        /// If this method fails, troubleshoot by launching Moksy.Host.exe <port> from an elevated command prompt.
        /// </summary>
        public bool Start(uint timeoutInSeconds)
        {
            Proxy proxy = new Proxy(Port);
            try
            {
                var all = GetAll();
                return true;
            }
            catch (System.Net.WebException)
            {
            }

            // Now we need to try and start the host. 
            var folder = System.IO.Path.GetDirectoryName(typeof(Proxy).Assembly.Location);
            var exeName = "Moksy.Host.Exe";

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
            psi.Arguments = Port.ToString();
            psi.WorkingDirectory = folder;
            psi.FileName = System.IO.Path.Combine(folder, exeName);

            var process = System.Diagnostics.Process.Start(psi);
            try
            {
                for (uint i = 0; i < timeoutInSeconds; i++)
                {
                    try
                    {
                        var all = GetAll();
                        return true;
                    }
                    catch (System.Net.WebException ex)
                    {
                    }

                    System.Threading.Thread.Sleep(1000);
                    if (process.HasExited) break;
                }

                process.WaitForInputIdle();
            }
            catch
            {
            }
            if (process.HasExited && process.ExitCode == 123)
            {
                string s = "Moksy launches a real HTTP Server and opens up a HTTP Endpoint so that your";
                s += "tests and other services can hit it. ";
                s += "\r\n";
                s += ("By default, only applications launched as Administrator are allowed to");
                s += ("launch the HTTP Server and create the end-point. ");
                s += "\r\n";
                s += ("You are not running as Administrator. ");
                s += "\r\n";
                s += ("There are two ways to proceed:");
                s += "\r\n";
                s += ("1. Launch Moksy.Host.Exe as Administrator. ");
                s += "\r\n";
                s += ("2. Or perform the following operation once from an Administrator Command Prompt");
                s += ("   to create the reservation that will allow everyone to launch a HTTP Server");
                s += ("   and open up the end-point: ");
                s += "\r\n";
                s += (string.Format(@"      netsh http add urlacl url=http://+:{0}/ sddl=""D:(A;;GX;;;WD)""", Port));
                s += "\r\n";
                s += ("   Then re-run Moksy.Host.Exe as an ordinary user. ");
                s += "\r\n";
                throw new System.ApplicationException(s);
            }
            if (process.HasExited && process.ExitCode != 0)
            {
                throw new System.ApplicationException(string.Format("ERROR: Unexpected error launching {0}. ExitCode={1}. Launch '{0} {2}' from a command prompt for more information. ", psi.FileName,process.ExitCode,Port.ToString()));
            }
            return true;
        }



        /// <summary>
        /// Wait up to timeoutInSeconds for the Proxy to become available. 
        /// </summary>
        /// <param name="timeoutInSeconds"></param>
        /// <returns></returns>
        public bool Wait(int timeoutInSeconds)
        {
            try
            {
                for (uint i = 0; i < timeoutInSeconds; i++)
                {
                    try
                    {
                        var all = GetAll();
                        return true;
                    }
                    catch (System.Net.WebException ex)
                    {
                    }

                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch
            {
            }

            return false;
        }



        /// <summary>
        /// Fetch all simulations that are currently configured. 
        /// </summary>
        /// <returns></returns>
        public SimulationCollection GetAll()
        {
            RestSharp.IRestClient client = new RestSharp.RestClient(Root);
            RestSharp.IRestRequest request = new RestSharp.RestRequest(GetSimulationResource(), RestSharp.Method.GET);
            var response = client.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new System.Net.WebException(string.Format("Unable to fetch all resources from {0}. StatusCode returned: {1}, Content: {2}", Root, response.StatusCode, response.Content));
            }
            var result = JsonConvert.DeserializeObject<SimulationCollection>(response.Content, JsonSerializerSettings);
            return result;
        }



        /// <summary>
        /// Return a simulation by name; or null if it does not exist (or an error occurs). 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Simulation GetByName(string name)
        {
            string path = string.Format("{0}('{1}')", GetSimulationResource(), name);

            RestSharp.IRestClient client = new RestSharp.RestClient(Root);
            RestSharp.IRestRequest request = new RestSharp.RestRequest(path, RestSharp.Method.GET);
            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.NotFound) return null;

            var result = JsonConvert.DeserializeObject<Simulation>(response.Content, JsonSerializerSettings);
            return result;
        }



        /// <summary>
        /// Delete all simulations currently registered. By default, this will remove all Imdb as well. 
        /// </summary>
        public HttpStatusCode DeleteAll()
        {
            return DeleteByName("*", true);
        }



        /// <summary>
        /// Delete all simulations currently registered. Call this at the start
        /// </summary>
        public HttpStatusCode DeleteAll(bool removeData)
        {
            return DeleteByName("*", removeData);
        }



        /// <summary>
        /// Delete the Simulation by name. This is case sensitive. This will not remove any Imdb that might exist for that endpoint. 
        /// </summary>
        /// <param name="name"></param>
        public HttpStatusCode DeleteByName(string name)
        {
            return DeleteByName(name, false);
        }



        /// <summary>
        /// Delete the Simulation by name. This is case sensitive. 
        /// </summary>
        /// <param name="name"></param>
        public HttpStatusCode DeleteByName(string name, bool removeImdb)
        {
            string path = string.Format("{0}('{1}')", GetSimulationResource(), name);

            RestSharp.IRestClient client = new RestSharp.RestClient(Root);
            RestSharp.IRestRequest request = new RestSharp.RestRequest(path, RestSharp.Method.DELETE);
            if (removeImdb)
            {
                request.AddParameter("moksy-purgedata", "true", RestSharp.ParameterType.HttpHeader);
            }
            var response = client.Execute(request);
            return response.StatusCode;
        }



        /// <summary>
        /// Add the given simulation to the running service. 
        /// </summary>
        /// <param name="simulation">The simulation to add. Must not be null. </param>
        /// <returns>Created - the entry was added. Any other error indicates a failure. </returns>
        public HttpStatusCode Add(Simulation simulation)
        {
            if (null == simulation) throw new System.ArgumentNullException("simulation");

            string path = string.Format("{0}", GetSimulationResource());

            RestSharp.IRestClient client = new RestSharp.RestClient(Root);
            RestSharp.IRestRequest request = new RestSharp.RestRequest(path, RestSharp.Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(simulation, JsonSerializerSettings), RestSharp.ParameterType.RequestBody);
            var response = client.Execute(request);
            return response.StatusCode;
        }

        public HttpStatusCode Add(SimulationCondition condition)
        {
            if (null == condition) throw new System.ArgumentNullException("condition");
            if (null == condition.Simulation) throw new System.ArgumentOutOfRangeException("ERROR: The .Simulation property on condition must reference the owner Simulation. ");
            return Add(condition.Simulation);
        }

        public HttpStatusCode Add(SimulationResponse response)
        {
            if (null == response) throw new System.ArgumentNullException("response");
            if (null == response.Simulation) throw new System.ArgumentOutOfRangeException("ERROR: The .Simulation property on response must reference the owner Simulation. ");
            return Add(response.Simulation);
        }



        /// <summary>
        /// Terminate the running proxy. 
        /// </summary>
        public void Exit()
        {
            try
            {
                var all = GetAll();

                // If we get to here, the proxy is running; we need to bail out. 
                RestSharp.IRestClient client = new RestSharp.RestClient(Root);
                RestSharp.IRestRequest request = new RestSharp.RestRequest(GetSimulationResource(), RestSharp.Method.DELETE);
                request.AddHeader("moksy-exit", "true");
                var response = client.Execute(request);
                // The response will not return; the service will bail out. 
            }
            catch (System.Net.WebException)
            {
                // The service is not running - return. 
                return;
            }
        }



        protected string GetSimulationResource()
        {
            return string.Format("/{0}", "__Simulation");
        }

        protected string Root
        {
            get
            {
                return string.Format("http://localhost:{0}", Port);
            }
        }
    }
}
