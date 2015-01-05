MOKSY - V1.3.146 by Grey Ham (www.twitter.com/brek_it, www.brekit.com, www.havecomputerwillcode.com)
----------------------------------------------------------------------------------------------------

New Additions to 1.3.146:
-------------------------
    - Nested Imdb. ie: /Pet/{Kind}/Toy/{Name}/Supplier/{...} etc.
	- Start the Proxy with additional parameters (such as logging)
	- Improved matching of headers, body parameters and url paramters (partial matches; urlencoding)
	- Lots of internal refactoring to improve the IntelliSENSE experience. 
	- Various bug fixes


What is Moksy?
--------------
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

While setting up canned responses for GET, PUT, DELETE, PATCH, POST and so forth is useful, the real power of Moksy comes with the use of the Imdb (In-Memory Database) 
functions. By submitting Json, Moksy will create an in-memory store of your CRUD functions and allow you to get your API up and running very quickly. 

For example: assuming a Pet Json structure that has a single property 'Kind':

{
    "Kind" : "Dog"
}

Moksy will support Create, Read, Update and Delete functions (POST, GET, DELETE, PUT) on that end-point with these simulations and use Kind as the unique index:

		SimulationFactory.When.I.Post().ToImdb("/Pet").And.NotExists("Kind").Return.StatusCode(System.Net.HttpStatusCode.Created).And.AddToImdb();
		SimulationFactory.When.I.Post().ToImdb("/Pet").And.Exists("Kind").Return.StatusCode(System.Net.HttpStatusCode.BadRequest).And.Body("A Pet of that kind already exists");
		
		SimulationFactory.When.I.Get().FromImdb("/Pet").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body("[{value}]);
		SimulationFactory.When.I.Get().FromImdb("/Pet/{kind}").And.Exists("{kind}").Then.Return.StatusCode(System.Net.HttpStatusCode.OK);
		SimulationFactory.When.I.Get().FromImdb("/Pet/{kind}").And.NotExists("{kind}").Then.Return.StatusCode(System.Net.HttpStatusCode.NotFound);

		SimulationFactory.When.I.Delete().FromImdb("/Pet/{kind}").And.Exists("{kind}").Then.Return.StatusCode(System.Net.HttpStatusCode.NoContent).And.RemoveFromImdb();
		SimulationFactory.When.I.Delete().FromImdb("/Pet/{kind}").And.NotExists("{kind}").Then.Return.StatusCode(System.Net.HttpStatusCode.NoContent);

		SimulationFactory.When.I.Put().ToImdb("/Pet").And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.Created).And.AddToImdb().And.Body("{value}");
		SimulationFactory.When.I.Put().ToImdb("/Pet").And.Exists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.OK).And.AddToImdb().And.Body("{value}");

Posting the Json structure to http://localhost:10011/Pet will add it to the in memory database for that resource; calling GET on http://localhost:10011 will return a comma-separated
list of the Json entries wrapped in [] to simulate an array. Of course, calling GET on http://localhost:10011/Dog will return just the above structure. 

Moksy can be useful for:

- Api Shaping. 
- Isolating services from one another: it is often difficult to 'inject' responses and faults into services to see how they respond to errors and conditions. Moksy
  makes this easy providing you parameterize the end-points your other services hit. 
- Integration, Web Service and System Testing
- Faking, stubbing or mocking Web Services, REST API Services or third-party end-points you need to interact with that might be unreliable or under development.
- Removing service development from the critical path - just set up simulations in Moksy and have your testers and devs hit the end-points until the final service is delivered.
  Ideal for Api shaping - just change the expectation and response and iteratively refine your services. 



Simple Header / Url / Body Parameter Examples:
----------------------------------------------
To return a response based on the presence of some header. For example:

    SimulationFactory.When.I.Get().From("/Pet").With.Header("Name").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).And.Body("Yo!");

To return a response based on the presence of some header and a fixed value:

    SimulationFactory.When.I.Get().From("/Pet").With.Header("Name", "theValue").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).And.Body("Requires header with given value");

Sometimes you only want to check a part of the value of a header. Use the .PartialValue and (optionally) CaseInsensitive to choose the comparison you want to make:

    SimulationFactory.When.I.Get().From("/Pet").With.Header("Name", "alu", ComparisonType.PartialValue | ComparisonType.CaseInsensitive).Then.Return.StatusCode(System.Net.HttpStatusCode.OK).And.Body("Yo!");

The same principle applies for Url/Query parameters and Body Parameters (as would be POST'd from a Form, for example):

    SimulationFactory.When.I.Post().To("/Pet").With.Parameter("Name", "alu", ParameterType.BodyParameter, ComparisonType.PartialValue | ComparisonType.CaseInsensitive).Then.Return.StatusCode(System.Net.HttpStatusCode.OK).And.Body("Yo!");

Or perhaps with a URL Parameter - you would GET from /Pet?Name=theValue to get this to match for example:

	SimulationFactory.When.I.GET().From("/Pet").With.Parameter("Name", "alu", ParameterType.UrlParameter, ComparisonType.PartialValue | ComparisonType.CaseInsensitive).Then.Return.StatusCode(System.Net.HttpStatusCode.OK).And.Body("Yo!");



It is possible to reuse the Header, Query or Body Parameter values that were sent as part of the request in the Response Body or Headers themselves. For example:

    SimulationFactory.When.I.Get().From("/Pet").With.Header("Kind", "Dog").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).And.Body("{Request:Header:Kind}");

The same applies for Body Parameters and Query Parameters. For example:

	... .And.Body("{Request:BodyParameter:TheBodyParameter}")
	... .And.Body("{Request:QueryParameter:TheQueryParameter}")

Sometimes you also want the body and query parameters to be encoded again when returned. Use the :UrlEncoded: placeholders instead:

	... .And.Body("{Request:BodyParameter:UrlEncoded:TheBodyParameter}")
	... .And.Body("{Request:QueryParameter:UrlEncoded:TheQueryParameter}")



How To Use Moksy In Your Own Integration Tests:
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




Constrains and Violations:
--------------------------
Constraints and violations can be set as part of your simulations. This is useful for returning error conditions if, for example, property conditions and constraints are not met.
For example:

     When.I.Post().ToImdb("/Pet").With.Constraint(new LengthBetweenConstraint("Kind", 0, 255)).And.HasRuleViolations().Then.Return.StatusCode(System.Net.HttpStatusCode.BadRequest).And.Body("{violationResponses}");

Notice the use of {violationResponses} - this is an array of zero or more respones from the constraints that were not met. Although a default is provided, a default response format can be specified as part of the Constraint to return custom error messages. ie:

    var between = new LengthBetweenConstraint("Kind", 0, 255) { Response = @"{""ErrorType"":""OutOfRangeException"",""PropertyName"":{PropertyName},""AdditonalInformation"":""The 'Kind' property must be between 0 and 255 characters in length""}";
    When.I.Post().ToImdb("/Pet").With.Constraint(between).And.HasRuleViolations().Then.Return.StatusCode(System.Net.HttpStatusCode.BadRequest).And.Body("{violationResponses}");


LIMITATIONS:
------------
- Very many! This is still a work in progress. 
- For In-Memory Database, only POST, GET, PUT and DELETE are supported. PATCH support will be added in future. 
- HTTPS is not supported. 
- Multiple placeholders can be specified in a path. ie: /Pet/{Kind}/Toy{Name} but the Exists and NotExists can only specify the last variable in the list. ie: {Name} 
- Accessing resources for GET and DELETE can only be done through the URL. ie: GET /Pet('Dog') and not GET /Pet with 'Dog' in the Body or Header. 


Quick samples:
--------------
A simulation returns a canned response from a service when a HTTP Request with certain conditions are met. In particular the conditions are:

   The HTTP Method - GET, POST, DELETE, PUT, etc. 
   The resource path - either fixed such as /Pet or a placeholder such as /Pet/{Kind}
   Additional request headers and their values [optional]
   Whether an existing object exists (ToImdb() and FromImdb() only)

The response can contain the following information (all are optional). 

   Body content
   StatusCode
   Headers

A Fluent API is used to specify the Simulation - anything after the When but before the .Then or .Return. is for the condition; anything after .Then or .Return. refers to the response.

In its simplest, form, we might want to return a particular status code when an end-point is hit. For example:

    Moksy.Common.Proxy Proxy = new Moksy.Common.Proxy(10011);

    var s = Moksy.Common.SimulationFactory.When.I.Get().From("/TheEndpoint").Return.StatusCode(System.Net.HttpStatusCode.MultipleChoices);
	Proxy.Add(s);

Or perhaps we want to return some content as well (the content is usually Json or Xml):

    var s = Moksy.Common.SimulationFactory.When.I.Get().From("/TheEndpoint").Return.Body("My content").StatusCode(System.Net.HttpStatusCode.MultipleChoices);
	Proxy.Add(s);

Or perhaps we want to return some content only if a particular REQUEST header and header value is present:

    var s = Moksy.Common.SimulationFactory.When.I.Get().From("/TheEndpoint").With.Header("MyHeader", "MyValue").Return.Body("My content").StatusCode(System.Net.HttpStatusCode.MultipleChoices);
	Proxy.Add(s);

We can also return addition RESPONSE headers as well:

    var s = Moksy.Common.SimulationFactory.When.I.Get().From("/TheEndpoint").Return.Body("My content").StatusCode(System.Net.HttpStatusCode.MultipleChoices).Header("MyResponseHeader", "MyResponseHeaderValue");
	Proxy.Add(s);


Advanced Samples: [Moksy In-Memory Database]
--------------------------------------------
The problem: 
The structure of your API has been decided. You need to support CRUD functionality quickly so that your teams (UI, Test, whatever) can utilize
that API while you develop the real implementation. 

The solution:
Moksy provides a (very simple) In Memory Database for POST, GET and DELETE calls by providing uniqueness based on a property of that submitted Json. 

Use Moksy with the .ToImdb() and .FromImdb() calls. Moksy will create an in-memory database based on the Json you submit. For example:

    When.I.Post().ToImdb("/Pet").And.NotExists("{Kind}").Then.Return.StatusCode(System.Net.HttpStatusCode.Created).And.AddToImdb();
	When.I.Post().ToImdb("/Pet").And.Exists("{Kind}").Then.Return.StatusCode(System.Net.HttpStatusCode.BadRequest);

	When.I.Get().FromImdb("/Pet('{Kind}')").And.NotExists("{Kind}").Then.Return.StatusCode(System.Net.HttpStatusCode.NotFound);
	When.I.Get().FromImdb("/Pet('{Kind}')").And.Exists("{Kind}").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body("{value}");

	When.I.Delete().FromImdb("/Pet('{Kind}')").And.NotExists("{Kind}").Then.Return.StatusCode(System.Net.HttpStatusCode.NoContent);
	When.I.Delete().FromImdb("/Pet('{Kind}')").And.Exists("{Kind}").Then.Return.StatusCode(System.Net.HttpStatusCode.NoContent).And.RemoveFromImdb();

	When.I.Put().ToImdb("/Pet").And.NotExists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.Created).And.AddToImdb().And.Body("{value}");
	When.I.Put().ToImdb("/Pet").And.Exists("{Kind}").Return.StatusCode(System.Net.HttpStatusCode.OK).And.AddToImdb().And.Body("{value}");

If I now submit the following string to /Pet:

   { "Kind" : "Dog" }

It will be added to the in-memory database by Moksy and .Created will be returned. If I submit it a second time, .BadRequest will be returned. 

Obviously, building up test data is part of the problem - you need to be able to retrieve it! The two Get statements intuitively indicate what to do if an object exists with that key or not. 

Similarly with Delete. 

PATCH is currently not supported for the Imdb. 

To clear an Imdb but not to remove the simulation itself, call:

    DELETE /resource

With a header of:

	moksy-retainsimulation: true
	moksy-purgedata: true



Nested Imdb:
------------
Moksy now supports "nested" Imdb's - with a few limitations. For example:

    When.I.Post().ToImdb("/Pet/{Kind}/Toy").And.NotExists("{Name}").Then.Return.StatusCode(System.Net.HttpStatusCode.Created).And.AddToImdb();
	When.I.Post().ToImdb("/Pet/{Kind}/Toy").And.Exists("{Name}").Then.Return.StatusCode(System.Net.HttpStatusCode.BadRequest);

	When.I.Get().FromImdb("/Pet('{Kind}')/Toy").And.NotExists().Then.Return.StatusCode(System.Net.HttpStatusCode.NotFound);
	When.I.Get().FromImdb("/Pet('{Kind}')/Toy").And.Exists().Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body("[{value}]");

	When.I.Delete().FromImdb("/Pet('{Kind}/Toy/{Name}')").And.NotExists("{Name}").Then.Return.StatusCode(System.Net.HttpStatusCode.NoContent);
	When.I.Delete().FromImdb("/Pet('{Kind}/Toy/{Name}')").And.Exists("{Name}").Then.Return.StatusCode(System.Net.HttpStatusCode.NoContent).And.RemoveFromImdb();

	When.I.Put().ToImdb("/Pet('{Kind}/Toy/{Name}')").And.NotExists("{Name}").Return.StatusCode(System.Net.HttpStatusCode.Created).And.AddToImdb().And.Body("{value}");
	When.I.Put().ToImdb("/Pet('{Kind}/Toy/{Name}')").And.Exists("{Name}").Return.StatusCode(System.Net.HttpStatusCode.OK).And.AddToImdb().And.Body("{value}");    

Exists() and NotExists() will only validate against the 'last' variable in the path. 


Convention:
If a resource "in the path" does not exist, then it will be created automatically. There are limitations to this in that specific variables cannot be automatically created for
intermediate resources. 



Posting using Body Parameters:
------------------------------
By default, the following statement:

	When.I.Post().ToImdb("/Pet").And.Exists("Kind").Then.Return.StatusCode(System.Net.HttpStatusCode.BadRequest);

Will assume that the body content is send through as Json:

	When.I.Post().ToImdb("/Pet").AsJson().And.Exists("Kind").Then.Return.StatusCode(System.Net.HttpStatusCode.BadRequest);

However, it is now possible to specify BodyParameters as an option:

	When.I.Post().ToImdb("/Pet").AsBodyParameters().And.Exists("Kind").Then.Return.StatusCode(System.Net.HttpStatusCode.BadRequest);

That allows you to create in-memory databases when developing simple Forms applications by Posting to (for example) /Pet. 

Typically, although you would want to Post() using Body Parameters it is likely you would want to retrieve using Json. Therefore, there are three values that can be specified in the .Body() response.
The default {value} is always Json:

	When.I.Get().FromImdb("/Pet('{Kind}')").AsJson().And.Exists("{Kind}").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body("{value}");

The other values are:

	.Body("{valueAsJson"}");
	.Body("{valueAsBodyParameters"}");				[the values will be encoded safely so they can be reissued immediately]
	.Body("{valueAsBodyParametersNotEncoded"}");	[the values are not encoded]
	


Returning Binary Content:
-------------------------
To simplify creating interactive, mocked up forms with images and Imdbs, it is possible to return binary content. For example:

	var bytes = System.IO.File.ReadAllBytes("the.Png");

    var s = Moksy.Common.SimulationFactory.When.I.Get().From("/Pet/the.png").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body(bytes).And.Header("Content-Type", "image/png");
    Proxy.Add(s);

	var response = Get("/Pet/the.png");
	// response.RawBytes will contain the image information. 

Navigating to http://localhost:10011/Pet/the.png from a browser will display the image. 



File / Binary Content:
----------------------
Files can also be stored in their own Imdb. Use the .AsBinary() type for the Imdb. You will need to return the Location of the resource in the header (for example) - all created
files have a {BinaryContentIdentity} variable created that you can set up as part of the response. 

	var s = SimulationFactory.When.I.Post().ToImdb("/Storage").AsBinary().Then.AddToImdb().And.Return.StatusCode(System.Net.HttpStatusCode.Created).With.Header("Location", "{requestroot}/Storage/{BinaryContentIdentity}");
	Proxy.Add(s);

	s = SimulationFactory.When.I.Get().FromImdb("/Storage/{BinaryContentIdentity}").AsBinary().And.Exists("{BinaryContentIdentity}").Then.Return.StatusCode(System.Net.HttpStatusCode.OK);
	Proxy.Add(s);

Files must be posted as Multipart. Please see the 'FileTests' under 'Moksy.IntegrationTests' for examples on how to manage file storage. GET, POST, DELETE, PUT are supported. 



Grouping By Header:
-------------------
The Problem:
Storing data in an Imdb and retrieving it is convenient but quite often an additional discriminator - such as the header - is used to logically group the data. 
Moksy supports using a named header as a grouping mechanism for data. For example: if we use a header called "Owner":

    When.I.Post().ToImdb("/Pet", "Owner").And.NotExists("Kind").Then.Return.StatusCode(System.Net.HttpStatusCode.Created).And.AddToImdb();
	When.I.Post().ToImdb("/Pet", "Owner").And.Exists("Kind").Then.Return.StatusCode(System.Net.HttpStatusCode.BadRequest);

All GET, PUT, DELETE and POST operations now group operations based on the value of the header. 



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

	{Request:Url:Scheme}		- ie: http								NOTE:	{requestscheme still works but has been superseded}
	{Request:Url:Host}			- ie: localhost									{requesthost still works but has been superseded}
	{Request:Url:Port}			- ie: 10011										{requestport still works but has been superseded}
	{Request:Url:Root}			- ie: http://localhost:10011					{requestroot still works but has been superseded}

This information can be used (for example) in 'inject' the Location of an object in a Response header:

	When.I.Post().ToImdb("/Pet").And.NotExists("Kind").Then.Return.StatusCode(System.Net.HttpStatusCode.Created).With.Variable("Identity").OverrideProperty("Id", "{Identity}").AddToImdb().Body("{Identity}").With.Header("Location", "{uriroot}/Pet/{Identity}");



To Set Up:
----------
In all cases, Moksy.Host.Exe must be running and set up to run at a particular part (ie: 10011). In order to programmatically set up the service for your simulations,
you use the Moksy.Common.Proxy class in your tests or application to 'bind' to the running Moksy host. You then add Simulations to Moksy via that Proxy and hit 
the Moksy endpoint. ie: [see Moksy.IntegrationTests.DocumentationTests for more information]

	Proxy proxy = new Proxy(10011);

    var s = Moksy.Common.SimulationFactory.When.I.Get().From("/TheEndpoint").Then.Return.StatusCode(System.Net.HttpStatusCode.OK).With.Body("Hello World!");
	proxy.Add(s);

	// You now call /TheEndpoint directly from a Browser; your own test; or typically call another service which calls the Endpoint with the response set up
	// by yourself. 



Misc:
-----
Logging:
Moksy can be started from the command line with an optional /log parameter - this will output all requests to the console.

Load Simulations from Disk:
Although discouraged because the Json format might change in future, it is possible to specify the path of a Json file containing simulations:

    /File:<path>



Repeatability:
--------------
We can also specify how many times we want the condition to evaluate. This is not applied to Imdb calls.

For example: if we want /Something to return once and only once:

var s = Moksy.Common.SimulationFactory.When.Get().From("/Something").Once().Return.Body("Hello!");

The default is 'Forever'. When a rule has been called the number of times specified, it is removed and will no longer be evaluated unless it is re-added.



NOTES: 
------
If there is more than one match on an operation, the first one wins. This is deliberate - the order in which you add simulations is the order in which they are executed. 
Specify the most specific conditions first; the most generic last. 
Lots of words like When, I, With, Use, And are optional. Only methods are mandatory. 



KNOWN ISSUES:
-------------
See the DocumentationTests.cs for a thorough end-to-end sample of how to use Moksy
