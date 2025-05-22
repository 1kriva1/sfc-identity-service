using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SFC.Identity.Domain.Entities.Metadata;
using SFC.Identity.Infrastructure.Persistence.Constants;

namespace SFC.Identity.Infrastructure.Persistence.Configurations.Metadata;
public class MetadataConfiguration : IEntityTypeConfiguration<MetadataEntity>
{
    public void Configure(EntityTypeBuilder<MetadataEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(e => new { e.Service, e.Type });

        builder.HasOne<MetadataService>()
               .WithMany()
               .HasForeignKey(t => t.Service)
               .IsRequired(true);

        builder.HasOne<MetadataType>()
              .WithMany()
              .HasForeignKey(t => t.Type)
              .IsRequired(true);

        builder.HasOne<MetadataState>()
               .WithMany()
               .HasForeignKey(t => t.State)
               .IsRequired(true);

        builder.HasOne<MetadataDomain>()
               .WithMany()
               .HasForeignKey(t => t.Domain)
               .IsRequired(true);

        builder.ToTable("Metadata", DatabaseConstants.MetadataSchemaName);
    }
}