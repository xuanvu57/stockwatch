using System.Reflection;

namespace stockwatch.Configurations
{
    public static class MauiControlRegister
    {
        public static void RegisterMauiPagesAsScopedService(this IServiceCollection serviceCollection, Assembly assembly)
        {
            assembly.ExportedTypes
                .Where(x => x.IsClass && x.BaseType == typeof(ContentPage))
                .ToList()
                .ForEach(x => serviceCollection.AddScoped(x));
        }
    }
}
