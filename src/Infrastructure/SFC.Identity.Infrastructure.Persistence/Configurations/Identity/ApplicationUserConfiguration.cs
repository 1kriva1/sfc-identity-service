using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SFC.Identity.Infrastructure.Persistence.Configurations.Base;
using SFC.Identity.Infrastructure.Persistence.Constants;
using SFC.Identity.Infrastructure.Persistence.Entities;

namespace SFC.Identity.Infrastructure.Persistence.Configurations.Identity;
public class ApplicationUserConfiguration : AuditableEntityConfiguration<ApplicationUser>
{
    public override void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users", DatabaseConstants.DefaultSchemaName);
        base.Configure(builder);
    }
}