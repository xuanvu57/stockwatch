namespace stockwatch.Services
{
    internal static class PlatformServiceProvider
    {
        public static IServiceProvider ServiceProvider
        {
            get
            {
                var app = IPlatformApplication.Current;
                if (app == null)
                    throw new InvalidOperationException("Cannot resolve current application. Services should be accessed after MauiProgram initialization.");
                return app.Services;
            }
        }
    }
}
