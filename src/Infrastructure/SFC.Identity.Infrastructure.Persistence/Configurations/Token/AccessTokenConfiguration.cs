using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SFC.Identity.Domain.Entities;

namespace SFC.Identity.Infrastructure.Persistence.Configurations.Token;
public class AccessTokenConfiguration : BaseTokenEntityConfiguration<AccessToken>
{
    public override void Configure(EntityTypeBuilder<AccessToken> builder)
    {
        builder.HasOne(b => b.RefreshToken)
               .WithOne(i => i.AccessToken)
               .HasForeignKey<RefreshToken>(b => b.Id)
               .IsRequired(true);

        builder.Navigation(e => e.RefreshToken)
               .AutoInclude(true);

        base.Configure(builder);
    }
}
