using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNetCore.Components.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddScopedImplementations<T>(this IServiceCollection services)
    {
        foreach (Type type in typeof(T).Assembly.GetTypes().Where(Implements<T>))
            services.TryAddScoped(type);
    }
    private static Boolean Implements<T>(Type type)
    {
        return !type.IsAbstract && type.IsClass && typeof(T).IsAssignableFrom(type);
    }
}
