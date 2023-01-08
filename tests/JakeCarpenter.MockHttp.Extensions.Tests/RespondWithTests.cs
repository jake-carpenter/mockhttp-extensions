using System.Net;
using RichardSzalay.MockHttp;
using Shouldly;

namespace JakeCarpenter.MockHttp.Extensions.Tests;

public class RespondWithTests
{
    [Theory(DisplayName = "Request returns provided HTTP status code")]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task HttpResponse(HttpStatusCode status)
    {
        var handler = new MockHttpMessageHandler();
        var client = handler.ToHttpClient();
        handler
            .When("*")
            .RespondWith(with => with.StatusCode(status));

        var request = new HttpRequestMessage(HttpMethod.Get, "https://arbitrary.com");
        var result = await client.SendAsync(request);

        result.StatusCode.ShouldBe(status);
    }

    [Fact(DisplayName = "Request returns provided HTTP status code")]
    public async Task HttpResponse200Default()
    {
        var handler = new MockHttpMessageHandler();
        var client = handler.ToHttpClient();
        handler
            .When("*")
            .RespondWith(with => with);

        var request = new HttpRequestMessage(HttpMethod.Get, "https://arbitrary.com");
        var result = await client.SendAsync(request);

        result.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "Request returns provided HTTP JSON object response")]
    public async Task JsonBody()
    {
        var handler = new MockHttpMessageHandler();
        var client = handler.ToHttpClient();
        handler
            .When("*")
            .RespondWith(with => with.JsonObject(new { foo = "bar" }));

        var request = new HttpRequestMessage(HttpMethod.Post, "https://arbitrary.com");
        var result = await client.SendAsync(request);

        var json = await result.Content.ReadAsStringAsync();
        json.ShouldBe("""{"foo":"bar"}""");
    }

    [Fact(DisplayName = "Request returns provided HTTP JSON string response")]
    public async Task JsonString()
    {
        const string expectedJson = """{"foo":"bar"}""";
        var handler = new MockHttpMessageHandler();
        var client = handler.ToHttpClient();
        handler
            .When("*")
            .RespondWith(with => with.JsonString(expectedJson));

        var request = new HttpRequestMessage(HttpMethod.Post, "https://arbitrary.com");
        var result = await client.SendAsync(request);

        var json = await result.Content.ReadAsStringAsync();
        json.ShouldBe(json);
    }
}