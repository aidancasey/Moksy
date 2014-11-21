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
        public GenericMessageHandler(ApplicationDirectives directives)
        {
            if (directives == null) directives = new ApplicationDirectives();

            Directives = directives;
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

            if (Directives.Log)
            {
                try
                {
                    foreach (var header in headers)
                    {
                        Console.WriteLine(string.Format("{0}: {1}", header.Name, header.Value));
                    }

                    if (request.Content.IsMimeMultipartContent())
                    {
                        Console.WriteLine("The content is MultiPart so will not be dumped. ");
                    }
                    else
                    {
                        ByteArrayContent content = request.Content as ByteArrayContent;
                        string contentAsString = "";
                        if (content != null)
                        {
                            var task = content.ReadAsByteArrayAsync();
                            task.Wait();

                            contentAsString = new System.Text.ASCIIEncoding().GetString(task.Result);
                            Console.WriteLine(contentAsString);
                        }
                    }
                }
                catch
                {
                }
                Console.WriteLine("");
            }

            var simulation = Storage.SimulationManager.Instance.Match(request.Method, request.Content, path, request.RequestUri.Query, headers, true);
            if(simulation != null)
            {
                match= simulation.Simulation;
            }
            if (match != null)
            {
                var discriminator = GetDiscriminator(headers, match.Condition.ImdbHeaderDiscriminator);

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
                        byte[] contentAsBytes = new byte[0];
                        if (content != null)
                        {
                            if (match.Condition.ContentKind == ContentKind.File && request.Content.IsMimeMultipartContent())
                            {
                                // NOTE: TODO: Lots of assumptions here... will need to modify this when I support real Multipart. 
                                var task = request.Content.ReadAsMultipartAsync();
                                task.Wait();

                                var fileContent = task.Result.Contents.FirstOrDefault(f => f.Headers.Contains("Content-Disposition"));
                                var taskContent = fileContent.ReadAsByteArrayAsync();
                                taskContent.Wait();
                                contentAsBytes = taskContent.Result;
                            }
                            else
                            {
                                var task = content.ReadAsByteArrayAsync();
                                task.Wait();

                                contentAsBytes = task.Result;
                                contentAsString = new System.Text.ASCIIEncoding().GetString(task.Result);
                            }
                            
                            if (match.Condition.ContentKind != ContentKind.File)
                            {
                                contentAsString = SubstituteProperties(contentAsString, path, match, vars, match.Response.Properties);
                                vars["value"] = contentAsString;
                            }
                            else
                            {
                                contentAsString = string.Format(@"{{""BinaryContentIdentity"":""{0}""}}", vars["BinaryContentIdentity"]);
                            }
                        }

                        if (match.Response.AddImdb)
                        {
                            if (match.Condition.ContentKind != ContentKind.File)
                            {
                                Storage.SimulationManager.Instance.AddToImdb(match, path, match.Condition.Pattern, contentAsString, discriminator);

                                var retrievedData = Storage.SimulationManager.Instance.GetFromImdbAsJson(match, path, discriminator);

                                vars["valueAsJson"] = retrievedData;
                                vars["valueAsBodyParameters"] = Storage.SimulationManager.Instance.ConvertJsonToBodyParameters(retrievedData, true);
                                vars["valueAsBodyParametersNotEncoded"] = Storage.SimulationManager.Instance.ConvertJsonToBodyParameters(retrievedData, false);
                            }
                            else
                            {
                                Storage.SimulationManager.Instance.AddToImdb(match, path, match.Condition.Pattern, contentAsString, contentAsBytes, discriminator);
                            }
                        }

                        // We now need to populate the response with any variables that have been set up. 
                        if (match.Condition.ContentKind != ContentKind.File)
                        {
                            // We only do the replacement if we are in text format. 
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
                    }
                    else if (match.Condition.HttpMethod == HttpMethod.Put)
                    {
                        ByteArrayContent content = request.Content as ByteArrayContent;
                        string contentAsString = "";
                        byte[] contentAsBytes = new byte[0];
                        if (content != null)
                        {
                            if (match.Condition.ContentKind == ContentKind.File && request.Content.IsMimeMultipartContent())
                            {
                                // NOTE: TODO: Lots of assumptions here... will need to modify this when I support real Multipart. 
                                var task = request.Content.ReadAsMultipartAsync();
                                task.Wait();

                                var fileContent = task.Result.Contents.FirstOrDefault(f => f.Headers.Contains("Content-Disposition"));
                                var taskContent = fileContent.ReadAsByteArrayAsync();
                                taskContent.Wait();
                                contentAsBytes = taskContent.Result;
                            }
                            else
                            {
                                var task = content.ReadAsByteArrayAsync();
                                task.Wait();

                                contentAsBytes = task.Result;
                                contentAsString = new System.Text.ASCIIEncoding().GetString(task.Result);
                            }

                            if (match.Condition.ContentKind != ContentKind.File)
                            {
                                contentAsString = SubstituteProperties(contentAsString, path, match, vars, match.Response.Properties);
                                vars["value"] = contentAsString;
                            }
                        }

                        if (match.Response.AddImdb)
                        {
                            if (match.Condition.ContentKind != ContentKind.File)
                            {
                                Storage.SimulationManager.Instance.AddToImdb(match, path, match.Condition.Pattern, contentAsString, discriminator);

                                var retrievedData = Storage.SimulationManager.Instance.GetFromImdbAsJson(match, path, discriminator);

                                vars["valueAsJson"] = retrievedData;
                                vars["valueAsBodyParameters"] = Storage.SimulationManager.Instance.ConvertJsonToBodyParameters(retrievedData, true);
                                vars["valueAsBodyParametersNotEncoded"] = Storage.SimulationManager.Instance.ConvertJsonToBodyParameters(retrievedData, false);
                            }
                            else
                            {
                                var entry = Storage.SimulationManager.Instance.GetEntryFromImdb(match, path, discriminator);
                                if (entry != null)
                                {
                                    entry.Bytes = contentAsBytes;
                                }
                            }
                        }

                        // We now need to populate the response with any variables that have been set up. 
                        if (match.Condition.ContentKind != ContentKind.File)
                        {
                            // We only do the replacement if we are in text format. 
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
                                content = Storage.SimulationManager.Instance.GetFromImdbAsText(match, path, discriminator);
                            }
                            else if (match.Condition.ContentKind == ContentKind.Json || match.Condition.ContentKind == ContentKind.BodyParameters)
                            {
                                // We are Imdb. Each entry is a Json fragment. We concatenate them together and separate them with ,
                                content = Storage.SimulationManager.Instance.GetFromImdbAsJson(match, path, discriminator);
                                vars["valueAsJson"] = content;
                                vars["valueAsBodyParameters"] = Storage.SimulationManager.Instance.ConvertJsonToBodyParameters(content, true);
                                vars["valueAsBodyParametersNotEncoded"] = Storage.SimulationManager.Instance.ConvertJsonToBodyParameters(content, false);

                                if (match.Condition.ContentKind == ContentKind.BodyParameters)
                                {
                                    content = Storage.SimulationManager.Instance.ConvertJsonToBodyParameters(content);
                                }
                            }
                            if (content == null) content = "";

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
                            var entry = Storage.SimulationManager.Instance.GetEntryFromImdb(HttpMethod.Get, path, request.RequestUri.Query, headers, discriminator);
                            string candidateValue = "";
                            if (entry != null && entry.Json != null)
                            {
                                candidateValue = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(entry.Json));
                            }

                            // We are Imdb. Each entry is a Json fragment. We concatenate them together and separate them with ,
                            vars["valueAsJson"] = candidateValue;
                            vars["valueAsBodyParameters"] = Storage.SimulationManager.Instance.ConvertJsonToBodyParameters(candidateValue, true);
                            vars["valueAsBodyParametersNotEncoded"] = Storage.SimulationManager.Instance.ConvertJsonToBodyParameters(candidateValue, false);

                            if (match.Condition.ContentKind == ContentKind.BodyParameters)
                            {
                                candidateValue = Storage.SimulationManager.Instance.ConvertJsonToBodyParameters(candidateValue);
                            }

                            // ASSERTION: We have a safe candidate value to use. This is either the empty string OR it is the object that has been retrieved. 

                            if (match.Condition.ContentKind == ContentKind.File && entry != null)
                            {
                                if (entry.Bytes == null) entry.Bytes = new Byte[0];

                                canned.Content = new ByteArrayContent(entry.Bytes);
                            }
                            else
                            {
                                // We need to substitute the replacement. 
                                if (candidateValue == null) candidateValue = "";

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
                    }
                    else if (match.Condition.HttpMethod == HttpMethod.Delete && match.Condition.Persistence == Persistence.Exists)
                    {
                        // We need to remove the item IF it exists. 
                        var existing = Storage.SimulationManager.Instance.GetFromImdb(match, path, discriminator);
                        if (existing != null)
                        {
                            if (match.Response.RemoveImdb)
                            {
                                Storage.SimulationManager.Instance.DeleteFromImdb(HttpMethod.Delete, path, match.Condition.Pattern, request.RequestUri.Query, headers, discriminator);
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
                    if (match.Response.ContentKind == ContentKind.File)
                    {
                        canned.Content = new ByteArrayContent(match.Response.ContentAsBytes);
                    }
                    else
                    {
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
                }

                if (simulation != null)
                {
                    if (simulation.Simulation.Response.ResponseHeaders.Count > 0)
                    {
                        canned.Headers.Clear();
                        if (canned.Content != null && canned.Content.Headers != null)
                        {
                            canned.Content.Headers.Clear();
                        }
                        foreach (var h in simulation.Simulation.Response.ResponseHeaders)
                        {
                            if (string.Compare(h.Name, "Content-Type", true) == 0)
                            {
                                if (canned.Content != null && canned.Content.Headers != null)
                                {
                                    if (canned.Content.Headers.Contains(h.Name))
                                    {
                                        canned.Content.Headers.Remove(h.Name);
                                    }
                                    canned.Content.Headers.Add(h.Name, h.Value);
                                }
                            }
                            else
                            {
                                canned.Headers.Add(h.Name, h.Value);
                            }
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

        protected string GetDiscriminator(IEnumerable<Header> headers, string key)
        {
            if (null == headers) return null;
            if (headers.Count() == 0) return null;
            if (null == key) return null;

            var match = headers.FirstOrDefault(f => f.Name == key);
            if (null == match) return null;

            return match.Value;
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

            if (s.Condition.ContentKind == ContentKind.File)
            {
                if (!vars.ContainsKey("BinaryContentIdentity"))
                {
                    vars["BinaryContentIdentity"] = System.Guid.NewGuid().ToString();
                }
            }

            return vars;
        }

        protected string SubstituteProperties(string content, string path, Simulation match, Dictionary<string, string> variables, IEnumerable<Property> properties)
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

            var tokens = RouteParser.Parse(path, match.Condition.Pattern).Where(f => f.Kind == RouteTokenKind.Property);
            foreach (var token in tokens)
            {
                var value = s.Substitute(token.Value, variables);
                result = helpers.SetProperty(token.Name, value, result);
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

        protected readonly ApplicationDirectives Directives;
    }
}
