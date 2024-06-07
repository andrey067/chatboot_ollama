using System.Text.Json.Serialization;

namespace Chatbot.Shared.Requests;

public class BaseResponse<TData>
{
    private int _code = Configuration.DefaultStatusCode;

    public BaseResponse(TData? data, int code = Configuration.DefaultStatusCode, string? message = null)
    {
        _code = code;
        Message = message;
        Data = data;
    }

    public string Message { get; set; }

    public TData? Data { get; set; }

    [JsonIgnore]
    public bool IsSuccess => _code is >= 200 and <= 299;
}
