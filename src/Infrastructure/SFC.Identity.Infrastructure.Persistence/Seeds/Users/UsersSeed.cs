using Microsoft.AspNetCore.Identity;

using SFC.Identity.Infrastructure.Persistence.Models;

namespace SFC.Identity.Infrastructure.Persistence.Seeds.Users;

public static class UsersSeed
{
    private const string TEST_PASSWORD = "Test1234!";

    public static async Task SeedUsers(this IdentityDbContext context)
    {
        ApplicationUser withoutProfileUser = new()
        {
            Id = Guid.Parse("{B0788D2F-8003-43C1-92A4-EDC76A7C5DDE}"),
            UserName = "TestUser",
            NormalizedUserName = "TESTUSER",
            Email = "testemail@mail.com",
            NormalizedEmail = "TESTEMAIL@MAIL.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            LockoutEnabled = true,
            PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null!, TEST_PASSWORD)
        };

        ApplicationUser withProfileUser = new()
        {
            Id = Guid.Parse("{836A7FB9-73FE-4107-91F2-4D916158E242}"),
            UserName = "TestProfileUser",
            NormalizedUserName = "TESTPROFILEUSER",
            Email = "testprofileemail@mail.com",
            NormalizedEmail = "TESTPROFILEEMAIL@MAIL.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            LockoutEnabled = true,
            PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null!, TEST_PASSWORD)
        };

        await context.Users.AddRangeAsync(new List<ApplicationUser> { withoutProfileUser, withProfileUser });
        await context.SaveChangesAsync();
    }
}
