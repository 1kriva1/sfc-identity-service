using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SFC.Identity.Infrastructure.Persistence.Constants;
using SFC.Identity.Infrastructure.Persistence.Entities;

namespace SFC.Identity.Infrastructure.Persistence.Configurations.Identity;
public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("Roles", DatabaseConstants.DefaultSchemaName);
    }
}
