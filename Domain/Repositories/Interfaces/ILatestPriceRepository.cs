using Domain.Entities;

namespace Domain.Repositories.Interfaces
{
    public interface ILatestPriceRepository
    {
        public Task Save(LatestPriceEntity latestPrice);
        public Task<LatestPriceEntity?> Get(string symbolId);
    }
}
