using Application.Attributes;
using Infrastructure.Services;
using System.Reflection;
using static Domain.Constants.StockWatchEnums;

namespace stockwatch.Configurations
{
    public static class ServiceRegister
    {
        public static void RegisterDependencies(this IServiceCollection serviceCollection)
        {
            var assembly1 = typeof(MauiProgram).Assembly;
            var assembly2 = typeof(DIServiceAttribute).Assembly;
            var assembly3 = typeof(ToastManagerService).Assembly;

            serviceCollection.Scan(scrutor => scrutor
                .FromAssemblies(assembly1, assembly2, assembly3)

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