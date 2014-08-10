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
            Substitution s = new Substitution();

            var simulation = Storage.SimulationManager.Instance.Match(request.Method, request.Content, path, headers, true);
            if(simulation != null)
            {
                match= simulation.Simulation;
            }
            if (match != null)
            {
                var vars = CreateVariables("", match);
                vars["requestscheme"] = request.RequestUri.Scheme;
                vars["requesthost"] = request.RequestUri.Host;
                vars["requestport"] = request.RequestUri.Port.ToString();
                vars["requestroot"] = string.Format("{0}://{1}:{2}", request.RequestUri.Scheme, request.RequestUri.Host, request.RequestUri.Port.ToString());

                var evaluatedMatchingResponses = from T in simulation.EvaluatedMatchingConstraints select T.EvaluatedResponse;
                var evaluatedMatchingResponsesAsString = string.Join(",", evaluatedMatchingResponses.ToArray());
                vars["constraintResponses"] = string.Format("[{0}]", evaluatedMatchingResponsesAsString);

                var evaluatedNoneMatchingResponses = from T in simulation.EvaluatedNoneMatchingConstraints select T.EvaluatedResponse;
                var evaluatedNoneMatchingResponsesAsString = string.Join(",", evaluatedNoneMatchingResponses.ToArray());
                vars["violationResponses"] = string.Format("[{0}]", evaluatedNoneMatchingResponsesAsString);

                SubstituteHeaders(vars, match.Response.ResponseHeaders);

                var canned = HttpResponseMessageFactory.New(match.Response);

                if (match.Condition.IsImdb)
                {
                    if (match.Condition.HttpMethod == HttpMethod.Post)
                    {
                        // This rule has been set up with Post().ToImdb() and .AddToImdb() is in the Response. 
                        // We need to extract the body of the Request (which we assume to be Json) and add it to the Imdb. 
                        ByteArrayContent content = request.Content as ByteArrayContent;
                        string contentAsString = "";
                        if (content != null)
                        {
                            var task = content.ReadAsByteArrayAsync();
                            task.Wait();

                            contentAsString = new System.Text.ASCIIEncoding().GetString(task.Result);

                            contentAsString = SubstituteProperties(contentAsString, vars, match.Response.Properties);
                            vars["value"] = contentAsString;
                        }

                        if (match.Response.AddImdb)
                        {
                            Storage.SimulationManager.Instance.AddToImdb(match, path, contentAsString);
                        }

                        // We now need to populate the response with any variables that have been set up. 
                        if (match.Response.Content != null)
                        {
                            foreach (var p in match.Response.Properties)
                            {
                                vars[p.Name] = p.Value;
                            }
                            var result = s.Substitute(match.Response.Content, vars);
                            canned.Content = new StringContent(result);
                        }
                    }
                    else if (match.Condition.HttpMethod == HttpMethod.Put)
                    {
                        ByteArrayContent content = request.Content as ByteArrayContent;
                        string contentAsString = null;
                        if (content != null)
                        {
                            var task = content.ReadAsByteArrayAsync();
                            task.Wait();

                            contentAsString = new System.Text.ASCIIEncoding().GetString(task.Result);
                            contentAsString = SubstituteProperties(contentAsString, vars, match.Response.Properties);
                        }

                        if (match.Response.AddImdb)
                        {
                            // This rule has been set up with Put().ToImdb() and .AddToImdb() is in the Response. 
                            // We need to extract the body of the Request (which we assume to be Json) and add it to the Imdb. 
                            Storage.SimulationManager.Instance.AddToImdb(match, path, contentAsString);
                        }

                        var responseBody = match.Response.Content;
                        if (responseBody != null && responseBody != "")
                        {
                            if (contentAsString == null) contentAsString = "";

                            var variables = s.GetVariables(responseBody);
                            if (variables.Count > 0)
                            {
                                // The .Body("...{value}...") might have been specified (but there is at least one placeholder). 
                                vars["value"] = contentAsString;
                                responseBody = s.Substitute(responseBody, vars);
                            }

                            canned.Content = new StringContent(responseBody);
                        }
                    }
                    else if (match.Condition.HttpMethod == HttpMethod.Get)
                    {
                        // This rule has been set up with Get().FromImdb(). The default Body in the Return structure is {value} - ie: the raw value of the stored Json.
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
                                vars["value"] = candidateValue;
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
                            string candidateValue = "";
                            var variables = s.GetVariables(match.Response.Content);
                            if (variables.Count > 0)
                            {
                                candidateValue = JsonConvert.SerializeObject(existing);
                                vars["value"] = candidateValue;
                                candidateValue = s.Substitute(match.Response.Content, vars);
                            }

                            canned.Content = new StringContent(candidateValue);
                        }
                    }
                }
                else
                {
                    // We have our match - but we need to substitute anything in the content. 
                    if (match.Response.Content != null)
                    {
                        foreach (var p in match.Response.Properties)
                        {
                            vars[p.Name] = p.Value;
                        }
                        var result = s.Substitute(match.Response.Content, vars);
                        canned.Content = new StringContent(result);
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

        protected Dictionary<string, string> CreateVariables(string value, Simulation s)
        {
            Dictionary<string, string> vars = new Dictionary<string, string>();
            vars["value"] = string.Format("{0}", value);

            var vs = s.Response.CalculateVariables();
            foreach (var v in vs)
            {
                vars[v.Key] = v.Value;
            }

            return vars;
        }

        protected string SubstituteProperties(string content, Dictionary<string, string> variables, IEnumerable<Property> properties)
        {
            if (null == properties || properties.Count() == 0) return content;

            var result = content;

            JsonHelpers helpers = new JsonHelpers();
            Substitution s = new Substitution();
            foreach (var p in properties)
            {
                var value = s.Substitute(p.Value, variables);
                result = helpers.SetProperty(p.Name, value, result);
            }

            return result;
        }

        protected void SubstituteHeaders(Dictionary<string, string> variables, IEnumerable<Header> headers)
        {
            if (null == headers || headers.Count() == 0) return;

            JsonHelpers helpers = new JsonHelpers();
            Substitution s = new Substitution();
            foreach (var h in headers)
            {
                h.Value = s.Substitute(h.Value, variables);
            }
        }

    }
}
