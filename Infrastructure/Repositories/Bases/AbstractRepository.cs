using Application.Constants;
using Application.Services.Interfaces;
using Domain.Constants;
using Domain.Entities.Bases;
using Infrastructure.Repositories.Bases.Interfaces;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Reflection;

namespace Infrastructure.Repositories.Bases
{
    public abstract class AbstractRepository<TEntity>(
        ILogger<AbstractRepository<TEntity>> logger,
        IToastManagerService toastManagerService,
        IMessageService messageService) : IBaseRepository<TEntity> where TEntity : StockBaseEntity
    {
        public async Task<List<TEntity>> GetAll()
        {
            try
            {
                return await GetAllInternal();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ApplicationConsts.LoggedErrorMessage.ErrorMessageInReadingData} {nameof(TEntity)}");
                await toastManagerService.Show($"{messageService.GetMessage(MessageConstants.MSG_CannotAccessResourceToReadData)}, {ex.Message}");
                return [];
            }
        }

        public async Task<TEntity?> GetById(string id)
        {
            try
            {
                return await GetByIdInternal(id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ApplicationConsts.LoggedErrorMessage.ErrorMessageInReadingData} {nameof(TEntity)}");
                await toastManagerService.Show($"{messageService.GetMessage(MessageConstants.MSG_CannotAccessResourceToReadData)}, {ex.Message}");
                return null;
            }
        }

        public async Task<bool> Create(TEntity entity)
        {
            try
            {
                return await CreateInternal(entity);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ApplicationConsts.LoggedErrorMessage.ErrorMessageInSavingData} {nameof(TEntity)}");
                await toastManagerService.Show($"{messageService.GetMessage(MessageConstants.MSG_CannotAccessResourceToWriteData)}, {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Delete(string id)
        {
            try
            {
                return await DeleteInternal(id);
            }

            catch (Exception ex)
            {
                logger.LogError(ex, $"{ApplicationConsts.LoggedErrorMessage.ErrorMessageInDeletingData} {nameof(TEntity)}");
                await toastManagerService.Show($"{messageService.GetMessage(MessageConstants.MSG_CannotAccessResourceToDeleteData)}, {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Update(TEntity entity)
        {
            try
            {
                return await UpdateInternal(entity);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ApplicationConsts.LoggedErrorMessage.ErrorMessageInSavingData} {nameof(TEntity)}");
                await toastManagerService.Show($"{messageService.GetMessage(MessageConstants.MSG_CannotAccessResourceToWriteData)}, {ex.Message}");
                return false;
            }
        }

        protected abstract Task<List<TEntity>> GetAllInternal();
        protected abstract Task<TEntity?> GetByIdInternal(string id);
        protected abstract Task<bool> CreateInternal(TEntity entity);
        protected abstract Task<bool> DeleteInternal(string id);
        protected abstract Task<bool> UpdateInternal(TEntity entity);

        protected string GetEntityName()
        {
            var entityName = typeof(TEntity).GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;

            return entityName ?? nameof(TEntity);
        }
    }
}
