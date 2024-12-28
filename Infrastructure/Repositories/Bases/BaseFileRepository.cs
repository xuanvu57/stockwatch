using Application.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Repositories.Bases
{
    public abstract class BaseFileRepository<TRepository>(
        ILogger<TRepository> logger,
        ILocalFileService localFileService,
        IToastManagerService toastManagerService,
        string fileName) where TRepository : class
    {
        protected string FilePath { get; } = Path.Combine(localFileService.GetRootDirectory(), fileName);

        protected async Task<List<T>?> ReadFromFile<T>() where T : class
        {
            try
            {
                var rawDatas = await File.ReadAllLinesAsync(FilePath);
                List<T> data = [];
                foreach (var rawData in rawDatas)
                {
                    data.Add(JsonSerializer.Deserialize<T>(rawData)!);
                }

                return data;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in getting {nameof(T)}");

                await toastManagerService.Show($"Cannot read data, {ex.Message}");
                return null;
            }
        }

        protected async Task<bool> SaveToFile<T>(IEnumerable<T> datas, bool overwrite = false)
        {
            try
            {
                if (!datas.Any())
                    return true;

                var serializedDatas = datas.Select(x => JsonSerializer.Serialize(x)).ToList();

                if (overwrite)
                {
                    await File.WriteAllLinesAsync(FilePath, serializedDatas);
                }
                else
                {
                    await File.AppendAllLinesAsync(FilePath, serializedDatas);
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in saving {nameof(T)}");

                await toastManagerService.Show($"Cannot write data, {ex.Message}");

                return false;
            }
        }
    }
}
