using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SFC.Identity.Infrastructure.Persistence.Constants;

namespace SFC.Identity.Infrastructure.Persistence.Configurations.Identity;
public class IdentityUserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> builder)
    {
        builder.ToTable("UserLogins", DatabaseConstants.DefaultSchemaName);
    }
}
