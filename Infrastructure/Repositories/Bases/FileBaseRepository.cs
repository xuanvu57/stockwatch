using Application.Attributes;
using Application.Constants;
using Application.Services.Interfaces;
using Domain.Entities.Bases;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using static Application.Constants.ApplicationEnums;

namespace Infrastructure.Repositories.Bases
{
    [DIService(DIServiceLifetime.Skipped)]
    public class FileBaseRepository<TEntity>(
        ILogger<FileBaseRepository<TEntity>> logger,
        ILocalFileService localFileService,
        IToastManagerService toastManagerService,
        IMessageService messageService) : AbstractRepository<TEntity>(logger, toastManagerService, messageService)
        where TEntity : StockBaseEntity
    {
        private string FilePath => Path.Combine(localFileService.GetRootDirectory(), $"{GetEntityName()}.json");

        protected override async Task<List<TEntity>> GetAllInternal()
        {
            return await ReadDataFromFile();
        }

        protected override async Task<TEntity?> GetByIdInternal(string id)
        {
            var entities = await GetAll();

            if (entities.Count == 0)
                return null;

            return entities.Find(x => x.Id == id);
        }

        protected override async Task<bool> CreateInternal(TEntity entity)
        {
            return await SaveDataToFile([entity], false);
        }

        protected override async Task<bool> DeleteInternal(string id)
        {
            var entities = await GetAll();

            if (entities.Count == 0)
                return true;

            var existingEntity = entities.Find(x => x.Id == id);
            if (existingEntity is null)
                return true;

            entities.Remove(existingEntity);

            return await SaveDataToFile(entities, true);
        }

        protected override async Task<bool> UpdateInternal(TEntity entity)
        {
            var entities = await GetAll();

            if (entities.Count == 0)
                return false;

            var existingEntity = entities.Find(x => x.Id == entity.Id);
            if (existingEntity is null)
                return false;

            entities.Remove(existingEntity);
            entities.Add(entity);

            return await SaveDataToFile(entities, true);
        }

        private async Task<List<TEntity>> ReadDataFromFile()
        {
            try
            {
                var rawDatas = await File.ReadAllLinesAsync(FilePath);
                List<TEntity> entities = [];
                foreach (var rawData in rawDatas)
                {
                    entities.Add(JsonSerializer.Deserialize<TEntity>(rawData)!);
                }

                return entities;
            }
            catch (FileNotFoundException ex)
            {
                logger.LogError(ex, $"{nameof(TEntity)} {ApplicationConsts.LoggedErrorMessage.ErrorMessageWhenFileNotFound}");
                throw;
            }
        }

        private async Task<bool> SaveDataToFile(IEnumerable<TEntity> entities, bool overwrite)
        {
            if (!entities.Any())
            {
                if (overwrite)
                {
                    await File.WriteAllTextAsync(FilePath, string.Empty);
                }
                return true;
            }

            var serializedDatas = entities.Select(x => JsonSerializer.Serialize(x)).ToList();

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
    }
}
