using Application.Constants;
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
        ILocalFileService localFileService)
        where TRepository : class
        where TEntity : StockBaseEntity
    {

        protected ILogger<TRepository> logger = logger;

        private string FilePath => Path.Combine(localFileService.GetRootDirectory(), $"{GetEntityName()}.json");

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
                logger.LogError(ex, $"{nameof(TEntity)} {ApplicationConsts.LoggedErrorMessage.ErrorMessageWhenFileNotFound}");
                return null;
            }
        }

        protected async Task<bool> SaveToFile(IEnumerable<TEntity> datas, bool overwrite = false)
        {
            if (!datas.Any())
            {
                if (overwrite)
                {
                    await File.WriteAllTextAsync(FilePath, string.Empty);
                }
                return true;
            }

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

        private static string GetEntityName()
        {
            var entityName = typeof(TEntity).GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;

            return entityName ?? nameof(TEntity);
        }
    }
}
