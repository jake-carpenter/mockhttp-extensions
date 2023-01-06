using System.Text.Json;
using System.Text.Json.JsonDiffPatch;
using RichardSzalay.MockHttp;
using System.Text.Json.Nodes;

namespace JakeCarpenter.MockHttp.Extensions;

public class JsonPropertyMatcher : IMockedRequestMatcher
{
    private readonly JsonNode? _expectedNode;

    private static readonly JsonNodeOptions NodeOptions = new() { PropertyNameCaseInsensitive = true };

    public JsonPropertyMatcher(object value)
    {
        _expectedNode = JsonSerializer.SerializeToNode(value);
    }

    public bool Matches(HttpRequestMessage message)
    {
        if (message.Content is null)
            return false;

        var json = message.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        var requestNode = JsonNode.Parse(json, NodeOptions);

        return (requestNode, _expectedNode) switch
        {
            (JsonObject req, JsonObject exp) => DoesObjectContainExpected(req, exp),
            (JsonArray req, JsonArray exp) => DoesArrayContainExpected(req, exp),
            _ => false
        };
    }

    private static bool DoesArrayContainExpected(JsonNode request, JsonNode expected)
    {
        return expected.DeepEquals(request, JsonElementComparison.Semantic);
    }

    private static bool DoesObjectContainExpected(JsonObject request, JsonObject expected)
    {
        foreach (var (key, value) in expected)
        {
            if (!request.TryGetPropertyValue(key, out var matchedValue))
                return false;

            if (!JsonSerializer.SerializeToNode(matchedValue).DeepEquals(value, JsonElementComparison.Semantic))
                return false;
        }

        return true;
    }
}