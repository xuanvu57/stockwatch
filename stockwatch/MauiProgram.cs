using CommunityToolkit.Maui;
using Infrastructure.Configurations;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
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
                .UseMauiCommunityToolkit(options =>
                {
                    options.SetShouldSuppressExceptionsInConverters(true);
                })
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
            builder.Services.RegisterMauiPagesAsScopedService(typeof(MauiProgram).Assembly);
            builder.Services.RegisterFirestoreDb();

            LanguageRegister.SetLanguage();

            return builder.Build();
        }
    }
}
