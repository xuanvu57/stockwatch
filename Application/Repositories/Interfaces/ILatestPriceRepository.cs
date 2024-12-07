using Domain.Entities;

namespace Application.Repositories.Interfaces
{
    public interface ILatestPriceRepository
    {
        public Task Save(LatestPriceEntity latestPrice);
        public Task<LatestPriceEntity?> Get(string symbolId);
    }
}
