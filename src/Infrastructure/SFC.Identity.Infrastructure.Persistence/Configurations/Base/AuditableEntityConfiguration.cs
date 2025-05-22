using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SFC.Identity.Domain.Common.Interfaces;
using SFC.Identity.Infrastructure.Persistence.Entities;

namespace SFC.Identity.Infrastructure.Persistence.Configurations.Base;

public class AuditableEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class, IAuditableEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Property(e => e.CreatedDate)
               .IsRequired(true);

        builder.HasOne<ApplicationUser>()
               .WithMany()
               .HasForeignKey(t => t.CreatedBy)
               .OnDelete(DeleteBehavior.ClientCascade)
               .IsRequired(true);

        builder.Property(e => e.LastModifiedDate)
               .IsRequired(true);

        builder.HasOne<ApplicationUser>()
               .WithMany()
               .HasForeignKey(t => t.LastModifiedBy)
               .OnDelete(DeleteBehavior.ClientCascade)
               .IsRequired(true);
    }
}