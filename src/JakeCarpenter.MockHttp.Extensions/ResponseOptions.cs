using System.Net;
using System.Text;
using System.Text.Json;
using RichardSzalay.MockHttp;

namespace JakeCarpenter.MockHttp.Extensions;

public interface IResponseOptions
{
    IResponseOptions StatusCode(HttpStatusCode statusCode);
    IResponseOptions JsonString(string responseJson);
    IResponseOptions JsonObject(object responseJson);
}

internal class ResponseOptions : IResponseOptions
{
    private readonly MockedRequest _request;
    private HttpStatusCode _status = HttpStatusCode.OK;
    private object? _responseJson;
    private string? _responseJsonString;

    public ResponseOptions(MockedRequest request)
    {
        _request = request;
    }

    public IResponseOptions StatusCode(HttpStatusCode status)
    {
        _status = status;
        return this;
    }

    public IResponseOptions JsonObject(object responseJson)
    {
        _responseJson = responseJson;
        return this;
    }

    public IResponseOptions JsonString(string responseJson)
    {
        _responseJsonString = responseJson;
        return this;
    }

    public void Build()
    {
        _request.Respond(
            _ =>
            {
                var response = new HttpResponseMessage(_status);
                var json = _responseJson is not null ? JsonSerializer.Serialize(_responseJson) : _responseJsonString;

                if (json is not null)
                {
                    response.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                return response;
            });
    }
}