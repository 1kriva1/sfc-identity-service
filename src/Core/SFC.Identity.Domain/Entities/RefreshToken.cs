using SFC.Identity.Domain.Common;

namespace SFC.Identity.Domain.Entities;
public class RefreshToken : BaseTokenEntity
{
    public AccessToken AccessToken { get; set; } = null!;
}
