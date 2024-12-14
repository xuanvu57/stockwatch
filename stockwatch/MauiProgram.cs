using CommunityToolkit.Maui;
using Infrastructure.Configurations;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using stockwatch.Configurations;
using stockwatch.Pages;

namespace stockwatch
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureMopups()
                .RegisterAppSettings();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.RegisterLogger(FileSystem.AppDataDirectory);
            builder.Services.RegisterDependencies(typeof(MauiProgram).Assembly);
            builder.Services.AddScoped<MainPage>();
            builder.Services.AddScoped<FindPotentialSymbolPage>();

            LanguageRegister.SetLanguage();

            return builder.Build();
        }
    }
}
