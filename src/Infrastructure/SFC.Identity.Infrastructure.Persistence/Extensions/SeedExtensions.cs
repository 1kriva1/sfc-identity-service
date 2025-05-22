using Microsoft.EntityFrameworkCore;

using SFC.Identity.Domain.Common;

namespace SFC.Identity.Infrastructure.Persistence.Extensions;
public static class SeedExtensions
{
    public static void SeedEnumValues<TEntity, TEnum>(this ModelBuilder builder, Func<TEnum, TEntity> converter)
       where TEntity : EnumEntity<TEnum>, new()
       where TEnum : struct
    {
        IEnumerable<TEntity> entities = Enum.GetValues(typeof(TEnum)).Cast<object>()
            .Select(value => converter((TEnum)value));
        builder.Entity<TEntity>().HasData(entities);
    }
}