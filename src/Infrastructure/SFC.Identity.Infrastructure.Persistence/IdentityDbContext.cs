using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Application.Models.Tokens;
using SFC.Identity.Infrastructure.Persistence.Models;
using SFC.Identity.Infrastructure.Persistence.Seeds.Users;

namespace SFC.Identity.Infrastructure.Persistence
{
    public class IdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

        public virtual DbSet<AccessToken> AccessTokens { get; set; }

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema(DbConstants.DEFAULT_SCHEMA_NAME);

            builder.Entity<AccessToken>(b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            builder.Entity<RefreshToken>(b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            builder.Entity<AccessToken>()
                .HasOne(b => b.RefreshToken)
                .WithOne(i => i.Token)
                .HasForeignKey<RefreshToken>(b => b.TokenForeignKey)
                .IsRequired();

            builder.Entity<AccessToken>()
               .Navigation(e => e.RefreshToken)
               .AutoInclude();

            builder.Entity<ApplicationUser>()
               .HasOne(b => b.AccessToken)
               .WithOne()
               .HasForeignKey<AccessToken>(b => b.UserForeignKey);

            builder.Entity<ApplicationUser>(b => b.ToTable("Users"));

            builder.Entity<ApplicationUser>()
                .Navigation(e => e.AccessToken)
                .AutoInclude();

            builder.Entity<IdentityUserClaim<Guid>>(b => b.ToTable("UserClaims"));

            builder.Entity<IdentityUserLogin<Guid>>(b => b.ToTable("UserLogins"));

            builder.Entity<IdentityUserToken<Guid>>(b => b.ToTable("UserTokens"));

            builder.Entity<ApplicationRole>(b => b.ToTable("Roles"));

            builder.Entity<IdentityRoleClaim<Guid>>(b => b.ToTable("RoleClaims"));

            builder.Entity<IdentityUserRole<Guid>>(b => b.ToTable("UserRoles"));

            //Seeding the User to Users table
            builder.SeedUsers();
        }
    }
}
