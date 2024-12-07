using Application.Attributes;
using Application.Repositories.Interfaces;
using Domain.Entities;
using static Domain.Constants.StockWatchEnums;

namespace Infrastructure.Repositories
{
    [DIService(DIServiceLifetime.Singleton)]
    public class LatestPriceInMemoryRepository : ILatestPriceRepository
    {
        private readonly Dictionary<string, LatestPriceEntity> latestPriceDictionary = [];

        public async Task<LatestPriceEntity?> Get(string symbolId)
        {
            latestPriceDictionary.TryGetValue(symbolId, out var latestPrice);

            return await Task.FromResult(latestPrice);
        }

        public async Task Save(LatestPriceEntity latestPrice)
        {
            if (!latestPriceDictionary.TryAdd(latestPrice.SymbolId, latestPrice))
            {
                latestPriceDictionary[latestPrice.SymbolId] = latestPrice;
            }

            await Task.CompletedTask;
        }
    }
}
