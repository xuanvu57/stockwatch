using Application.Services.Interfaces;
using Domain.Entities.Bases;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories.Bases
{
    public abstract class BaseRepository<TRepository, TEntity>(
        ILogger<TRepository> logger,
        ILocalFileService localFileService,
        IToastManagerService toastManagerService) :
        BaseFileRepository<TRepository, TEntity>(logger, localFileService, toastManagerService)
        where TRepository : class
        where TEntity : StockBaseEntity
    {
        protected async Task<List<TEntity>> GetAll()
        {
            var entities = await ReadFromFile();

            return entities ?? [];
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
            return await SaveToFile([entity], false);
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

            return await SaveToFile(entities, true);
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

            return await SaveToFile(entities, true);
        }
    }
}
