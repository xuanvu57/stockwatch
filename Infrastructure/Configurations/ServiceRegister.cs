using Application.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using static Domain.Constants.StockWatchEnums;

namespace Infrastructure.Configurations
{
    public static class ServiceRegister
    {
        public static void RegisterDependencies(this IServiceCollection serviceCollection, Assembly? presentationLayerAssembly = null)
        {
            var applicationAssembly = typeof(DIServiceAttribute).Assembly;
            var infrastructureAssembly = typeof(ServiceRegister).Assembly;

            serviceCollection.Scan(scrutor => scrutor
                .FromAssemblies(applicationAssembly, infrastructureAssembly)

                .AddClasses(x => x
                    .Where(type => type
                        .GetCustomAttribute<DIServiceAttribute>()?.Lifetime == DIServiceLifetime.Scoped))
                .AsImplementedInterfaces()
                .WithScopedLifetime()

                .AddClasses(x => x
                    .Where(type => type
                        .GetCustomAttribute<DIServiceAttribute>()?.Lifetime == DIServiceLifetime.Singleton))
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                );

            if (presentationLayerAssembly is not null)
            {
                serviceCollection.Scan(scrutor => scrutor
                    .FromAssemblies(presentationLayerAssembly)

                    .AddClasses(x => x
                        .Where(type => type
                            .GetCustomAttribute<DIServiceAttribute>()?.Lifetime == DIServiceLifetime.Scoped))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()

                    .AddClasses(x => x
                        .Where(type => type
                            .GetCustomAttribute<DIServiceAttribute>()?.Lifetime == DIServiceLifetime.Singleton))
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime()
                    );
            }
        }
    }
}