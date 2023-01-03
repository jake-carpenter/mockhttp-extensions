using System.Text;
using Shouldly;

namespace JakeCarpenter.MockHttp.Extensions.Tests;

public class JsonPropertyMatcherTests
{
    [Theory(DisplayName = "False when the expected property does not exist in a single property request")]
    [InlineData("b")]
    [InlineData(1)]
    [InlineData(1.1)]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void SinglePropertyDoesNotExistNoMatch(object value)
    {
        var expected = new { a = value };
        const string json = """{"c":"d"}""";
        using var msg = CreateRequest(json);

        var result = GetSut(expected).Matches(msg);

        result.ShouldBeFalse();
    }

    [Theory(DisplayName = "False when the expected value does not match in a single property request")]
    [InlineData("b")]
    [InlineData(1)]
    [InlineData(1.1)]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void SingePropertyValueNoMatch(object value)
    {
        var expected = new { a = value };
        const string json = """{"a":"c"}""";
        using var msg = CreateRequest(json);

        var result = GetSut(expected).Matches(msg);

        result.ShouldBeFalse();
    }

    [Theory(DisplayName = "False when any expected property does not exist in a multi property request")]
    [InlineData("z")]
    [InlineData(1)]
    [InlineData(1.1)]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void MultiPropertyDoesNotExistNoMatch(object value)
    {
        var expected = new { a = "b", c = value };
        const string json = """{"a":"b"}""";
        using var msg = CreateRequest(json);

        var result = GetSut(expected).Matches(msg);

        result.ShouldBeFalse();
    }

    [Fact(DisplayName = "True when matching an empty object type")]
    public void TrueEmptyObject()
    {
        var expected = new { };
        const string json = "{}";
        using var msg = CreateRequest(json);

        var result = GetSut(expected).Matches(msg);

        result.ShouldBeTrue();
    }

    [Fact(DisplayName = "True when multiple properties that match are out of order")]
    public void TrueMultiPropertyOutOfOrder()
    {
        var expected = new { a = 1, b = "x", c = true }; // a, b, c
        const string json = """{"c":true,"a":1,"b":"x"}"""; // c, a, b
        using var msg = CreateRequest(json);

        var result = GetSut(expected).Matches(msg);

        result.ShouldBeTrue();
    }

    [Fact(DisplayName = "True when string type matches on a single property request")]
    public void TrueStringSingleProperty()
    {
        var expected = new { a = "b" };
        const string json = """{"a":"b"}""";
        using var msg = CreateRequest(json);

        var result = GetSut(expected).Matches(msg);

        result.ShouldBeTrue();
    }

    [Theory(DisplayName = "True when numeric type matches on a single property request")]
    [InlineData(1, """{"a":1}""")]
    [InlineData(1.9, """{"a":1.9}""")]
    public void TrueNumericSingleProperty(object value, string json)
    {
        var expected = new { a = value };
        using var msg = CreateRequest(json);

        var result = GetSut(expected).Matches(msg);

        result.ShouldBeTrue();
    }

    [Theory(DisplayName = "True when multiple numeric types match on a multi property request")]
    [InlineData(1, 2, """{"a":1,"b":2}""")]
    [InlineData(1.1, 2.2, """{"a":1.1,"b":2.2}""")]
    public void TrueNumericMultiProperty(object a, object b, string json)
    {
        var expected = new { a, b };
        using var msg = CreateRequest(json);

        var result = GetSut(expected).Matches(msg);

        result.ShouldBeTrue();
    }

    [Theory(DisplayName = "True when single boolean value matches on a single property request")]
    [InlineData(true, """{"a":true}""")]
    [InlineData(false, """{"a":false}""")]
    public void TrueBooleanSingleProperty(bool value, string json)
    {
        var expected = new { a = value };
        using var msg = CreateRequest(json);

        var result = GetSut(expected).Matches(msg);

        result.ShouldBeTrue();
    }

    [Theory(DisplayName = "True when multiple boolean value match on a multi property request")]
    [InlineData(true, true, """{"a":true,"b":true}""")]
    [InlineData(true, false, """{"a":true,"b":false}""")]
    [InlineData(false, true, """{"a":false,"b":true}""")]
    [InlineData(false, false, """{"a":false,"b":false}""")]
    public void TrueBooleanMultiProperty(bool a, bool b, string json)
    {
        var expected = new { a, b };
        using var msg = CreateRequest(json);

        var result = GetSut(expected).Matches(msg);

        result.ShouldBeTrue();
    }

    [Fact(DisplayName = "True when expected value is null on single property request")]
    public void TrueNullSingleProperty()
    {
        var expected = new { a = (string?)null };
        const string json = """{"a":null}""";
        using var msg = CreateRequest(json);

        var result = GetSut(expected).Matches(msg);

        result.ShouldBeTrue();
    }

    [Fact(DisplayName = "True when expected value is null on multi property request")]
    public void TrueNullMultiProperty()
    {
        var expected = new { a = (string?)null, b = (int?)null };
        const string json = """{"a":null,"b":null}""";
        using var msg = CreateRequest(json);

        var result = GetSut(expected).Matches(msg);

        result.ShouldBeTrue();
    }

    private static HttpRequestMessage CreateRequest(string json)
    {
        var msg = new HttpRequestMessage(HttpMethod.Post, "http://localhost");
        msg.Content = new StringContent(json, Encoding.UTF8, "application/json");
        return msg;
    }

    private JsonPropertyMatcher GetSut(object expected)
    {
        return new JsonPropertyMatcher(expected);
    }
}