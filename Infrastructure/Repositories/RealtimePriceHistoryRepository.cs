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
    public class RealtimePriceHistoryRepository(
        ILogger<RealtimePriceHistoryRepository> logger,
        ILocalFileService localFileService,
        IToastManagerService toastManagerService) :
        BaseFileRepository<RealtimePriceHistoryRepository>(logger, localFileService, toastManagerService, "RealtimePriceHistory.json"), IRealtimePriceHistoryRepository
    {
        public Task<IList<RealtimePriceHistoryEntity>> Get(int symbolId, DateTime fromTime, DateTime toTime)
        {
            throw new NotImplementedException();
        }

        public async Task Save(RealtimePriceHistoryEntity priceHistory)
        {
            await SaveToFile([priceHistory], false);
        }
    }
}
