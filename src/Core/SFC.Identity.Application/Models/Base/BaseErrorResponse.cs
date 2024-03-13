using System.Text.Json.Serialization;

namespace SFC.Identity.Application.Models.Base;

public class BaseErrorResponse : BaseResponse
{
    public BaseErrorResponse() { }

    [JsonConstructor]
    public BaseErrorResponse(string message, Dictionary<string, IEnumerable<string>> errors) : base(message, false)
    {
        Errors = errors;
    }

    [JsonPropertyOrder(2)]
    public Dictionary<string, IEnumerable<string>>? Errors { get; }
}
