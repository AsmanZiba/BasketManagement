using BasketManagement.Application.Common.Interfaces;
using System.Reflection;

namespace BasketManagement.API.Extensions;

public static class DependencyRegistrar
{
    public static IServiceCollection RegisterDependencies(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
        {
            assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        // ثبت Scoped
        RegisterTypes<IScopedDependency>(services, assemblies, ServiceLifetime.Scoped);

        // ثبت Transient
        RegisterTypes<ITransientDependency>(services, assemblies, ServiceLifetime.Transient);

        // ثبت Singleton
        RegisterTypes<ISingletonDependency>(services, assemblies, ServiceLifetime.Singleton);

        return services;
    }

    private static void RegisterTypes<TMarker>(
        IServiceCollection services,
        Assembly[] assemblies,
        ServiceLifetime lifetime)
    {
        var types = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericTypeDefinition &&
                t.IsAssignableTo(typeof(TMarker)) && t != typeof(TMarker))
            .ToList();

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces()
                .Where(i =>
                    i != typeof(TMarker) &&
                    !i.IsAssignableTo(typeof(TMarker)))
                .ToList();

            // اگر اینترفیس خاصی نداشت، خودش را ثبت کن
            if (!interfaces.Any())
            {
                services.Add(new ServiceDescriptor(type, type, lifetime));
            }
            else
            {
                // هر اینترفیس را به صورت جداگانه ثبت کن
                foreach (var @interface in interfaces)
                {
                    services.Add(new ServiceDescriptor(@interface, type, lifetime));
                }
            }
        }
    }
}