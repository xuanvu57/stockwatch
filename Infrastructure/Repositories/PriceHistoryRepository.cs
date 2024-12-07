using Application.Attributes;
using Application.Repositories.Interfaces;
using Domain.Entities;
using Infrastructure.Repositories.Bases;
using static Domain.Constants.StockWatchEnums;

namespace Infrastructure.Repositories
{
    [DIService(DIServiceLifetime.Scoped)]
    public class PriceHistoryRepository() : BaseFileRepository("PriceHistory.json"), IPriceHistoryRepository
    {
        public async Task<IList<PriceHistoryEntity>> Get(int symbolId, DateTime fromTime, DateTime toTime)
        {
            var prices = new List<PriceHistoryEntity>();

            return await Task.FromResult(prices);
        }

        public Task Save(PriceHistoryEntity priceHistory)
        {
            return Task.CompletedTask;
        }
    }
}
