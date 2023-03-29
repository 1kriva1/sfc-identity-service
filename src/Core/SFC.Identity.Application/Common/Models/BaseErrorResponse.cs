using System.Text.Json.Serialization;

namespace SFC.Identity.Application.Common.Models
{
    public class BaseErrorResponse : BaseResponse
    {
        public BaseErrorResponse() { }

        public BaseErrorResponse(string message, Dictionary<string, IEnumerable<string>> errors) : base(message, false)
        {
            Errors = errors;
        }

        [JsonPropertyOrder(2)]
        public Dictionary<string, IEnumerable<string>>? Errors { get; }
    }
}
