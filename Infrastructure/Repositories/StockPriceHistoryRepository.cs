using Application.Attributes;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Repositories.Bases.Interfaces;
using static Application.Constants.ApplicationEnums;

namespace Infrastructure.Repositories
{
    [DIService(DIServiceLifetime.Scoped)]
    public class StockPriceHistoryRepository(IBaseRepository<StockPriceHistoryEntity> baseRepository) : IStockPriceHistoryRepository
    {
        public async Task Save(IEnumerable<StockPriceHistoryEntity> stockPriceHistories)
        {
            foreach (var stockPriceHistory in stockPriceHistories)
            {
                await baseRepository.Create(stockPriceHistory);
            }
        }
    }
}
