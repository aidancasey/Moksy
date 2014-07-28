Moksy v0.2
----------
An open source library for stubbing, faking, mocking and simulating Web Service calls.

See www.twitter.com/brek_it, www.havecomputerwillcode.com and www.brekit.com for more information. 


Why?
----
For test automation, I write my integration tests against REST API Services using a combination of RestSharp and Json.NET. However, I often end up calling third party services - either directly or indirectly - through those API Calls. These services might be incomplete, unreliable or unavailable for testing. I have typically got around this problem by writing simple Http services that return canned responses so that I can continue testing.

Moksy tries to make this easier by providing a Fluent API around 'Simulations' on how a service should work. As part of your Integration test, you set up a Simulation for a particular endpoint and then - either directly or indirectly through another service call - hit the endpoint specified in that Simulation. The simulation is invoked and a predictable response is returned.

A Simulation consists of a Condition and a Response. In its simplest use case, a Condition includes a resource such as /Pet. For example:

    Moksy.Common.SimulationFactory.When.I.Get.From("/Pet").Then.Return.Body("Hello World!").With.StatusCode(System.Net.HttpStatusCode.OK);

When we navigate to http://localhost:10011/Pet in a browser [or whichever port Moksy is running on] we will get back 'Hello World!'. Of course, the real motivation is to call other services which then hit /Pet!


In-Memory Database:
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


More Advanced Samples (Variables and Properties)
------------------------------------------------
The Problem: [the application is currently limited to GUIDS]
When we add an object to the Imdb, we sometimes need to give it an identity. This typically a GUID or some hash but is always opaque. As a client, we cannot provide this identity because it is server calculated. 

Moksy can support this by creating a dynamic Variable in the Response and then using the OverrideProperty method to assign that Variable to the value of a Property in the Json. 
For example:

	When.I.Post().ToImdb("/Pet").And.NotExists("Kind").Then.Return.StatusCode(System.Net.HttpStatusCode.Created).With.Variable("Identity").OverrideProperty("Id", "{Identity}").AddToImdb().Body("{Identity}");

That will set "Id" to a new Guid (the Variable() method will calculate a new Guid on every simulation assuming no hard coded value is provided). Id is then a property of the submitted / stored Json object.

With that in mind, we can 'round trip' by returning the object using that Identity:

	When.I.Get().FromImdb("/Pet/{Id}").And.Exists("Id").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).And.Body("{value}");

Variables are referenced in the Body using {...} notation. There are four variables that are always present to every simulation:

	{requestscheme}			- ie: http
	{requesthost}			- ie: localhost
	{requestport}			- ie: 10011
	{requestroot}			- ie: http://localhost:10011

This information can be used (for example) in 'inject' the Location of an object in a Response header:

	When.I.Post().ToImdb("/Pet").And.NotExists("Kind").Then.Return.StatusCode(System.Net.HttpStatusCode.Created).With.Variable("Identity").OverrideProperty("Id", "{Identity}").AddToImdb().Body("{Identity}").With.Header("Location", "{uriroot}/Pet/{Identity}");



To use for your Integration Tests:
----------------------------------
Read the Readme.txt to get started (after you have Cloned this Git repository!). 

There is currently no NuGET library so you will need to clone and build the solution. Reference Moksy.Common to use the Proxy class. This will be streamlined in future versions.



Limitations:
------------
Many! This is literally v0.1 and does the bare minimum. And the code is a bit of mess - it was cobbled together from an old application but to use the WebApi SelfHost libraries.

However, it is usable - I am using it :-)

See the readme.txt for a key list of limitations. 
