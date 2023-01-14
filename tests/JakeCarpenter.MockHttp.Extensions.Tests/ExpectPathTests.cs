using System.Net;
using RichardSzalay.MockHttp;

namespace JakeCarpenter.MockHttp.Extensions.Tests;

public class ExpectPathTests
{
    [Fact(DisplayName = "Applies expectation for the mocked request")]
    public async Task Expectation()
    {
        var handler = new MockHttpMessageHandler();
        var client = handler.ToHttpClient();
        handler
            .ExpectPath(HttpMethod.Get, "/")
            .Respond(HttpStatusCode.OK);

        var message = new HttpRequestMessage(HttpMethod.Get, "https://arbitrary.com");
        await client.SendAsync(message);
        
        handler.VerifyNoOutstandingExpectation();
    }
    
    [Theory(DisplayName = "Applies expectation for the mocked request with provided HTTP method")]
    [InlineData("POST")]
    [InlineData("GET")]
    [InlineData("PUT")]
    [InlineData("DELETE")]
    [InlineData("PATCH")]
    public async Task Method(string method)
    {
        var httpMethod = new HttpMethod(method);
        var handler = new MockHttpMessageHandler();
        var client = handler.ToHttpClient();
        handler
            .ExpectPath(httpMethod, "/")
            .Respond(HttpStatusCode.OK);

        var message = new HttpRequestMessage(httpMethod, "https://arbitrary.com");
        await client.SendAsync(message);
        
        handler.VerifyNoOutstandingExpectation();
    }
    
    [Theory(DisplayName = "Applies expectation for the mocked request ignoring everything before the path")]
    [InlineData("/foo/bar")]
    [InlineData("/foo/bar/")]
    [InlineData("foo/bar")]
    [InlineData("foo/bar/")]
    [InlineData("/foo/bar?q=baz")]
    [InlineData("/foo/bar/?q=baz")]
    public async Task Path(string path)
    {
        var handler = new MockHttpMessageHandler();
        var client = handler.ToHttpClient();
        handler
            .ExpectPath(HttpMethod.Get, path)
            .Respond(HttpStatusCode.OK);

        var message = new HttpRequestMessage(HttpMethod.Get, $"https://arbitrary.com/{path}");
        await client.SendAsync(message);
        
        handler.VerifyNoOutstandingExpectation();
    }
}