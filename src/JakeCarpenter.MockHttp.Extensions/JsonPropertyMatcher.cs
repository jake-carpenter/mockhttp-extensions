using System.Text.Json;
using RichardSzalay.MockHttp;

namespace JakeCarpenter.MockHttp.Extensions;

public class JsonPropertyMatcher : IMockedRequestMatcher
{
    private readonly JsonDocument _expected;

    public JsonPropertyMatcher(object value)
    {
        var json = JsonSerializer.Serialize(value);
        _expected = JsonDocument.Parse(json);
    }

    public bool Matches(HttpRequestMessage message)
    {
        var json = message.Content.ReadAsStringAsync().Result;
        var requestJsonDoc = JsonDocument.Parse(json);

        foreach (var expectedProperty in _expected.RootElement.EnumerateObject())
        {
            if (!requestJsonDoc.RootElement.TryGetProperty(expectedProperty.Name, out var matchedJsonElement))
                return false;

            if (matchedJsonElement.ValueKind != expectedProperty.Value.ValueKind)
                return false;

            if (!DoTheSwitch(matchedJsonElement, expectedProperty))
                return false;
        }

        return true;
    }

    private static bool DoTheSwitch(JsonElement requestElement, JsonProperty expectedProperty)
    {
        switch (expectedProperty.Value.ValueKind)
        {
            case JsonValueKind.String:
                return IsStringMatch(requestElement, expectedProperty);

            case JsonValueKind.Number:
                return IsNumericMatch(requestElement, expectedProperty);

            case JsonValueKind.True:
            case JsonValueKind.False:
                return IsBooleanMatch(requestElement, expectedProperty);

            case JsonValueKind.Null:
                return IsNullMatch(requestElement, expectedProperty);

            case JsonValueKind.Undefined:
            case JsonValueKind.Object:
            case JsonValueKind.Array:
            default:
                throw new ArgumentException($"Case has not been coded for: {expectedProperty.Value.ValueKind}");
        }
    }

    private static bool IsStringMatch(JsonElement requestElement, JsonProperty expectedProperty) =>
        requestElement.GetString() == expectedProperty.Value.GetString();

    private static bool IsNumericMatch(JsonElement requestElement, JsonProperty expectedProperty) =>
        requestElement.GetDecimal() == expectedProperty.Value.GetDecimal();

    private static bool IsBooleanMatch(JsonElement requestElement, JsonProperty expectedProperty) =>
        requestElement.GetBoolean() == expectedProperty.Value.GetBoolean();

    private static bool IsNullMatch(JsonElement requestElement, JsonProperty expectedProperty) =>
        requestElement.ValueKind == JsonValueKind.Null && requestElement.GetString() == null &&
        expectedProperty.Value.GetString() == null;
}