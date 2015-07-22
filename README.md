# Moksy v1.0
----------
See http:/www.brekit.com/moksy for more information. 


# Why?
----
Moksy is an open source .Net library for stubbing, mocking and simulating web services. 

Intended to be driven from MsTest (or your favorite testing framework), Moksy will create a real HTTP Server end-point that your system under test or other services can hit. 

For example:

		Moksy.Common.Proxy proxy = new Moksy.Common.Proxy(10011);
		proxy.Start();

		var simulation = SimulationFactory.When.I.Get().From("/TheEndpoint").Then.Return.Body("Hello World!").And.StatusCode(System.Net.HttpStatusCode.OK);
		proxy.Add(simulation);

Navigating to http://localhost:10011/TheEndpoint in your browser or hitting that Url from another service will return "Hello World!". 

This release has a strong focus on testing JSON-based web services. 

To get up and running quickly: see "How To Use Moksy In Your Own Integration Tests" below. 



# In-Memory Database:
-------------------
Simple canned responses for common verbs - POST, PATCH, DELETE and so forth - like above are ideal in many cases. However, there is a very common use case when developing REST API's using the Micro/Macro service pattern: we often want to support Create, Read and Delete functionality to help UI devs and testers get up and running quickly. 

Moksy does this for your automatically by creating an 'In Memory Database' for Json submitted to that resource. A property in the Json is specified as the 'Index' for uniqueness (optional). For example - assuming a simple Pet structure like this:

{ "Kind" : "Dog" }

We might use the following to set up an simulate a Pet service:

		SimulationFactory.When.I.Post().ToImdb("/Pet").And.NotExists("Kind").Return.StatusCode(System.Net.HttpStatusCode.Created).And.AddToImdb();
		SimulationFactory.When.I.Post().ToImdb("/Pet").And.Exists("Kind").Return.StatusCode(System.Net.HttpStatusCode.BadRequest).And.Body("A Pet of that kind already exists");
		
		SimulationFactory.When.I.Get().FromImdb("/Pet").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body("[{value}]);
		SimulationFactory.When.I.Get().FromImdb("/Pet/{kind}").And.Exists().Then.Return.StatusCode(System.Net.HttpStatusCode.OK);
		SimulationFactory.When.I.Get().FromImdb("/Pet/{kind}").And.NotExists().Then.Return.StatusCode(System.Net.HttpStatusCode.NotFound);

		SimulationFactory.When.I.Delete().FromImdb("/Pet/{kind}").And.Exists().Then.Return.StatusCode(System.Net.HttpStatusCode.NoContent).And.RemoveFromImdb();
		SimulationFactory.When.I.Delete().FromImdb("/Pet/{kind}").And.NotExists().Then.Return.StatusCode(System.Net.HttpStatusCode.NoContent);
		
		SimulationFactory.When.I.Put().ToImdb("/Pet").And.NotExists("Kind").Return.StatusCode(System.Net.HttpStatusCode.Created).And.AddToImdb().And.Return.Body("{value}");
		SimulationFactory.When.I.Put().ToImdb("/Pet").And.Exists("Kind").Return.StatusCode(System.Net.HttpStatusCode.OK).And.AddToImdb().And.Return.Body("{value}");		
		
Posting the Json structure to http://localhost:10011/Pet will add it to the in memory database; calling GET on http://localhost:10011 will return a comma-separated
list of the Json entries wrapped in [] to simulate an array. Of course, calling GET on http://localhost:10011/Dog will return just the above structure. 


# More Advanced Samples
------------------------------------------------

### Variables and Properties
The Problem: [the application is currently limited to GUIDS]
When we add an object to the Imdb, we sometimes need to give it an identity. This typically a GUID or some hash but is always opaque. As a client, we cannot provide this identity because it is server calculated. 

Moksy can support this by creating a dynamic Variable in the Response and then using the OverrideProperty method to assign that Variable to the value of a Property in the Json. 
For example:

	When.I.Post().ToImdb("/Pet").And.NotExists("Kind").Then.Return.StatusCode(System.Net.HttpStatusCode.Created).With.Variable("Identity").OverrideProperty("Id", "{Identity}").AddToImdb().Body("{Identity}");

That will set "Id" to a new Guid (the Variable() method will calculate a new Guid on every simulation assuming no hard coded value is provided). Id is then a property of the submitted / stored Json object.

With that in mind, we can 'round trip' by returning the object using that Identity:

	When.I.Get().FromImdb("/Pet/{Id}").And.Exists("Id").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).And.Body("{value}");

Variables are referenced in the Body using {...} notation. There are five variables that are always present to every simulation:

	{requestscheme}			- ie: http
	{requesthost}			- ie: localhost
	{requestport}			- ie: 10011
	{requestroot}			- ie: http://localhost:10011
	{value}                 - The value of an object from the Imdb. 

This information can be used (for example) in 'inject' the Location of an object in a Body or Response header. For example:

	When.I.Post().ToImdb("/Pet").And.NotExists("Kind").Then.Return.StatusCode(System.Net.HttpStatusCode.Created).With.Variable("Identity").OverrideProperty("Id", "{Identity}").AddToImdb().Body("{Identity}").With.Header("Location", "{uriroot}/Pet/{Identity}");

### Example - Scoping a Moksy Simulation to a querystring parameter value
```C#

 // in this example I only want to simulate oauth calls where the querystring parameter 'scope'
 // is passed in with the value of 'userinfo.profile'
    
    UserProfile profile = new UserProfile();
    profile.UserName = "John Doe";
    profile.Uid = "john.doe@gmail.com";
    profile.UtcTokenExpiryDate = DateTime.Now.AddDays(2).ToUniversalTime();
    var json = JsonConvert.SerializeObject(profile);
    
    SimulationFactory.When.I.Get().From("/oauth2/v1/validate")
        .With.Parameter("scope","userinfo.profile" ParameterType.UrlParameter)
        .With.Header("Authorization", string.Format("Bearer {0}", "abcd12345"))
        .Then.Return.StatusCode(System.Net.HttpStatusCode.OK).And.Body(json).With.Header("Content-Type", "application/json");
'''

# How To Use Moksy In Your Own Integration Tests:
-----------------------------------------------
Using Moksy within your tests is easy:

1. Add a reference to the Moksy NuGET Package: Install-Package Moksy

2. Create a Unit Test, create the Moksy instance and set up a simulation:

   Proxy = new Proxy(10011);
   Proxy.Start();

   // Remove any existing simulations on this endpoint.
   Proxy.DeleteAll();

   var simulation = SimulationFactory.When.I.Get().From("/TheEndpoint").Then.Return.Body("Hello World!").And.StatusCode(System.Net.HttpStatusCode.OK);
   proxy.Add(simulation);

   // Now navigate to http://localhost:10011/TheEndpoint in your browser or hit that Url from another service to receive "Hello World!"

Consider getting the source code and using the Moksy.IntegrationTests.DocumentationTests as your playpen; step through the tests in MsTest and follow the instructions :-) 


# Limitations:
------------
Many! 

Please see the Readme.txt file for a fuller list of features, limitations and use cases. 
