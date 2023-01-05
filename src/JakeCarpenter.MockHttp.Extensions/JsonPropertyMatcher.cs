using System.Text.Json;
using System.Text.Json.JsonDiffPatch;
using RichardSzalay.MockHttp;
using System.Text.Json.Nodes;

namespace JakeCarpenter.MockHttp.Extensions;

public class JsonPropertyMatcher : IMockedRequestMatcher
{
    private readonly JsonNode? _expectedNode;

    public JsonPropertyMatcher(object value)
    {
        var json = JsonSerializer.Serialize(value);
        _expectedNode = JsonNode.Parse(json);
    }

    public bool Matches(HttpRequestMessage message)
    {
        if (message.Content is null)
            return false;

        var json = message.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        var requestElement = JsonDocument.Parse(json).RootElement;

        return (_expectedNode, requestElement.ValueKind) switch
        {
            (JsonObject expected, JsonValueKind.Object) => DoesObjectContainExpected(requestElement, expected),
            (JsonArray expected, JsonValueKind.Array) => DoesArrayContainExpected(requestElement, expected),
            _ => false
        };
    }

    private static bool DoesArrayContainExpected(JsonElement requestElement, JsonArray expected)
    {
        var requestedNode = JsonSerializer.SerializeToNode(requestElement);
        return expected.DeepEquals(requestedNode);
    }

    private static bool DoesObjectContainExpected(JsonElement requestElement, JsonObject expected)
    {
        foreach (var (key, value) in expected)
        {
            if (!requestElement.TryGetProperty(key, out var matchedValue))
                return false;

            if (!JsonSerializer.SerializeToNode(matchedValue).DeepEquals(value))
                return false;
        }

        return true;
    }
}