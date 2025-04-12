using Application.Attributes;
using Application.Services.Interfaces;
using static Application.Constants.ApplicationEnums;

namespace stockwatch.Services
{
    [DIService(DIServiceLifetime.Singleton)]
    public class LocalFileService : ILocalFileService
    {
        public string GetRootDirectory()
        {
            return FileSystem.AppDataDirectory;
        }

        public async Task<string> ReadFileAsync(string filePath)
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(filePath);
            var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}
