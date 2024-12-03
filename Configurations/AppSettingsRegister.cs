using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace stockwatch.Configurations
{
    public static class AppSettingsRegister
    {
        public static void RegisterAppSettings(this MauiAppBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var appSettingsFile = $"{assembly.GetName().Name}.appsettings.json";
            var localAppSettingsFile = $"{assembly.GetName().Name}.appsettings.local.json";

            using var streamAppSettings = assembly.GetManifestResourceStream(appSettingsFile);

            var configurationBuilder = new ConfigurationBuilder()
                    .AddJsonStream(streamAppSettings!);

            IConfigurationRoot config;
            if (assembly.GetManifestResourceInfo(localAppSettingsFile) is not null)
            {
                using var streamLocalAppSettings = assembly.GetManifestResourceStream(localAppSettingsFile);

                configurationBuilder = configurationBuilder
                    .AddJsonStream(streamLocalAppSettings!);

                config = configurationBuilder.Build();
            }
            else
            {
                config = configurationBuilder.Build();
            }

            builder.Configuration.AddConfiguration(config);
        }
    }
}
