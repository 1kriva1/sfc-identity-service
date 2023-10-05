using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SFC.Identity.Application.Models.Tokens;
using SFC.Identity.Infrastructure.Persistence.Models;

namespace SFC.Identity.Infrastructure.Persistence.Configurations;
public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasOne(b => b.AccessToken)
           .WithOne()
           .HasForeignKey<AccessToken>(b => b.UserForeignKey);

        builder.Navigation(e => e.AccessToken)
            .AutoInclude();

        builder.ToTable("Users");
    }
}
