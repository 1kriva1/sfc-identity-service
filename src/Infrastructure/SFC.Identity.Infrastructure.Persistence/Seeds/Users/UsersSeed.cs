using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SFC.Identity.Infrastructure.Persistence.Models;

namespace SFC.Identity.Infrastructure.Persistence.Seeds.Users
{
    public static class UsersSeed
    {
        public static void SeedUsers(this ModelBuilder builder)
        {
            Guid userId = Guid.Parse("{B0788D2F-8003-43C1-92A4-EDC76A7C5DDE}");

            builder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = userId,
                    UserName = "TestUser",
                    NormalizedUserName = "TESTUSER",
                    Email = "testemail@mail.com",
                    NormalizedEmail = "TESTEMAIL@MAIL.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    LockoutEnabled = true,
                    PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null!, "Test1234!")
                }
            );
        }
    }
}
