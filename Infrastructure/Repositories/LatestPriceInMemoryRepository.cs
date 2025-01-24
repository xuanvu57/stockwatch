using Application.Attributes;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Domain.Services;
using static Application.Constants.ApplicationEnums;

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

            if (latestPrice is not null)
            {
                var isExpired = DateOnly.FromDateTime(latestPrice.AtTime) < latestDate;

                if (isExpired)
                {
                    latestPriceDictionary.Remove(symbolId);

                    return null;
                }
            }

            return await Task.FromResult(latestPrice);
        }

        public async Task Save(LatestPriceEntity latestPrice)
        {
            if (!latestPriceDictionary.TryAdd(latestPrice.Id, latestPrice))
            {
                latestPriceDictionary[latestPrice.Id] = latestPrice;
            }

            await Task.CompletedTask;
        }
    }
}
