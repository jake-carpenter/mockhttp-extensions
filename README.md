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
