using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SFC.Identity.Domain.Common;

namespace SFC.Identity.Infrastructure.Persistence.Configurations.Base;
public abstract class EnumEntityConfiguration<TEntity, TEnum> : IEntityTypeConfiguration<TEntity>
    where TEntity : EnumEntity<TEnum>
    where TEnum : struct
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .ValueGeneratedNever()
               .HasColumnOrder(0)
               .IsRequired(true);

        builder.Property(e => e.Title)
               .IsRequired(true);
    }
}