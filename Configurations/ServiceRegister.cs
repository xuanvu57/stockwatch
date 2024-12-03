using stockwatch.Attributes;
using System.Reflection;
using static stockwatch.Constants.StockWatchEnums;

namespace stockwatch.Configurations
{
    public static class ServiceRegister
    {
        public static void RegisterDependencies(this IServiceCollection serviceCollection)
        {
            var assembly = typeof(MauiProgram).Assembly;

            serviceCollection.Scan(scrutor => scrutor
                .FromAssemblies(assembly)
                .AddClasses(x => x
                    .Where(type => type
                        .GetCustomAttribute<DIServiceAttribute>()?.Lifetime == DIServiceLifetime.Scoped))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
                );

            serviceCollection.Scan(scrutor => scrutor
                .FromAssemblies(assembly)
                .AddClasses(x => x
                    .Where(type => type
                        .GetCustomAttribute<DIServiceAttribute>()?.Lifetime == DIServiceLifetime.Singleton))
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                );
        }
    }
}