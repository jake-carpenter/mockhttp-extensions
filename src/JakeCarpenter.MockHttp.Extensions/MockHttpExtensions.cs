using System.Text;
using System.Text.Json;
using RichardSzalay.MockHttp;

namespace JakeCarpenter.MockHttp.Extensions;

public static class MockHttpExtensions
{
    /// <summary>
    /// Match a JSON request body on the provided properties of an object or an array.
    /// Other properties of an object can exist in the request, but the provided properties must match. 
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="value">An object/dynamic value representing the desired body.</param>
    /// <example>
    /// <code>request.WithPartialJson(new { a = "1" });</code>
    /// <code>request.WithPartialJson(new { a = "1", b = 2 });</code>
    /// <code>request.WithPartialJson(new [] { 1, 2, 3 });</code>
    /// </example>
    public static MockedRequest WithPartialJson(this MockedRequest request, object value)
    {
        return request.With(new PartialJsonMatcher(value));
    }

    /// <summary>
    /// Use a thrown exception as a fallback for when an HTTP request is made
    /// which was not set up with the message handler. This fallback exception
    /// includes the request method and URL, as well as the JSON body if
    /// applicable. This overrides the default behavior of MockHttp returning
    /// an HTTP 404.
    /// </summary>
    public static void UseFallbackWithRequestJson(this MockHttpMessageHandler handler)
    {
        handler.Fallback.Respond(
            request =>
            {
                var reasonBuilder = new StringBuilder();
                reasonBuilder
                    .AppendLine("\n=========================")
                    .Append(
                        "A request was made to the following URL which did not match a request definition or expectation:\n")
                    .Append(request.Method)
                    .Append(' ')
                    .AppendLine(request.RequestUri?.AbsoluteUri);

                if (request.Content is { Headers.ContentType.MediaType: "application/json" })
                {
                    var content = request.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                    if (content.Length > 0)
                    {
                        var json = JsonDocument.Parse(content);
                        var prettyJson = JsonSerializer.Serialize(
                            json,
                            new JsonSerializerOptions { WriteIndented = true });
                        reasonBuilder.AppendLine("\nThe JSON body content was:").AppendLine(prettyJson).AppendLine();
                    }
                }

                reasonBuilder.AppendLine("=========================");

                throw new HttpRequestException(reasonBuilder.ToString());
            });
    }

    /// <summary>
    /// Allow a fluent interface for defining the response to an intercepted HTTP request.
    /// Builds a request that includes an HTTP status 200 by default.
    /// </summary>
    /// <param name="request">Instance of MockHttp <see cref="MockedRequest" /></param>
    /// <param name="setup">Lambda function allowing fluent access to response options.</param>
    public static void RespondWith(this MockedRequest request, Func<IResponseOptions, IResponseOptions> setup)
    {
        var options = setup(new ResponseOptions(request)) as ResponseOptions;
        options?.Build();
    }

    /// <summary>
    /// Allow a fluent interface for defining the response to an HTTP request with any URL.
    /// Builds a request that includes an HTTP status 200 and matches a URL of "*" by default.
    /// </summary>
    /// <param name="messageHandler">Instance of MockHttp <see cref="MockHttpMessageHandler" /></param>
    /// <param name="setup">Lambda function allowing fluent access to response options.</param>
    public static void RespondToAnyRequestWith(
        this MockHttpMessageHandler messageHandler,
        Func<IResponseOptions, IResponseOptions> setup)
    {
        var request = messageHandler.When("*");
        var options = setup(new ResponseOptions(request)) as ResponseOptions;
        options?.Build();
    }
}