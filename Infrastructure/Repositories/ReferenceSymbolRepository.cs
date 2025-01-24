using Application.Attributes;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Repositories.Bases;
using Microsoft.Extensions.Logging;
using static Domain.Constants.StockWatchEnums;

namespace Infrastructure.Repositories
{
    [DIService(DIServiceLifetime.Scoped)]
    public class ReferenceSymbolRepository(
        ILogger<ReferenceSymbolRepository> logger,
        ILocalFileService localFileService,
        IToastManagerService toastManagerService) :
        BaseRepository<ReferenceSymbolRepository, ReferenceSymbolEntity>(logger, localFileService, toastManagerService), IReferenceSymbolRepository
    {
        public async Task<ReferenceSymbolEntity?> Get()
        {
            var entities = await GetAll();

            return entities.FirstOrDefault();
        }

        public async Task Save(ReferenceSymbolEntity entity)
        {
            var existingEntities = await GetAll();

            foreach (var existingEntity in existingEntities)
            {
                await Delete(existingEntity.Id);
            }

            await Create(entity);
        }
    }
}
