using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SFC.Identity.Domain.Entities.Metadata;
using SFC.Identity.Infrastructure.Persistence.Configurations.Base;
using SFC.Identity.Infrastructure.Persistence.Constants;

namespace SFC.Identity.Infrastructure.Persistence.Configurations.Metadata;
public class MetadataDomainConfiguration : EnumEntityConfiguration<MetadataDomain, MetadataDomainEnum>
{
    public override void Configure(EntityTypeBuilder<MetadataDomain> builder)
    {
        builder.ToTable("Domains", DatabaseConstants.MetadataSchemaName);
        base.Configure(builder);
    }
}