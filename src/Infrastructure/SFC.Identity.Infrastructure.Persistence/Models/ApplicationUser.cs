using Microsoft.AspNetCore.Identity;
using SFC.Identity.Application.Models.Tokens;
using SFC.Identity.Domain.Entities;

namespace SFC.Identity.Infrastructure.Persistence.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public AccessToken? AccessToken { get; set; } = null!;
}
