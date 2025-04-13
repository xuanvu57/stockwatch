using Application.Attributes;
using Application.Constants;
using Infrastructure.Repositories.Bases;
using Infrastructure.Repositories.Bases.Interfaces;
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

            RegisterBaseRepository(serviceCollection);
        }

        private static void RegisterBaseRepository(IServiceCollection serviceCollection)
        {
            if (OperatingSystem.IsAndroid())
            {
                if (OperatingSystem.IsAndroidVersionAtLeast(ApplicationConsts.MinAndroidVersionSupportFirebaseSdk))
                {
                    serviceCollection.AddScoped(typeof(IBaseRepository<>), typeof(FirestoreBaseRepository<>));
                }
                else
                {
                    serviceCollection.AddScoped(typeof(IBaseRepository<>), typeof(FirestoreRestApiBaseRepositoryy<>));
                }
            }
            else
            {
                serviceCollection.AddScoped(typeof(IBaseRepository<>), typeof(FirestoreBaseRepository<>));
            }
        }
    }
}