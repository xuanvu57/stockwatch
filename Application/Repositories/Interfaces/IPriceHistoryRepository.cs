using Domain.Entities;

namespace Application.Repositories.Interfaces
{
    public interface IPriceHistoryRepository
    {
        public Task<IList<PriceHistoryEntity>> Get(int symbolId, DateTime fromTime, DateTime toTime);
        public Task Save(PriceHistoryEntity priceHistory);
    }
}
