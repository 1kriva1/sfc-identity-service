using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SFC.Identity.Domain.Common;

namespace SFC.Identity.Infrastructure.Persistence.Configurations.Token;
public abstract class BaseTokenEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseTokenEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .ValueGeneratedOnAdd()
               .HasColumnOrder(0)
               .IsRequired(true);

        builder.Property(e => e.CreatedDate)
               .HasColumnOrder(1)
               .IsRequired(true);

        builder.Property(e => e.ExpiresDate)
              .HasColumnOrder(2)
              .IsRequired(true);

        builder.Property(e => e.Value)
              .HasColumnOrder(3)
              .IsRequired(true);
    }
}
