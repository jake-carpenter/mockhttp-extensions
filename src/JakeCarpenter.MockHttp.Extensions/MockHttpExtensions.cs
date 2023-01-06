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
}