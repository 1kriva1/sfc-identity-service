using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SFC.Identity.Domain.Entities.Metadata;
using SFC.Identity.Infrastructure.Persistence.Configurations.Base;
using SFC.Identity.Infrastructure.Persistence.Constants;

namespace SFC.Identity.Infrastructure.Persistence.Configurations.Metadata;
public class MetadataStateConfiguration : EnumEntityConfiguration<MetadataState, MetadataStateEnum>
{
    public override void Configure(EntityTypeBuilder<MetadataState> builder)
    {
        builder.ToTable("States", DatabaseConstants.MetadataSchemaName);
        base.Configure(builder);
    }
}