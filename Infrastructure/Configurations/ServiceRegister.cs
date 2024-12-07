using Application.Attributes;
using Infrastructure.Services;
using System.Reflection;
using static Domain.Constants.StockWatchEnums;

namespace Infrastructure.Configurations
{
    public static class ServiceRegister
    {
        public static void RegisterDependencies(this IServiceCollection serviceCollection)
        {
            var applicationAssembly = typeof(DIServiceAttribute).Assembly;
            var infrastructureAssembly = typeof(ToastManagerService).Assembly;

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
        }
    }
}