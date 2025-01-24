using Application.Services.Interfaces;
using Domain.Entities.Bases;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace Infrastructure.Repositories.Bases
{
    public abstract class BaseFileRepository<TRepository, TEntity>(
        ILogger<TRepository> logger,
        ILocalFileService localFileService,
        IToastManagerService toastManagerService)
        where TRepository : class
        where TEntity : StockBaseEntity
    {
        private string FilePath { get; } = Path.Combine(localFileService.GetRootDirectory(), $"{GetEntityName()}.json");

        protected async Task<List<TEntity>?> ReadFromFile()
        {
            try
            {
                var rawDatas = await File.ReadAllLinesAsync(FilePath);
                List<TEntity> data = [];
                foreach (var rawData in rawDatas)
                {
                    data.Add(JsonSerializer.Deserialize<TEntity>(rawData)!);
                }

                return data;
            }
            catch (FileNotFoundException ex)
            {
                logger.LogError(ex, $"Error in getting {nameof(TEntity)}");

                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in getting {nameof(TEntity)}");

                await toastManagerService.Show($"Cannot read data, {ex.Message}");
                return null;
            }
        }

        protected async Task<bool> SaveToFile(IEnumerable<TEntity> datas, bool overwrite = false)
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
                logger.LogError(ex, $"Error in saving {nameof(TEntity)}");

                await toastManagerService.Show($"Cannot write data, {ex.Message}");

                return false;
            }
        }

        private static string GetEntityName()
        {
            var entityName = typeof(TEntity).GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;

            return entityName ?? nameof(TEntity);
        }
    }
}
