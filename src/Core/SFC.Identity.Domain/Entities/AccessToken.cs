using SFC.Identity.Domain.Common;

namespace SFC.Identity.Domain.Entities;

public class AccessToken : BaseTokenEntity
{
    public RefreshToken RefreshToken { get; set; } = null!;
}

