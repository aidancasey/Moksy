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
    /// Handler for everything that is not a /__Simulation. It is this handler that evaluates the headers and the resource against the Simulations
    /// and returns the expected response. 
    /// </summary>
    /// <remarks>If /__Simulation is hit with GET, POST or DELETE< then the SimulationMessageHandler is invoked. 
    /// If / anything else is hit, this handler is called and the path and headers used to find a simulation that will be used to set up the response. 
    /// </remarks>
    internal class GenericMessageHandler : HttpMessageHandler
    {
        public GenericMessageHandler()
        {
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var path = request.RequestUri.AbsolutePath;
            var headers = from T in request.Headers.ToArray() select new Header(T.Key, string.Join("", T.Value.ToArray()));

            // The .Match call will match EVERY aspect of the Simulation.Condition structure: the method, the headers, whether an existing object exists with that key 
            // and so forth. Exactly one value will be returned. The Match algorithm is greedy - it will match the very first one it finds, so add your most
            // specific simulations first. 
            Simulation match = null;
            match = Storage.SimulationManager.Instance.Match(request.Method, request.Content, path, headers, true);
            if (match != null)
            {
                var canned = HttpResponseMessageFactory.New(match.Response);

                if (match.Condition.IsImdb)
                {
                    if (match.Condition.HttpMethod == HttpMethod.Post && match.Response.AddImdb)
                    {
                        // This rule has been set up with Post().ToImdb() and .AddToImdb() is in the Response. 
                        // We need to extract the body of the Request (which we assume to be Json) and add it to the Imdb. 
                        ByteArrayContent content = request.Content as ByteArrayContent;
                        if (content != null)
                        {
                            var task = content.ReadAsByteArrayAsync();
                            task.Wait();

                            var contentAsString = new System.Text.ASCIIEncoding().GetString(task.Result);

                            Storage.SimulationManager.Instance.AddToImdb(match, path, contentAsString);
                        }
                    }
                    else if (match.Condition.HttpMethod == HttpMethod.Put && match.Response.AddImdb)
                    {
                        // This rule has been set up with Put().ToImdb() and .AddToImdb() is in the Response. 
                        // We need to extract the body of the Request (which we assume to be Json) and add it to the Imdb. 
                        ByteArrayContent content = request.Content as ByteArrayContent;
                        if (content != null)
                        {
                            var task = content.ReadAsByteArrayAsync();
                            task.Wait();

                            var contentAsString = new System.Text.ASCIIEncoding().GetString(task.Result);

                            Storage.SimulationManager.Instance.AddToImdb(match, path, contentAsString);
                        }
                    }
                    else if (match.Condition.HttpMethod == HttpMethod.Get)
                    {
                        // This rule has been set up with Get().FromImdb(). The default Body in the Return structure is {value} - ie: the raw value of the stored Json.
                        Substitution s = new Substitution();
                        var variables = s.GetVariables(match.Condition.Pattern);
                        if (variables.Count == 0)
                        {
                            // The path of the Get operation does not contain any placeholders. ie: it is a 'GetAll' or 'GetSpecific'. The path might look like:
                            // GET /TheResource or GET /TheResource('100'). 
                            // but it definitely does not contain placeholders for matching. ie:
                            // GET /TheResource({id})
                            string content = "";

                            if (match.Condition.ContentKind == ContentKind.Text)
                            {
                                content = Storage.SimulationManager.Instance.GetFromImdbAsText(match, path);
                            }
                            else
                            {
                                // We are Imdb. Each entry is a Json fragment. We concatenate them together and separate them with ,
                                content = Storage.SimulationManager.Instance.GetFromImdbAsJson(match, path);
                            }
                            if (content != null)
                            {
                                // We need to do substitution now... but only if the response content contains placeholders. 
                                if (match.Response.Content == null)
                                {
                                    // No explicit content was specified with the .Body() call in the Simulation Response. Therefore, we will return the default
                                    // data above. 
                                }
                                else
                                {
                                    // .Body("...somecontent...") was specified. 
                                    variables = s.GetVariables(match.Response.Content);
                                    if (variables.Count > 0)
                                    {
                                        // The .Body("...{value}...") might have been specified (but there is at least one placeholder). 
                                        Dictionary<string, string> vars = new Dictionary<string, string>();
                                        vars["value"] = content;

                                        content = s.Substitute(match.Response.Content, vars);
                                    }
                                    else
                                    {
                                        // A specific value was provided with no placeholders; return that fixed text. 
                                        content = match.Response.Content;
                                    }
                                }

                                if (content != null)
                                {
                                    canned.Content = new StringContent(content);
                                }
                            }
                        }
                        else
                        {
                            // The Path of the Simulation might be of the form /TheResource('{id}') where there is at least one placeholder. 
                            var jobject = Storage.SimulationManager.Instance.GetFromImdb(HttpMethod.Get, path, headers);
                            string candidateValue = "";
                            if (jobject != null)
                            {
                                candidateValue = JsonConvert.SerializeObject(jobject) as string;
                            }

                            // ASSERTION: We have a safe candidate value to use. This is either the empty string OR it is the object that has been retrieved. 

                            // We need to substitute the replacement. 
                            variables = s.GetVariables(match.Response.Content);
                            if (variables.Count > 0)
                            {
                                Dictionary<string, string> vars = new Dictionary<string, string>();
                                vars["value"] = string.Format("{0}", candidateValue);

                                candidateValue = s.Substitute(match.Response.Content, vars);
                            }
                            else
                            {
                                // Either the default .Body() was specified (ie: not at all) or we have a constant. 
                                if (match.Response.Content == null)
                                {
                                    // candidateValue contains the default value. 
                                }
                                else
                                {
                                    // We have a specified value we need to return. 
                                    candidateValue = match.Response.Content;
                                }
                            }

                            canned.Content = new StringContent(candidateValue);
                        }
                    }
                    else if (match.Condition.HttpMethod == HttpMethod.Delete && match.Condition.Persistence == Persistence.Exists)
                    {
                        // We need to remove the item IF it exists. 
                        var existing = Storage.SimulationManager.Instance.GetFromImdb(match, path);
                        if (existing != null)
                        {
                            if (match.Response.RemoveImdb)
                            {
                                Storage.SimulationManager.Instance.DeleteFromImdb(HttpMethod.Delete, path, match.Condition.Pattern, headers);
                            }
                            Substitution s = new Substitution();
                            string candidateValue = "";
                            var variables = s.GetVariables(match.Response.Content);
                            if (variables.Count > 0)
                            {
                                candidateValue = JsonConvert.SerializeObject(existing);

                                Dictionary<string, string> vars = new Dictionary<string, string>();
                                vars["value"] = string.Format("{0}", candidateValue);

                                candidateValue = s.Substitute(match.Response.Content, vars);
                            }

                            canned.Content = new StringContent(candidateValue);
                        }
                    }
                }

                return Task<HttpResponseMessage>.Factory.StartNew(() => { return canned; });
            }

            // Fallthrough: if nothing matches a simulation, return NotImplemented. 
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.NotImplemented);
            message.Content = new StringContent(string.Format("Enter the end-point as /{0} to view the Mosky simulations. ", Routes.SimulationRoute.SimulationName));
            var response = Task<HttpResponseMessage>.Factory.StartNew( () => {return message;});
            return response;
        }
    }
}
