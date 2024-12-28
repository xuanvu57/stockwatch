using Application.Attributes;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities;
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
        BaseFileRepository<ReferenceSymbolRepository>(logger, localFileService, toastManagerService, "ReferenceSymbol.json"), IReferenceSymbolRepository
    {
        public async Task<ReferenceSymbolEntity?> Get()
        {
            var referenceSymbolEntities = await ReadFromFile<ReferenceSymbolEntity>();

            return referenceSymbolEntities?.Count > 0 ? referenceSymbolEntities[0] : null;
        }

        public async Task Save(ReferenceSymbolEntity symbol)
        {
            await SaveToFile([symbol], true);
        }
    }
}
