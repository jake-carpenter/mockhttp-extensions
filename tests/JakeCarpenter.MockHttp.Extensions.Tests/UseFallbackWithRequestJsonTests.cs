using System.Text;
using RichardSzalay.MockHttp;
using Shouldly;

namespace JakeCarpenter.MockHttp.Extensions.Tests;

[UsesVerify]
public class UseFallbackWithRequestJsonTests
{
    [Fact(DisplayName = "Non-matching request provides the request type in the exception message (Test 1)")]
    public async Task Test1()
    {
        const string json = """{"foo":"bar","arr":[1,2,3],"obj":{"foo":"bar"}}""";
        var client = HttpClient();
        var msg = BuildRequest(HttpMethod.Put, "https://google.com/foo/bar", json);
        var exception = await ActAndGetException(client, msg);

        await Verify(exception.Message);
    }
    
    [Fact(DisplayName = "Non-matching request provides the request type in the exception message (Test 2)")]
    public async Task Test2()
    {
        const string json =
            """[{"_id":"63ba503231cd45efdb78fe0c","index":0,"guid":"90f2830d-1cd3-45a3-b8ee-0d5100d8c244","isActive":false,"balance":"$2,662.51","picture":"http://placehold.it/32x32","age":31,"eyeColor":"brown","name":"Robbins Cabrera","gender":"male","company":"EXOVENT","email":"robbinscabrera@exovent.com","phone":"+1 (932) 559-3591","address":"969 Catherine Street, Kenwood, Ohio, 3726","about":"Occaecat labore cupidatat commodo mollit commodo id consectetur qui magna ex laboris Lorem. Aliqua non minim nulla non cupidatat. Nisi sit proident amet ullamco amet aliquip culpa commodo eu veniam sunt eiusmod dolore.\r\n","registered":"2015-04-17T06:03:41 +06:00","latitude":-33.656527,"longitude":78.780547,"tags":["exercitation","labore","velit","fugiat","aliquip","eu","do"],"friends":[{"id":0,"name":"Rachel Burt"},{"id":1,"name":"Kristine Crawford"},{"id":2,"name":"Knapp Carrillo"}],"greeting":"Hello, Robbins Cabrera! You have 6 unread messages.","favoriteFruit":"banana"}]""";
        var client = HttpClient();
        var msg = BuildRequest(HttpMethod.Post, "https://my.url/api/resource/id", json);
        var exception = await ActAndGetException(client, msg);

        await Verify(exception.Message);
    }

    private static async Task<Exception> ActAndGetException(HttpClient client, HttpRequestMessage msg)
    {
        var act = () => client.SendAsync(msg);
        var exception = await act.ShouldThrowAsync<Exception>();
        exception.ShouldNotBeNull();
        return exception;
    }

    private static HttpRequestMessage BuildRequest(HttpMethod method, string url, string json)
    {
        var msg = new HttpRequestMessage(method, url);
        msg.Content = new StringContent(json, Encoding.UTF8, "application/json");
        return msg;
    }

    private static HttpClient HttpClient()
    {
        var handler = new MockHttpMessageHandler();
        handler.UseFallbackWithRequestJson();
        var client = handler.ToHttpClient();
        return client;
    }
}