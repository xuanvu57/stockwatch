using Serilog;

namespace Infrastructure.Configurations
{
    public static class LoggingRegister
    {
        private const string LogFolderName = "logs";
        private const string LogFileName = "log.txt";

        public static void RegisterLogger(this IServiceCollection services)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Debug()
                .WriteTo.File(Path.Combine(FileSystem.AppDataDirectory, LogFolderName, LogFileName), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            services.AddSerilog(logger);
        }
    }
}
