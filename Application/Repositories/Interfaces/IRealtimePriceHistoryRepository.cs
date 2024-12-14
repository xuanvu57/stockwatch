using Domain.Entities;

namespace Application.Repositories.Interfaces
{
    public interface IRealtimePriceHistoryRepository
    {
        public Task<IList<RealtimePriceHistoryEntity>> Get(int symbolId, DateTime fromTime, DateTime toTime);
        public Task Save(RealtimePriceHistoryEntity priceHistory);
    }
}
