Moksy v0.1 by Grey Ham
----------------------
An open source library for stubbing, faking, mocking and simulating Web Service calls.

See www.twitter.com/brek_it, www.havecomputerwillcode.com and www.brekit.com for more information. 


Why?
----
As a technical tester, I often need to 'fake' services that are incomplete, unreliable or unavailable for testing by third parties. I have typically done this by writing simple Http applications that return canned responses.

Moksy tries to make this easier by providing a Fluent API around 'Simulations' on how you expect a service to work. The assumption is (for now) that these calls work from C# within MsTest.

A Simulation consists of a Condition and a Response. In its simplest use case, a Condition includes a resource such as /Pet. For example:

    Moksy.Common.SimulationFactory.When.I.Get.From("/Pet").Then.Return.Body("Hello World!").With.StatusCode(System.Net.HttpStatusCode.OK);

When we navigate to http://localhost:10011/Pet in a browser [or whichever port Moksy is running on] we will get back 'Hello World!'. Of course, the real motivation is to call other services which then hit /Pet!


Imdb:
-----
Simple canned responses for common verbs - POST, PATCH, DELETE and so forth - like above are ideal in many cases. However, there is a very common use case when developing REST API's using the Micro/Macro service pattern: we often want to support Create, Read and Delete functionality to help UI devs and testers get up and running quickly. 

Moksy does this for your automatically by creating an 'In Memory Database' for Json submitted to that resource. A property in the Json is specified as the 'Index' for uniqueness (optional). For example:

		SimulationFactory.When.I.Post().ToImdb("/Pet").And.NotExists("Kind").Return.StatusCode(System.Net.HttpStatusCode.Created).And.AddToImdb();
		SimulationFactory.When.I.Post().ToImdb("/Pet").And.Exists("Kind").Return.StatusCode(System.Net.HttpStatusCode.BadRequest).And.Body("A Pet of that kind already exists");
		
		SimulationFactory.When.I.Get().FromImdb("/Pet").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body("[{value}]);
		SimulationFactory.When.I.Get().FromImdb("/Pet/{kind}").And.Exists().Then.Return.StatusCode(System.Net.HttpStatusCode.OK);
		SimulationFactory.When.I.Get().FromImdb("/Pet/{kind}").And.NotExists().Then.Return.StatusCode(System.Net.HttpStatusCode.NotFound);

		SimulationFactory.When.I.Delete().FromImdb("/Pet/{kind}").And.Exists().Then.Return.StatusCode(System.Net.HttpStatusCode.NoContent).And.RemoveFromImdb();
		SimulationFactory.When.I.Delete().FromImdb("/Pet/{kind}").And.NotExists().Then.Return.StatusCode(System.Net.HttpStatusCode.NoContent);
		
Posting the Json structure to http://localhost:10011/Pet will add it to the in memory database; calling GET on http://localhost:10011 will return a comma-separated
list of the Json entries wrapped in [] to simulate an array. Of course, calling GET on http://localhost:10011/Dog will return just the above structure. 



To use for your Integration Tests:
----------------------------------
Read the Readme.txt to get started (after you have Cloned this Git repository!). 

There is currently no NuGET library so you will need to clone and build the solution. Reference Moksy.Common to use the Proxy class. This will be streamlined in future versions.



Limitations:
------------
Many! This is literally v0.1 and does the bare minimum. And the code is a bit of mess - it was cobbled together from an old application but to use the WebApi SelfHost libraries.

However, it is usable - I am using it :-)

See the readme.txt for a key list of limitations. 


Coming next:
------------
The key features I will add shortly are:

1. Specify parameters as part of the URL or as part of the Body.
2. Turn Swagger specifications into services automatically.
3. "Dynamic" properties for calculating properties, headers and so forth. 
4. Leverage header values for Imdb filtering and grouping. 
