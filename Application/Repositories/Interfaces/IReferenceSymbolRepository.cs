using Domain.Entities;

namespace Application.Repositories.Interfaces
{
    public interface IReferenceSymbolRepository
    {
        public Task<ReferenceSymbolInfo?> Get();
        public Task Save(ReferenceSymbolInfo symbol);
    }
}
