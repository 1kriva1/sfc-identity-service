using System.Reflection;

using AutoMapper;

namespace SFC.Identity.Application.Common.Extensions;
public static class MappingExtensions
{
    public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
    {
        BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

        Type sourceType = typeof(TSource);

        PropertyInfo[] destinationProperties = typeof(TDestination).GetProperties(flags);

        foreach (PropertyInfo property in destinationProperties)
        {
            if (sourceType.GetProperty(property.Name, flags) == null)
            {
                expression.ForMember(property.Name, opt => opt.Ignore());
            }
        }

        return expression;
    }
}
