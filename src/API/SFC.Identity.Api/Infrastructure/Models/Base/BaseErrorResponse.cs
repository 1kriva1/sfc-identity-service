using System.Text.Json.Serialization;

namespace SFC.Identity.Api.Infrastructure.Models.Base;

/// <summary>
/// **Base** response model with **errors**.
/// </summary>
public class BaseErrorResponse : BaseResponse
{
    public BaseErrorResponse() { }

    [JsonConstructor]
    public BaseErrorResponse(string message, Dictionary<string, IEnumerable<string>> errors) : base(message, false)
    {
        Errors = errors;
    }

    /// <summary>
    /// Response result errors in key/value representation.
    /// </summary>
    [JsonPropertyOrder(2)]
    public Dictionary<string, IEnumerable<string>>? Errors { get; }
}
