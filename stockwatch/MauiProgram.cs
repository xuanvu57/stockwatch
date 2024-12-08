using CommunityToolkit.Maui;
using Infrastructure.Configurations;
using Microsoft.Extensions.Logging;
using stockwatch.Configurations;

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
                .RegisterAppSettings();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.RegisterLogger();
            builder.Services.RegisterDependencies();
            builder.Services.AddScoped<MainPage>();
            builder.Services.AddScoped<FindPotentialSymbol>();

            LanguageRegister.SetLanguage();

            return builder.Build();
        }
    }
}
