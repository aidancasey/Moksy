using Moksy.Common;
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
        public Simulation Match(System.Net.Http.HttpMethod method, string path, IEnumerable<Header> headers, bool decrement)
        {
            lock (Storage.SyncRoot)
            {
                SimulationConditionEvaluator e = new SimulationConditionEvaluator();
                var match = e.Match(Storage, method, path, headers);
                if (match != null)
                {
                    if (method == HttpMethod.Get)
                    {
                        var vars = new Substitution().GetVariables(match.Condition.Path);
                        if (vars.Count() > 0)
                        {
                            var result = MatchesGetFromImdb(match, path, vars.First().Key);
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

        protected Simulation MatchesGetFromImdb(Simulation match, string path, string variable)
        {
            // TODO: Split out and improve the resource parsing. 
            // The path has placeholders such as /Get({id}). We need to pull out the resource.
            var candidateResourceName = Regex.Match(match.Condition.Path, "^/[A-Za-z0-9]*");
            if (!candidateResourceName.Success) return null;

            string resourceName = candidateResourceName.Value;

            // There are two possibilities:
            // 1. Persistence == Exists. In other words: an object with this property must exist for the match to occur.
            // 2. Persistence == NotExists. In other words: an object WITHOUT this property must exist for the match to occur. 

            if (!InMemoryDatabase.ContainsKey(resourceName))
            {
                if (match.Condition.Persistence == Persistence.NotExists)
                {
                    // By definition: we would be able to add this item because as there is no Imdb the entry does not exist. 
                    return match;
                }
            }

            // ASSERTION: The in memory database exists. We now need to work out whether the value being requested exists or not. 
            Substitution s = new Substitution();
            var regex = s.ConvertPatternToRegularExpression(match.Condition.Path);

            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(regex);
            var result = rex.Match(path);
            if (!result.Success) return null;

            // TODO: We only get two matches back - the whole string and the text in the middle (the identity)
            if (result.Groups.Count != 2) return null;

            var value = path.Substring(result.Groups[1].Index, result.Groups[1].Length);
            if (value == null) return null;

            var existingJson = FindMatch(resourceName, variable, value);
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
        public JObject GetFromImdb(System.Net.Http.HttpMethod method, string path, IEnumerable<Header> headers)
        {
            if (null == path) return null;
            if (null == headers) headers = new List<Header>();

            lock (Storage.SyncRoot)
            {
                var match = Match(method, path, headers, false);
                if (match == null) return null;

                if (method == HttpMethod.Get)
                {
                    return GetFromImdb(match, path);
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
        public JObject GetFromImdb(Simulation match, string path)
        {
            lock (Storage.SyncRoot)
            {
                var vars = new Substitution().GetVariables(match.Condition.Path);
                if (vars.Count() == 0) return null;

                // The path has placeholders such as /Get({id}). We need to pull out the resource.
                var candidateResourceName = Regex.Match(match.Condition.Path, "^/[A-Za-z0-9]*");
                if (!candidateResourceName.Success) return null;

                string resourceName = candidateResourceName.Value;
                if (!InMemoryDatabase.ContainsKey(resourceName)) return null;

                Substitution s = new Substitution();
                var regex = s.ConvertPatternToRegularExpression(match.Condition.Path);

                System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(regex);
                var result = rex.Match(path);
                if (!result.Success) return null;

                // TODO: We only get two matches back - the whole string and the text in the middle (the identity)
                if (result.Groups.Count != 2) return null;

                var value = path.Substring(result.Groups[1].Index, result.Groups[1].Length);
                if (value == null) return null;

                var existingJson = FindMatch(resourceName, vars.First().Key, value);
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
        public bool DeleteFromImdb(System.Net.Http.HttpMethod method, string path, IEnumerable<Header> headers)
        {
            // TODO: Refactor. Like GetFromImdb
            if (null == path) return false;
            if (null == headers) headers = new List<Header>();

            lock (Storage.SyncRoot)
            {
                var match = Match(method, path, headers, false);
                if (match == null) return false;

                if (method == HttpMethod.Delete)
                {
                    var vars = new Substitution().GetVariables(match.Condition.Path);
                    if (vars.Count() == 0) return false;

                    // The path has placeholders such as /Get({id}). We need to pull out the resource.
                    var candidateResourceName = Regex.Match(match.Condition.Path, "^/[A-Za-z0-9]*");
                    if (!candidateResourceName.Success) return false;

                    string resourceName = candidateResourceName.Value;
                    if (!InMemoryDatabase.ContainsKey(resourceName)) return false;

                    Substitution s = new Substitution();
                    var regex = s.ConvertPatternToRegularExpression(match.Condition.Path);

                    System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(regex);
                    var result = rex.Match(path);
                    if (!result.Success) return false;

                    // TODO: We only get two matches back - the whole string and the text in the middle (the identity)
                    if (result.Groups.Count != 2) return false;

                    var value = path.Substring(result.Groups[1].Index, result.Groups[1].Length);
                    if (value == null) return false;

                    var removed = Remove(resourceName, vars.First().Key, value);
                    return removed;
                }
            }

            return false;
        }



        /// <summary>
        /// Remove an entry from the Imdb for the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public bool Remove(string path, string propertyName, string propertyValue)
        {
            if (!InMemoryDatabase.ContainsKey(path)) return false;
            if (null == propertyName) return false;

            foreach (var e in InMemoryDatabase[path])
            {
                try
                {
                    var jobject = JsonConvert.DeserializeObject(e) as JObject;
                    if (null == jobject) continue;

                    // ASSERTION: We are valid Json. 
                    var value = jobject[propertyName];
                    if (null == value && propertyValue == null)
                    {
                        InMemoryDatabase[path].Remove(e);
                        return true;
                    }
                    if (value == null) continue;

                    if (System.Convert.ToString((value as JValue).Value) == propertyValue)
                    {
                        InMemoryDatabase[path].Remove(e);
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
        /// <param name="headers"></param>
        /// <returns></returns>
        public Simulation Match(System.Net.Http.HttpMethod method, HttpContent content, string path, IEnumerable<Header> headers, bool decrement)
        {
            lock (Storage.SyncRoot)
            {
                SimulationConditionEvaluator e = new SimulationConditionEvaluator();
                var matches = e.Matches(Storage, method, path, headers);
                if (matches.Count() == 0) return null;

                foreach(var match in matches)
                {
                    if (method == HttpMethod.Post)
                    {
                        if (match.Condition.IndexProperty != null)
                        {
                            // NOTE: IndexProperty != null implies that a uniqueness constraint has been applied. 
                            ByteArrayContent c = content as ByteArrayContent;
                            if (c != null)
                            {
                                var task = c.ReadAsByteArrayAsync();
                                task.Wait();

                                var contentAsString = new System.Text.ASCIIEncoding().GetString(task.Result);

                                // Can we add this new object?
                                bool canAddObject = CanAddObject(match, match.Condition.IndexProperty, contentAsString);
                                if (match.Condition.Persistence == Persistence.NotExists)
                                {
                                    if (canAddObject)
                                    {
                                        // An object with this property does not exist, therefore we can add it. 
                                        return match;
                                    }

                                    continue;
                                }
                                if (match.Condition.Persistence == Persistence.Exists)
                                {
                                    if (!canAddObject)
                                    {
                                        // The property already exists because we can't add it. We therefore have a match. 
                                        return match;
                                    }

                                    continue;
                                }

                                continue;
                            }
                        }
                    }
                    if (match.Condition.HttpMethod == HttpMethod.Get)
                    {
                        var vars = new Substitution().GetVariables(match.Condition.Path);
                        if (vars.Count() > 0)
                        {
                            var result = MatchesGetFromImdb(match, path, vars.First().Key);
                            if (null == result)
                            {
                                continue;
                            }
                            return result;
                        }
                    }
                    if (match.Condition.HttpMethod == HttpMethod.Delete)
                    {
                        var vars = new Substitution().GetVariables(match.Condition.Path);
                        if (vars.Count() > 0)
                        {
                            var result = MatchesGetFromImdb(match, path, vars.First().Key);
                            if (null == result)
                            {
                                continue;
                            }
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

                    return match;
                }

                return null;
            }
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
        /// <param name="content"></param>
        public void AddToImdb(Simulation simulation, string content)
        {
            if (null == simulation) throw new System.ArgumentNullException("simulation");

            lock (Storage.SyncRoot)
            {
                if (!InMemoryDatabase.ContainsKey(simulation.Condition.Path))
                {
                    InMemoryDatabase[simulation.Condition.Path] = new List<string>();
                }

                InMemoryDatabase[simulation.Condition.Path].Add(content);
            }
        }

        /// <summary>
        /// Return true if the object represented by Json can be added. False otherwise. If the property is not present in the Json,
        /// it is assumed to be null. If the json is invalid, this will always return false. 
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="propertyName"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public bool CanAddObject(Simulation simulation, string propertyName, string json)
        {
            if (null == json) { json = ""; }

            try
            {
                JObject job = JsonConvert.DeserializeObject(json) as JObject;
                if(null == job) return false;

                var value = GetPropertyValueFromJson(json, propertyName);
                return CanAdd(simulation, propertyName, value);
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
        /// This method is typically called when only Json values are added. This will look at each object and work out whether candidatePropertyValue already exists. If so, this returns false;
        /// Othewise it returns true. 
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool CanAdd(Simulation simulation, string propertyName, string candidatePropertyValue)
        {
            var match = FindMatch(simulation, propertyName, candidatePropertyValue);
            return (null == match);
        }

        /// <summary>
        /// Finds a match (or returns Null) for the given simulation Path. If a propertyName with candidatePropertyValue exists, this will return the
        /// JSon object that corresponds to it. 
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="propertyName"></param>
        /// <param name="candidatePropertyValue"></param>
        /// <returns></returns>
        public JObject FindMatch(Simulation simulation, string propertyName, string candidatePropertyValue)
        {
            if (null == simulation) throw new System.ArgumentNullException("simulation");

            if (!InMemoryDatabase.ContainsKey(simulation.Condition.Path))
            {
                // If the in memory database does not contain this path, then by definition we can add it: so it is unique. 
                return null;
            }

            foreach (var e in InMemoryDatabase[simulation.Condition.Path])
            {
                try
                {
                    var jobject = JsonConvert.DeserializeObject(e) as JObject;
                    if (null == jobject) continue;

                    // ASSERTION: We are valid Json. 
                    var value = jobject[propertyName];
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
        public JObject FindMatch(string path, string propertyName, string propertyValue)
        {
            if (!InMemoryDatabase.ContainsKey(path)) return null;
            if (null == propertyName) return null;

            foreach (var e in InMemoryDatabase[path])
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
        /// Return a list of (possibly empty) list of keys in the given path. The path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IEnumerable<string> GetKeysFor(string path, string propertyName)
        {
            List<string> result = new List<string>();
            if (null == path) return result;
            if (null == propertyName) return result;

            if (!InMemoryDatabase.ContainsKey(path))
            {
                return result;
            }

            foreach (var p in InMemoryDatabase[path])
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
        public string GetFromImdbAsText(Simulation simulation)
        {
            if (null == simulation) throw new System.ArgumentNullException("simulation");

            lock (Storage.SyncRoot)
            {
                if (InMemoryDatabase.ContainsKey(simulation.Condition.Path))
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (var e in InMemoryDatabase[simulation.Condition.Path])
                    {
                        builder.Append(e);
                    }
                    return builder.ToString();
                }

                return null;
            }
        }

        /// <summary>
        /// Return all of the entries for the given path in the Imdb in Json format (this does NOT wrap the output; it just returns the entries separated by a ,
        /// </summary>
        /// <param name="simulation"></param>
        /// <returns></returns>
        public string GetFromImdbAsJson(Simulation simulation)
        {
            if (null == simulation) throw new System.ArgumentNullException("simulation");

            lock (Storage.SyncRoot)
            {
                if (InMemoryDatabase.ContainsKey(simulation.Condition.Path))
                {
                    List<string> components = new List<string>();
                    foreach (var e in InMemoryDatabase[simulation.Condition.Path])
                    {
                        components.Add(e);
                    }
                    var result = string.Join(",", components);
                    return result;
                }

                return null;
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
                    InMemoryDatabase.Clear();
                    return true;
                }
                var match = Storage.FirstOrDefault(f => string.Compare(f.Name, name, true) == 0);
                if (match == null) return false;

                Storage.Remove(match);

                if (deleteData)
                {
                    var candidateResourceName = Regex.Match(match.Condition.Path, "^/[A-Za-z0-9]*");
                    if (!candidateResourceName.Success) return true;

                    if (InMemoryDatabase.ContainsKey(candidateResourceName.Value))
                    {
                        InMemoryDatabase.Remove(candidateResourceName.Value);
                    }
                }

                return true;
            }
        }

        private SimulationCollection Storage;

        /// <summary>
        /// Used to store data POSTED to end-points. The key is the end-point (the full path). The list is the order in which items 
        /// were posted. 
        /// </summary>
        private Dictionary<string, List<string>> InMemoryDatabase = new Dictionary<string, List<string>>();
    }
}
