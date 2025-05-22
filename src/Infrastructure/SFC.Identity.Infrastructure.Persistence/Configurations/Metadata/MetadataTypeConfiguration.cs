using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SFC.Identity.Domain.Entities.Metadata;
using SFC.Identity.Infrastructure.Persistence.Configurations.Base;
using SFC.Identity.Infrastructure.Persistence.Configurations.Metadata;
using SFC.Identity.Infrastructure.Persistence.Constants;

namespace SFC.Identity.Infrastructure.Persistence.Configurations.Metadata;
public class MetadataTypeConfiguration : EnumEntityConfiguration<MetadataType, MetadataTypeEnum>
{
    public override void Configure(EntityTypeBuilder<MetadataType> builder)
    {
        builder.ToTable("Types", DatabaseConstants.MetadataSchemaName);
        base.Configure(builder);
    }
}