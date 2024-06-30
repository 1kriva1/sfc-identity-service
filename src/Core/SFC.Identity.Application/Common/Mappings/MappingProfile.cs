using System.Reflection;

using AutoMapper;

namespace SFC.Identity.Application.Common.Mappings;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        Configure();

        ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private void Configure()
    {
        AllowNullCollections = true;
    }

    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        string mappingMethodName = nameof(IMapFrom<object>.Mapping);

        static bool HasInterface(Type t) => t.IsGenericType &&
            (t.GetGenericTypeDefinition() == typeof(IMapFrom<>) || t.GetGenericTypeDefinition() == typeof(IMapFromReverse<>));

        List<Type> types = assembly.GetExportedTypes()
                                   .Where(t => t.GetInterfaces().Any(HasInterface) && !t.IsInterface)
                                   .ToList();

        Type[] argumentTypes = [typeof(Profile)];

        foreach (Type type in types)
        {
            object? instance = Activator.CreateInstance(type);

            MethodInfo? methodInfo = type.GetMethod(mappingMethodName);

            if (methodInfo != null)
            {
                methodInfo.Invoke(instance, [this]);
            }
            else
            {
                List<Type> interfaces = type.GetInterfaces().Where(HasInterface).ToList();

                if (interfaces.Count > 0)
                {
                    foreach (Type @interface in interfaces)
                    {
                        MethodInfo? interfaceMethodInfo = @interface.GetMethod(mappingMethodName, argumentTypes);

                        interfaceMethodInfo?.Invoke(instance, [this]);
                    }
                }
            }
        }
    }
}
