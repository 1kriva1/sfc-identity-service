using System.Text.Json.Serialization;

namespace SFC.Identity.Application.Common.Models
{
    [JsonDerivedType(typeof(BaseErrorResponse))]
    public class BaseResponse
    {
        public BaseResponse()
        {
            Success = true;
            Message = "Success result.";
        }
        public BaseResponse(string message)
        {
            Success = true;
            Message = message;
        }

        public BaseResponse(string message, bool success)
        {
            Success = success;
            Message = message;
        }

        [JsonPropertyOrder(0)]
        public bool Success { get; }

        [JsonPropertyOrder(1)]
        public string Message { get; }
    }
}
