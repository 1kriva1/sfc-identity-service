using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SFC.Identity.Infrastructure.Persistence.Constants;

namespace SFC.Identity.Infrastructure.Persistence.Configurations.Identity;
public class IdentityUserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserClaim<Guid>> builder)
    {
        builder.ToTable("UserClaims", DatabaseConstants.DefaultSchemaName);
    }
}
