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
    /// 
    /// </summary>
    /// <param name="handler"></param>
    /// <exception cref="HttpRequestException"></exception>
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

    public static void RespondWith(this MockedRequest request, Func<IResponseOptions, IResponseOptions> setup)
    {
        var options = setup(new ResponseOptions(request)) as ResponseOptions;
        options?.Build();
    }

    public static void RespondToAnyRequestWith(
        this MockHttpMessageHandler messageHandler,
        Func<IResponseOptions, IResponseOptions> setup)
    {
        var request = messageHandler.When("*");
        var options = setup(new ResponseOptions(request)) as ResponseOptions;
        options?.Build();
    }
}