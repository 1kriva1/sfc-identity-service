using SFC.Identity.Application.Common.Models;

namespace SFC.Identity.Application.Models.Registration
{
    public class RegistrationResponse : BaseResponse
    {
        public Guid UserId { get; set; }
    }
}
