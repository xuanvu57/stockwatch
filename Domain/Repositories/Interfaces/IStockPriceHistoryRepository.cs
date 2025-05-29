using Domain.Entities;

namespace Domain.Repositories.Interfaces
{
    public interface IStockPriceHistoryRepository
    {
        Task Save(IEnumerable<StockPriceHistoryEntity> stockPriceHistories);
    }
}
