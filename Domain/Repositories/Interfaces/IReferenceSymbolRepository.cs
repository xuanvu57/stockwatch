using Domain.Entities;

namespace Domain.Repositories.Interfaces
{
    public interface IReferenceSymbolRepository
    {
        public Task<ReferenceSymbolEntity?> Get();
        public Task Save(ReferenceSymbolEntity entity);
    }
}
