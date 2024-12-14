using Application.Attributes;
using Application.Services.Interfaces;
using static Domain.Constants.StockWatchEnums;

namespace stockwatch.Services
{
    [DIService(DIServiceLifetime.Singleton)]
    public class LocalFileService : ILocalFileService
    {
        public string GetRootDirectory()
        {
            return FileSystem.AppDataDirectory;
        }
    }
}
