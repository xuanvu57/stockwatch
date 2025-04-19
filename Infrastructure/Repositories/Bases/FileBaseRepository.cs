using Application.Attributes;
using Application.Constants;
using Application.Services.Interfaces;
using Domain.Constants;
using Domain.Entities.Bases;
using Infrastructure.Repositories.Bases.Interfaces;
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
        IMessageService messageService) : AbstractRepository<TEntity>, IBaseRepository<TEntity>
        where TEntity : StockBaseEntity
    {
        private string FilePath => Path.Combine(localFileService.GetRootDirectory(), $"{GetEntityName()}.json");

        public async Task<List<TEntity>> GetAll()
        {
            return await ReadDataFromFile();
        }

        public async Task<TEntity?> GetById(string id)
        {
            var entities = await GetAll();

            if (entities.Count == 0)
                return null;

            return entities.Find(x => x.Id == id);
        }

        public async Task<bool> Create(TEntity entity)
        {
            return await SaveDataToFile([entity], false);
        }

        public async Task<bool> Update(TEntity entity)
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

        public async Task<bool> Delete(string id)
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
                return [];
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ApplicationConsts.LoggedErrorMessage.ErrorMessageInReadingData} {nameof(TEntity)}");
                await toastManagerService.Show($"{messageService.GetMessage(MessageConstants.MSG_CannotAccessResourceToReadData)}, {ex.Message}");
                return [];
            }
        }

        private async Task<bool> SaveDataToFile(IEnumerable<TEntity> entities, bool overwrite)
        {
            try
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
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ApplicationConsts.LoggedErrorMessage.ErrorMessageInSavingData} {nameof(TEntity)}");
                await toastManagerService.Show($"{messageService.GetMessage(MessageConstants.MSG_CannotAccessResourceToWriteData)}, {ex.Message}");
                return false;
            }
        }
    }
}
