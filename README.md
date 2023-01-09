# Extensions for RichardSzalay.MockHttp

## Partial JSON matching

Match the provided properties from an object to the request intercepted by the MockHttp message handler.

* Additional properties on a JSON object request will be ignored
* Works with JSON objects or arrays as root
* Objects/values that are not a direct child of the root element will be compared using "deep equals"

### Examples

```csharp
// use RichardSzalay.MockHttp HttpMessageHandler
var handler = new MockHttpMessageHandler();
var mockedRequest = handler.When("/my/url");

// Single property object expected in the request.
mockedRequest.WithPartialJson(new { a = "1" });

// Multi-property object expected in the request
mockedRequest.WithPartialJson(new { a = "1", b = 2 });

// Array used as the root of the JSON
mockedRequest.WithPartialJson(new [] { 1, 2, 3 });
```

## Response helpers

These extension methods are available for shorter setup for responses.

### RespondWith

Add a response to the mocked request with a fluent interface. Sets up the response with HTTP 200 by default.

```csharp
// use RichardSzalay.MockHttp HttpMessageHandler
var handler = new MockHttpMessageHandler();
var mockedRequest = handler.When("/my/url");

// Respond with HTTP 401
mockedRequest.RespondWith(with => with.StatusCode(HttpStatusCode.Unauthorized));

// Include a JSON object serialized in the response (and HTTP 200 implicitly)
mockedRequest.RespondWith(with => with.JsonObject(new { foo = "bar" }));

// Include a JSON string in the response
mockedRequest.RespondWith(with => 
    with.StatusCode(HttpStatusCode.NotFound)
    .JsonString("""{"reason": "not found"}"""));
```

### RespondToAnyRequestWith

Add a request setup directly on an instance of `MockHttpMessageHandler` with a wildcard URL matcher to match any request and a fluent interface to setup the response (similar to `RespondWith`).

```csharp
var handler = new MockHttpMessageHandler();

// Respond to "*" with HTTP 401
handler.RespondToAnyRequestWith(with => with.StatusCode(HttpStatusCode.Unauthorized));

// Include a JSON object serialized in the response to "*" (and HTTP 200 implicitly)
handler.RespondToAnyRequestWith(with => with.JsonObject(new { foo = "bar" }));

// Include a JSON string in the response to "*"
handler.RespondToAnyRequestWith(with => 
    with.StatusCode(HttpStatusCode.NotFound)
    .JsonString("""{"reason": "not found"}"""));
```