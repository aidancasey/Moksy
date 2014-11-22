using Moksy.Common;
using Moksy.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Handlers
{
    /// <summary>
    /// Handler all hits on the __Simulation resource. 
    /// </summary>
    /// <remarks>The __Simulation resource is a private resource used by Moksy to receive and manage Simulations. Simulations are typically managed by clients using 
    /// the Proxy class in the Moksy.Common library.
    /// To see all Simulations in your browser, navigate to http://localhost:10011/__Simulation once the Moksy.Host is running. 
    /// </remarks>
    internal class SimulationMessageHandler : HttpMessageHandler
    {
        public SimulationMessageHandler()
        {
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            SimulationUriParser parser = new SimulationUriParser(request.RequestUri.AbsolutePath); 
            
            if (request.Method == HttpMethod.Get)
            {
                if (!parser.HasIdentity)
                {
                    return Get(request, cancellationToken);
                }

                return GetByName(parser.Identity, request, cancellationToken);
            }
            else if (request.Method == HttpMethod.Post)
            {
                ByteArrayContent content = request.Content as ByteArrayContent;
                if (content != null)
                {
                    var task = content.ReadAsByteArrayAsync();
                    task.Wait();
                    
                    var contentAsString = new System.Text.ASCIIEncoding().GetString(task.Result);
                    var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects };
                    var s = JsonConvert.DeserializeObject<Simulation>(contentAsString, settings);
                    return Create(s, request, cancellationToken);
                }
            }
            else if (request.Method == HttpMethod.Delete)
            {
                if (request.Headers.Contains("moksy-exit"))
                {
                    System.Environment.Exit(0);
                }
                return DeleteByName(parser.Identity, request, cancellationToken);   
            }

            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            message.Content = new StringContent(string.Format("Enter the end-point as /{0} to view the Mosky endpoints. ", Routes.SimulationRoute.SimulationName));
            var response = Task<HttpResponseMessage>.Factory.StartNew( () => {return message;});
            return response;
        }

        /// <summary>
        /// Get all of the simulations that are currently configured. Just navigate to http://localhost:10011/__Simulation to see everything that is available. 
        /// </summary>
        /// <param name="request">The request. </param>
        /// <param name="cancellationToken">Cancellation token. </param>
        /// <returns></returns>
        protected Task<HttpResponseMessage> Get(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

            var allSimulations = SimulationManager.Instance.Get();
            var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects };
            var json = JsonConvert.SerializeObject(allSimulations, settings);
            message.Content = new StringContent(json);

            var response = Task<HttpResponseMessage>.Factory.StartNew(() => { return message; });
            return response;
        }

        /// <summary>
        /// Get a single simulation. The resource is specified as GET /TheResource('{name}');
        /// </summary>
        /// <param name="request">The request. </param>
        /// <param name="cancellationToken">Cancellation token. </param>
        /// <returns></returns>
        protected Task<HttpResponseMessage> GetByName(string name, HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

            var instance = SimulationManager.Instance.GetByName(name);
            string json = "";
            var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects };
            if (instance != null)
            {
                json = JsonConvert.SerializeObject(instance, settings);
            }
            else
            {
                message.StatusCode = System.Net.HttpStatusCode.NotFound;
            }
            message.Content = new StringContent(json);

            var response = Task<HttpResponseMessage>.Factory.StartNew(() => { return message; });
            return response;
        }

        /// <summary>
        /// Create a simulation. The resource is specified as POST /TheResource with the body content being the Json representation of a Simulation;
        /// </summary>
        /// <param name="s">The simulation to be created. This is typically passed to this handler via the Proxy class in Moksy.Common. </param>
        /// <param name="request">The request. </param>
        /// <param name="cancellationToken">Cancellation token. </param>
        /// <returns></returns>
        protected Task<HttpResponseMessage> Create(Moksy.Common.Simulation s, HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.Created);

            var response = Task<HttpResponseMessage>.Factory.StartNew(() => 
            { 
                bool added = Storage.SimulationManager.Instance.Add(s);
                if (!added)
                {
                    message.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    message.Content = new StringContent(string.Format("ERROR: The Simulation with a name of {0} already exists. ", s.Name));
                }
                return message;
            });
            return response;
        }

        /// <summary>
        /// Delete a single simulation. The resource is specified as DELETE /TheResource('{name}');
        /// </summary>
        /// <param name="request">The request. </param>
        /// <param name="cancellationToken">Cancellation token. </param>
        /// <returns></returns>
        protected Task<HttpResponseMessage> DeleteByName(string name, HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.NoContent);

            var response = Task<HttpResponseMessage>.Factory.StartNew(() => 
            {
                var moksyHeader = from T in request.Headers where T.Key == "moksy-purgedata" select T;
                var moksyRetainSimulation = from T in request.Headers where T.Key == "moksy-retainsimulation" select T;
                var deleteData = false;
                var retainSimulation = false;
                if (moksyHeader.Count() == 1)
                {
                    deleteData = true;
                }
                if (moksyRetainSimulation.Count() == 1)
                {
                    List<string> parts = new List<string>();
                    parts.AddRange(moksyRetainSimulation.First().Value);
                    var result = string.Join("", parts);

                    if (result.ToLower() == "true")
                    {
                        retainSimulation = true;
                    }
                }
                Storage.SimulationManager.Instance.Delete(name, deleteData, retainSimulation); 
                return message; 

            });
            return response;
        }
    }
}
