using Domain.Entities;

namespace Application.Repositories.Interfaces
{
    public interface IReferenceSymbolRepository
    {
        public Task<ReferenceSymbolEntity?> Get();
        public Task Save(ReferenceSymbolEntity symbol);
    }
}
