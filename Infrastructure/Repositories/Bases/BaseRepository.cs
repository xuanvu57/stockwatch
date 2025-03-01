using Application.Constants;
using Application.Services.Interfaces;
using Domain.Constants;
using Domain.Entities.Bases;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories.Bases
{
    public abstract class BaseRepository<TRepository, TEntity>(
        ILogger<TRepository> logger,
        ILocalFileService localFileService,
        IToastManagerService toastManagerService,
        IMessageService messageService) : BaseFileRepository<TRepository, TEntity>(logger, localFileService)
        where TRepository : class
        where TEntity : StockBaseEntity
    {

        protected async Task<List<TEntity>> GetAll()
        {
            return await ReadData();
        }

        protected async Task<TEntity?> GetById(string id)
        {
            var entities = await GetAll();

            if (entities.Count == 0)
                return null;

            return entities.Find(x => x.Id == id);
        }

        protected async Task<bool> Create(TEntity entity)
        {
            return await SaveData([entity], false);
        }

        protected async Task<bool> Update(TEntity entity)
        {
            var entities = await GetAll();

            if (entities.Count == 0)
                return false;

            var existingEntity = entities.Find(x => x.Id == entity.Id);
            if (existingEntity is null)
                return false;

            entities.Remove(existingEntity);
            entities.Add(entity);

            return await SaveData(entities, true);
        }

        protected async Task<bool> Delete(string id)
        {
            var entities = await GetAll();

            if (entities.Count == 0)
                return true;

            var existingEntity = entities.Find(x => x.Id == id);
            if (existingEntity is null)
                return true;

            entities.Remove(existingEntity);

            return await SaveData(entities, true);
        }

        private async Task<List<TEntity>> ReadData()
        {
            try
            {
                var entities = await ReadFromFile();

                return entities ?? [];
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ApplicationConsts.LoggedErrorMessage.ErrorMessageInReadingData} {nameof(TEntity)}");
                await toastManagerService.Show($"{messageService.GetMessage(MessageConstants.MSG_CannotAccessResourceToReadData)}, {ex.Message}");
                return [];
            }
        }

        private async Task<bool> SaveData(IEnumerable<TEntity> entities, bool overwrite)
        {
            try
            {
                return await SaveToFile(entities, overwrite);
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
