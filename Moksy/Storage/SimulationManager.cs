using Moksy.Common;
using Moksy.Common.Constraints;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace Moksy.Storage
{
    /// <summary>
    /// Manager for Simulations. Used to store all currenlty active Simulations and any Imdb data associated with them. 
    /// </summary>
    // TODO: Refactor and tidy up.
    internal class SimulationManager
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public SimulationManager()
        {
            Storage = new SimulationCollection();
            Database = new Database();
        }

        public static SimulationManager Instance = new SimulationManager();



        /// <summary>
        /// Return all of the simulations that currently exist. 
        /// </summary>
        public SimulationCollection Get()
        {
            var result = Storage.Clone();
            return result;
        }

        /// <summary>
        /// Get a simulation by Name. Will return null if the simulation does not exist (is case insensitive). 
        /// </summary>
        /// <param name="name">Name of the simulation to retrieve. </param>
        /// <returns></returns>
        public Simulation GetByName(string name)
        {
            if (null == name) return null;

            var all = Get();
            var match = all.FirstOrDefault(f => string.Compare(f.Name, name, false) == 0);
            if (null == match) return null;

            return match;
        }



        /// <summary>
        /// Match the very first rule given the path and the headers. 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public Simulation Match(System.Net.Http.HttpMethod method, string path, string query, IEnumerable<Header> headers, bool decrement, string discriminator)
        {
            lock (Storage.SyncRoot)
            {
                
                SimulationConditionEvaluator e = new SimulationConditionEvaluator();

                Simulation match = null;
                match = e.Match(Storage, method, path, query, headers);
                if (method == HttpMethod.Get)
                {
                    Common.Match match2 = SimulationManager.Instance.Match(method, null, path, null, headers, decrement);
                    if (match2 != null)
                    {
                        match = match2.Simulation;
                    }
                }
                if (match != null)
                {
                    if (method == HttpMethod.Get)
                    {
                        var vars = new Substitution().GetVariables(match.Condition.Pattern);
                        if (vars.Count() > 0)
                        {
                            var result = MatchesGetFromImdb(match, path, vars.First().Name, discriminator);
                            return result;
                        }
                    }

                    if (decrement)
                    {
                        match.Condition.Repeat--;
                        if (match.Condition.Repeat == 0)
                        {
                            Storage.Remove(match);
                        }
                    }
                }
                return match;
            }
        }



        protected Simulation MatchesGetFromImdb(Simulation match, string path, string variable, string discriminator)
        {
            var resourceName = RouteParser.GetFirstResource(path, match.Condition.Pattern);
            if (null == resourceName) return null;

            // There are two possibilities:
            // 1. Persistence == Exists. In other words: an object with this property must exist for the match to occur.
            // 2. Persistence == NotExists. In other words: an object WITHOUT this property must exist for the match to occur. 

            if (!Database.ContainsResource(path, match.Condition.Pattern))
            {
                if (match.Condition.Persistence == Persistence.NotExists)
                {
                    // By definition: we would be able to add this item because as there is no Imdb the entry does not exist. 
                    return match;
                }
            }

            // ASSERTION: The in memory database exists. We now need to work out whether the value being requested exists or not. 
            Substitution s = new Substitution();
            var regex = RouteParser.ConvertPatternToRegularExpression(match.Condition.Pattern);

            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(regex);
            var result = rex.Match(path);
            if (!result.Success) return null;

            // TODO: We only get four matches back - the whole; prefix; value; end
            if (result.Groups.Count != 4) return null;

            var value = path.Substring(result.Groups[2].Index, result.Groups[2].Length);
            if (value == null) return null;

            var existingJson = FindMatch(path, match.Condition.Pattern, variable, value, discriminator);
            if (match.Condition.Persistence == Persistence.NotExists)
            {
                if (existingJson == null)
                {
                    return match;
                }

                return null;
            }
            if (match.Condition.Persistence == Persistence.Exists)
            {
                if (existingJson != null)
                {
                    return match;
                }

                return null;
            }

            // NOTE: If not condition is specified, they are saying something like: Get("/Pet('Dog')") and we must now match it as an exact match. 
            return match;
        }



        /// <summary>
        /// Look up an object given a path - such as /Pet('Dog'). 
        /// </summary>
        /// <param name="path">The raw path passed in via the request - ie: /Pet('Dog')</param>
        /// <returns>Null if an object does not exist; otherwise, the JObject (json) of the object indexes by that property. </returns>
        /// <remarks>This will look at all paths currently in the system (based on the method) 
        /// </remarks>
        public JObject GetFromImdb(System.Net.Http.HttpMethod method, string path, string query, IEnumerable<Header> headers, string discriminator)
        {
            if (null == path) return null;
            if (null == headers) headers = new List<Header>();

            lock (Storage.SyncRoot)
            {
                var match = Match(method, path, query, headers, false, discriminator);
                if (match == null) return null;

                if (method == HttpMethod.Get)
                {
                    return GetFromImdb(match, path, discriminator);
                }
            }

            return null; 
        }



        /// <summary>
        /// Return the object from the database given the existing simulation match and path. 
        /// </summary>
        /// <param name="match"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public JObject GetFromImdb(Simulation match, string path, string discriminator)
        {
            lock (Storage.SyncRoot)
            {
                var vars = new Substitution().GetVariables(match.Condition.Pattern);
                if (vars.Count() == 0) return null;

                // The path has placeholders such as /Get({id}). We need to pull out the resource.
                var resourceName = RouteParser.GetFirstResource(path, match.Condition.Pattern);
                if (null == resourceName) return null;

                if (!Database.ContainsResource(path, match.Condition.Pattern)) return null;

                Substitution s = new Substitution();
                var regex = RouteParser.ConvertPatternToRegularExpression(match.Condition.Pattern);

                System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(regex);
                var result = rex.Match(path);
                if (!result.Success) return null;

                // TODO: We only get four matches back - the whole; the prefix; kind; stem. 
                if (result.Groups.Count != 4) return null;

                var value = path.Substring(result.Groups[2].Index, result.Groups[2].Length);
                if (value == null) return null;

                var existingJson = FindMatch(path, match.Condition.Pattern, vars.First().Name, value, discriminator);
                return existingJson;
            }
        }



        /// <summary>
        /// Deletes the entry if it exists. 
        /// </summary>
        /// <param name="path">The raw path passed in via the request - ie: /Pet('Dog')</param>
        /// <returns>true if the object was deleted; false otherwise. </returns>
        /// <remarks>This will look at all paths currently in the system (based on the method) 
        /// </remarks>
        public bool DeleteFromImdb(System.Net.Http.HttpMethod method, string path, string pattern, string query, IEnumerable<Header> headers, string discriminator)
        {
            // TODO: Refactor. Like GetFromImdb
            if (null == path) return false;
            if (null == headers) headers = new List<Header>();

            lock (Storage.SyncRoot)
            {
                var match = Match(method, path, query, headers, false, discriminator);
                if (match == null) return false;

                if (method == HttpMethod.Delete)
                {
                    var vars = new Substitution().GetVariables(match.Condition.Pattern);
                    if (vars.Count() == 0) return false;

                    // The path has placeholders such as /Get({id}). We need to pull out the resource.
                    var resourceName = RouteParser.GetFirstResource(path, match.Condition.Pattern);
                    if (null == resourceName) return false;

                    if (!Database.ContainsResource(path,pattern)) return false;

                    Substitution s = new Substitution();
                    var regex = RouteParser.ConvertPatternToRegularExpression(match.Condition.Pattern);

                    System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(regex);
                    var result = rex.Match(path);
                    if (!result.Success) return false;

                    // TODO: We only get four matches back - the whole string; prefix; value; end. 
                    if (result.Groups.Count != 4) return false;

                    var value = path.Substring(result.Groups[2].Index, result.Groups[2].Length);
                    if (value == null) return false;

                    var removed = Remove(path, pattern, vars.First().Name, value, discriminator);
                    return removed;
                }
            }

            return false;
        }



        /// <summary>
        /// Remove an entry from the Imdb for the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pattern"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <param name="discriminator"></param>
        /// <returns></returns>
        public bool Remove(string path, string pattern, string propertyName, string propertyValue, string discriminator)
        {
            if (!Database.ContainsResource(path, pattern)) return false;
            if (null == propertyName) return false;

            var resource = Database.LookupResource(path, pattern);
            if (null == resource) return false;

            foreach (var e in resource.Data(discriminator))
            {
                try
                {
                    var jobject = JsonConvert.DeserializeObject(e) as JObject;
                    if (null == jobject) continue;

                    // ASSERTION: We are valid Json. 
                    var value = jobject[propertyName];
                    if (null == value && propertyValue == null)
                    {
                        Database.Remove(path, pattern, propertyName, propertyValue);
                        return true;
                    }
                    if (value == null) continue;

                    if (System.Convert.ToString((value as JValue).Value) == propertyValue)
                    {
                        Database.Remove(path, pattern, propertyName, propertyValue, discriminator);
                        return true;
                    }
                }
                catch (Exception)
                {
                    // If the entry is not valid Json - no problem; just continue. Try the next entry. 
                }
            }

            return false;
        }



        /// <summary>
        /// Match the very first rule given the path and the headers. The content is used to help evaluate for Post - if IsUniqueBy is specified in the simulation
        /// condition, then we need to perform that check before we match. 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content">The body content. </param>
        /// <param name="queryString">The encoded query string that was passed into the request. </param>
        /// <param name="headers"></param>
        /// <param name="decrement">If true, the counter of the simulation will be reduced after a match. </param>
        /// <returns></returns>
        public Moksy.Common.Match Match(System.Net.Http.HttpMethod method, HttpContent content, string path, string queryString, IEnumerable<Header> headers, bool decrement)
        {
            if (queryString == null) queryString = "";

            lock (Storage.SyncRoot)
            {
                SimulationConditionEvaluator e = new SimulationConditionEvaluator();
                var matches = e.Matches(Storage, content, method, path, queryString, headers);
                if (matches.Count() == 0) return null;

                foreach(var match in matches)
                {
                    if (method == HttpMethod.Post || method == HttpMethod.Put)
                    {
                        string contentAsString = "";

                        ByteArrayContent c = content as ByteArrayContent;
                        if (c != null)
                        {
                            var task = c.ReadAsByteArrayAsync();
                            task.Wait();

                            contentAsString = new System.Text.ASCIIEncoding().GetString(task.Result);
                        }

                        var matchingAssertions = FindMatchingConstraints(match, match.Condition.Constraints, contentAsString, GetDiscriminator(headers, match.Condition.ImdbHeaderDiscriminator));
                        var noneMatchingAsserations = FindNoneMatchingConstraints(match, match.Condition.Constraints, contentAsString);
                        if (match.Condition.Constraints.Count > 0 && matchingAssertions.Count() != match.Condition.Constraints.Count)
                        {
                            // This means that not every constraint that was specified was matched
                            // If .HasRuleViolations() is specified, this means it is OK to continue matching. 
                            if (match.Condition.HasAnyConstraintViolations)
                            {
                            }
                            else
                            {
                                // We want all constraints to be matched to continue. 
                                continue;
                            }
                        }
                        else
                        {
                            // All constraints have been met. 
                            if (match.Condition.HasAnyConstraintViolations) continue;
                        }

                        if (match.Condition.IndexProperty != null)
                        {
                            // NOTE: IndexProperty != null implies that a uniqueness constraint has been applied. 

                            // Can we add this new object? 
                            // NOTE: We have a complication to deal with here. What if we do this:
                            // PUT /Pet/Dog   , have a pattern of /Pet/{Kind},    but Json content of {"Kind":"Lizard"}
                            // canAddObject just indicates whether we can add a new object using the value of the content; not with the value overridden. 
                            bool exists = !CanAddObject(match, path, match.Condition.IndexProperty, contentAsString, GetDiscriminator(headers, match.Condition.ImdbHeaderDiscriminator));
                            var routes = RouteParser.Parse(path, match.Condition.Pattern);
                            var routeMatch = routes.FirstOrDefault(f => string.Compare(f.Name, match.Condition.IndexProperty.Replace("{", "").Replace("}", ""), false) == 0);
                            if(routeMatch != null)
                            {
                                // This means that the index property is specified in the route. It is that value we use to determine whether it already exists. 
                                var nestedMatch = FindMatch(match, path, routeMatch.Name, routeMatch.Value, GetDiscriminator(headers, match.Condition.ImdbHeaderDiscriminator));
                                if (nestedMatch != null)
                                {
                                    exists = true;
                                }
                            }
                            if (match.Condition.Persistence == Persistence.NotExists)
                            {
                                if (!exists)
                                {
                                    // An object with this property does not exist, therefore we can add it. 
                                    var t = new Common.Match() { Simulation = match };
                                    t.EvaluatedMatchingConstraints.AddRange(matchingAssertions);
                                    t.EvaluatedNoneMatchingConstraints.AddRange(noneMatchingAsserations);
                                    return t;
                                }

                                continue;
                            }
                            if (match.Condition.Persistence == Persistence.Exists)
                            {
                                if (exists)
                                {
                                    // The property already exists because we can't add it. We therefore have a match. 
                                    var t = new Common.Match() { Simulation = match };
                                    t.EvaluatedMatchingConstraints.AddRange(matchingAssertions);
                                    t.EvaluatedNoneMatchingConstraints.AddRange(noneMatchingAsserations);
                                    return t;
                                }

                                continue;
                            }

                            continue;
                        }
                        else
                        {
                            var t = new Common.Match() { Simulation = match };

                            if (matchingAssertions.Count() > 0)
                            {
                                t.EvaluatedMatchingConstraints.AddRange(matchingAssertions);
                                t.EvaluatedNoneMatchingConstraints.AddRange(noneMatchingAsserations);
                            }
                            return t;
                        }
                    }
                    if (match.Condition.HttpMethod == HttpMethod.Get)
                    {
                        var vars = new Substitution().GetVariables(match.Condition.Pattern);
                        if (vars.Count() > 0)
                        {
                            var result = MatchesGetFromImdb(match, path, vars.First().Name, GetDiscriminator(headers, match.Condition.ImdbHeaderDiscriminator));
                            if (null == result)
                            {
                                continue;
                            }

                            var t = new Common.Match() { Simulation = result };
                            return t;
                        }
                    }
                    if (match.Condition.HttpMethod == HttpMethod.Delete)
                    {
                        var vars = new Substitution().GetVariables(match.Condition.Pattern);
                        if (vars.Count() > 0)
                        {
                            var result = MatchesGetFromImdb(match, path, vars.First().Name, GetDiscriminator(headers, match.Condition.ImdbHeaderDiscriminator));
                            if (null == result)
                            {
                                continue;
                            }

                            var t = new Common.Match() { Simulation = result };
                            return t;
                        }
                    }

                    if (decrement)
                    {
                        match.Condition.Repeat--;
                        if (match.Condition.Repeat == 0)
                        {
                            Storage.Remove(match);
                        }
                    }

                    var t2 = new Common.Match() { Simulation = match };
                    return t2;
                }

                return null;
            }
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



        /// <summary>
        /// Add the simulation. The name must be unqiue by the Name (case insensitive). 
        /// </summary>
        /// <param name="simulation"></param>
        /// <returns>true - The simulation was added; false - The name is not unique</returns>
        public bool Add(Simulation simulation)
        {
            if (null == simulation) throw new System.ArgumentNullException("simulation");

            lock (Storage.SyncRoot)
            {
                var match = Storage.FirstOrDefault(f => string.Compare(f.Name, simulation.Name, true) == 0);
                if (match != null) return false;

                Storage.Add(simulation);
                return true;
            }
        }



        /// <summary>
        /// Add the content to the end point (path) specified in the simulation. 
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="path"></param>
        /// <param name="content"></param>
        public void AddToImdb(Simulation simulation, string path, string pattern, string content, string discriminator)
        {
            if (null == simulation) throw new System.ArgumentNullException("simulation");

            lock (Storage.SyncRoot)
            {
                var resourceName = RouteParser.GetFirstResource(path, simulation.Condition.Pattern);
                if (null == resourceName) return;

                if (simulation.Condition.ContentKind == ContentKind.BodyParameters)
                {
                    content = ConvertBodyParametersToJson(content);
                }

                Database.AddJson(path, pattern, simulation.Condition.IndexProperty, content, discriminator);
            }
        }




        /// <summary>
        /// Return true if the object represented by Json can be added. False otherwise. If the property is not present in the Json,
        /// it is assumed to be null. If the json is invalid, this will always return false. 
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="path">Path</param>
        /// <param name="propertyName"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public bool CanAddObject(Simulation simulation, string path, string propertyName, string json, string discriminator)
        {
            if (null == json) { json = ""; }

            try
            {
                string value = null;
                if (simulation.Condition.ContentKind == ContentKind.Json)
                {
                    value = GetPropertyValueFromJson(json, propertyName);
                    JObject job = JsonConvert.DeserializeObject(json) as JObject;
                    if (null == job) return false;
                
                }
                else if (simulation.Condition.ContentKind == ContentKind.BodyParameters)
                {
                    value = GetPropertyValueFromJson(ConvertBodyParametersToJson(json), propertyName);
                }
                else
                {
                    
                }
                return CanAdd(simulation, path, propertyName, value, discriminator);
            }
            catch
            {
                // If this is invalid Json, return false;
                return false;
            }
        }

        /// <summary>
        /// Returns the value of the property from json. If json is not valid json, or null, this will return null. If propertyName does not 
        /// exist in the json, this will return null. 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string GetPropertyValueFromJson(string json, string propertyName)
        {
            if (null == json) return null;
            if (null == propertyName) return null;

            try
            {
                JObject job = JsonConvert.DeserializeObject(json) as JObject;
                if (null == job) return null;

                propertyName = propertyName.Replace("{", "").Replace("}", "");

                var result = (job[propertyName] as JValue).Value.ToString();
                if (result == null) return result;

                return System.Convert.ToString(result);
            }
            catch
            {
                // If this is invalid Json, return false;
                return null;
            }
        }



        /// <summary>
        /// Convert body parameters - from the content of a request (which will be encoded) - into Json.
        /// </summary>
        /// <param name="bodyParameters"></param>
        /// <returns></returns>
        public string ConvertBodyParametersToJson(string bodyParameters)
        {
            JObject job = new JObject();
            var result = JsonConvert.SerializeObject(job);
            if (string.IsNullOrEmpty(bodyParameters)) return result;

            var pairs = bodyParameters.Split('&');
            foreach (var pair in pairs)
            {
                var parts = pair.Split('=');
                if (parts.Length == 1)
                {
                    job[parts[0]] = null;
                }
                else
                {
                    job[parts[0]] = RestSharp.Contrib.HttpUtility.UrlDecode(parts[1]);
                }
            }

            result = JsonConvert.SerializeObject(job);
            return result;
        }



        public string ConvertJsonToBodyParameters(string json)
        {
            return ConvertJsonToBodyParameters(json, true);
        }

        /// <summary>
        /// Converts a Json object into Name=Value pairs and encode the values. 
        /// </summary>
        /// <param name="json">The json object to convert. If a property is not a primitive type, it will be converted as a string value. </param>
        /// <param name="encode">If true, the return body parameters will be encoded so they are suitable for resubmission. </param>
        /// <returns></returns>
        public string ConvertJsonToBodyParameters(string json, bool encode)
        {
            if (string.IsNullOrEmpty(json)) return "";

            List<string> result = new List<string>();
            try
            {
                JObject job = JsonConvert.DeserializeObject(json) as JObject;
                foreach (var j in job.Properties())
                {
                    var v = job[j.Name];
                    var value = System.Convert.ToString(v);
                    if (value != null && encode)
                    {
                        value = RestSharp.Contrib.HttpUtility.UrlEncode(value);
                    }
                    string pair = string.Format("{0}={1}", j.Name, value);
                    result.Add(pair);
                }
            }
            catch
            {
                return json;
            }

            var s = string.Join("&", result);
            return s;
        }



        /// <summary>
        /// This method is typically called when only Json values are added. This will look at each object and work out whether candidatePropertyValue already exists. If so, this returns false;
        /// Othewise it returns true. 
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="path"></param>
        /// <param name="candidatePropertyValue"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool CanAdd(Simulation simulation, string path, string propertyName, string candidatePropertyValue, string discriminator)
        {
            var match = FindMatch(simulation, path, propertyName, candidatePropertyValue, discriminator);
            return (null == match);
        }



        /// <summary>
        /// Finds a match (or returns Null) for the given simulation Path. If a propertyName with candidatePropertyValue exists, this will return the
        /// JSon object that corresponds to it. 
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="path"></param>
        /// <param name="propertyName"></param>
        /// <param name="candidatePropertyValue"></param>
        /// <returns></returns>
        public JObject FindMatch(Simulation simulation, string path, string propertyName, string candidatePropertyValue, string discriminator)
        {
            if (null == simulation) throw new System.ArgumentNullException("simulation");

            var resourceName = RouteParser.GetFirstResource(path, simulation.Condition.Pattern);
            if (null == resourceName) return null;

            var resource = Database.LookupResource(path, simulation.Condition.Pattern);
            if (null == resource) return null;

            foreach (var e in resource.Data(discriminator))
            {
                try
                {
                    var jobject = JsonConvert.DeserializeObject(e) as JObject;
                    if (null == jobject) continue;

                    // ASSERTION: We are valid Json. 
                    var value = jobject[propertyName.Replace("{","").Replace("}", "")];
                    if (null == value && candidatePropertyValue == null) return jobject;
                    if (value == null) continue;

                    if (System.Convert.ToString((value as JValue).Value) == candidatePropertyValue)
                    {
                        return jobject;
                    }
                }
                catch (Exception)
                {
                    // If the entry is not valid Json - no problem; just continue. Try the next entry. 
                }
            }

            return null;
        }



        /// <summary>
        /// Find a match in the database using the path (typically: just the resource name), the propertyName that is used as the key and the propertyValue to match. 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public JObject FindMatch(string path, string pattern, string propertyName, string propertyValue, string discriminator)
        {
            if (!Database.ContainsResource(path, pattern)) return null;
            if (null == propertyName) return null;

            var resource = Database.LookupResource(path, pattern);
            if (null == resource) return null;

            foreach (var e in resource.Data(discriminator))
            {
                try
                {
                    var jobject = JsonConvert.DeserializeObject(e) as JObject;
                    if (null == jobject) continue;

                    // ASSERTION: We are valid Json. 
                    var value = jobject[propertyName];
                    if (null == value && propertyValue == null) return jobject;
                    if (value == null) continue;

                    if (System.Convert.ToString((value as JValue).Value) == propertyValue)
                    {
                        return jobject;
                    }
                }
                catch (Exception)
                {
                    // If the entry is not valid Json - no problem; just continue. Try the next entry. 
                }
            }

            return null;
        }



        /// <summary>
        /// Return a list of (possibly empty) list of keys in the given path. 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IEnumerable<string> GetKeysFor(string path, string pattern, string propertyName, string discriminator)
        {
            List<string> result = new List<string>();
            if (null == path) return result;
            if (null == propertyName) return result;

            var resourceName = RouteParser.GetFirstResource(path, pattern);
            if (null == resourceName) return result;

            var resource = Database.LookupResource(path, pattern);
            if (null == resource) return result;

            foreach (var p in resource.Data(discriminator))
            {
                var key = GetPropertyValueFromJson(p, propertyName);
                if (null == key) continue;

                result.Add(key);
            }

            return result;
        }



        /// <summary>
        /// Return all of the entries for the given path in the Imdb. 
        /// </summary>
        /// <param name="simulation"></param>
        /// <returns></returns>
        public string GetFromImdbAsText(Simulation simulation, string path, string discriminator)
        {
            if (null == simulation) throw new System.ArgumentNullException("simulation");

            lock (Storage.SyncRoot)
            {
                var resourceName = RouteParser.GetFirstResource(path, simulation.Condition.Pattern);
                if (null == resourceName) return null;

                var resource = Database.LookupResource(path, simulation.Condition.Pattern);
                if (null == resource) return null;

                StringBuilder builder = new StringBuilder();
                foreach (var e in resource.Data(discriminator))
                {
                    builder.Append(e);
                }
                return builder.ToString();
            }
        }



        /// <summary>
        /// Return all of the entries for the given path in the Imdb in Json format (this does NOT wrap the output; it just returns the entries separated by a ,
        /// </summary>
        /// <param name="simulation"></param>
        /// <returns></returns>
        public string GetFromImdbAsJson(Simulation simulation, string path, string discriminator)
        {
            if (null == simulation) throw new System.ArgumentNullException("simulation");

            lock (Storage.SyncRoot)
            {
                var resourceName = RouteParser.GetFirstResource(path, simulation.Condition.Pattern);
                if (null == resourceName) return null;

                var resource = Database.LookupResource(path, simulation.Condition.Pattern);
                if (null == resource) return null;

                List<string> components = new List<string>();
                foreach (var e in resource.Data(discriminator))
                {
                    components.Add(e);
                }
                var result = string.Join(",", components);
                return result;
            }
        }

        /// <summary>
        /// Remove an entry from storage with the given name. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Delete(string name)
        {
            return Delete(name, false);
        }

        /// <summary>
        /// Remove an entry from storage with the given name. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="deleteData">If true, all of the data associated with this resource will be purged. </param>
        /// <returns></returns>
        public bool Delete(string name, bool deleteData)
        {
            if (null == name) return false;

            lock (Storage.SyncRoot)
            {
                if (name == "*")
                {
                    Storage.Clear();
                    Database.RemoveAll();
                    return true;
                }
                var match = Storage.FirstOrDefault(f => string.Compare(f.Name, name, true) == 0);
                if (match == null) return false;

                Storage.Remove(match);

                if (deleteData)
                {
                    // TODO: Correct this... I need the path + pattern context. 

                    var resourceName = RouteParser.GetFirstResource(match.Condition.Pattern, match.Condition.Pattern);
                    if (null == resourceName) return false;

                    var resource = Database.LookupResource(match.Condition.Pattern, match.Condition.Pattern);
                    if (null == resource) return false;
                    
                    Database.RemoveResource(match.Condition.Pattern, match.Condition.Pattern);
                }

                return true;
            }
        }



        /// <summary>
        /// Will match 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public IEnumerable<ConstraintBase> FindMatchingConstraints(Simulation simulation, IEnumerable<ConstraintBase> constraints, string content, string discriminator)
        {
            List<ConstraintBase> result = new List<ConstraintBase>();
            if (constraints == null) return result;
            if (content == null) return result;

            if (simulation != null && simulation.Condition.ContentKind == ContentKind.BodyParameters)
            {
                content = ConvertBodyParametersToJson(content);
            }

            JObject jobject = null;
            try
            {
                jobject = JsonConvert.DeserializeObject(content) as JObject;

            }
            catch(Exception ex)
            {
            }

            if(jobject == null) return result;

            foreach (var c in constraints)
            {
                var e = c.Evaluate(jobject);
                if (e)
                {
                    c.EvaluatedResponse = c.GetState(jobject);
                    result.Add(c);
                }
            }

            return result;
        }

        /// <summary>
        /// Will match 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public IEnumerable<ConstraintBase> FindNoneMatchingConstraints(Simulation simulation, IEnumerable<ConstraintBase> constraints, string content)
        {
            List<ConstraintBase> result = new List<ConstraintBase>();
            if (constraints == null) return result;
            if (content == null) return result;

            if (simulation != null && simulation.Condition.ContentKind == ContentKind.BodyParameters)
            {
                content = ConvertBodyParametersToJson(content);
            }

            JObject jobject = null;
            try
            {
                jobject = JsonConvert.DeserializeObject(content) as JObject;

            }
            catch (Exception ex)
            {
            }

            if (jobject == null) return result;

            foreach (var c in constraints)
            {
                var e = c.Evaluate(jobject);
                if (!e)
                {
                    c.EvaluatedResponse = c.GetState(jobject);
                    result.Add(c);
                }
            }

            return result;
        }

        private SimulationCollection Storage;

        /// <summary>
        /// In-memory database for simulated resources. 
        /// </summary>
        private Database Database;
    }
}
