using Application.Attributes;
using Application.Repositories.Interfaces;
using Application.Services;
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
            var latestDate = StockRulesService.GetLatestAvailableDate();

            latestPriceDictionary.TryGetValue(symbolId, out var latestPrice);

            if (latestPrice is not null && DateOnly.FromDateTime(latestPrice.AtTime) < latestDate)
            {
                latestPriceDictionary.Remove(symbolId);

                return null;
            }

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
