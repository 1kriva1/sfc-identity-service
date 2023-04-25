using SFC.Identity.Application.Common.Constants;
using System.Text.Json.Serialization;

namespace SFC.Identity.Application.Common.Models
{
    [JsonDerivedType(typeof(BaseErrorResponse))]
    public class BaseResponse
    {        
        public BaseResponse()
        {
            Success = true;
            Message = CommonConstants.SUCCESS_MESSAGE;
        }
        public BaseResponse(string message)
        {
            Success = true;
            Message = message;
        }

        [JsonConstructor]
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
