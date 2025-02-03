using Application.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using static Application.Constants.ApplicationEnums;

namespace Infrastructure.Configurations
{
    public static class ServiceRegister
    {
        public static void RegisterDependencies(this IServiceCollection serviceCollection, Assembly? presentationLayerAssembly = null)
        {
            var applicationLayerAssembly = typeof(DIServiceAttribute).Assembly;
            var infrastructureLayerAssembly = typeof(ServiceRegister).Assembly;

            var assemblies = new[] { applicationLayerAssembly, infrastructureLayerAssembly };
            if (presentationLayerAssembly is not null)
            {
                assemblies = [.. assemblies, presentationLayerAssembly];
            }

            serviceCollection
                .Scan(scrutor => scrutor
                    .FromAssemblies(assemblies)

                    .AddClasses(x => x
                        .Where(type => type
                            .GetCustomAttribute<DIServiceAttribute>()?.Lifetime == DIServiceLifetime.Transient))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()

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