using stockwatch.Models;

namespace stockwatch.Repositories.Interfaces
{
    public interface IReferenceSymbolRepository
    {
        public Task<ReferenceSymbolInfo?> Get();
        public Task Save(ReferenceSymbolInfo symbol);
    }
}
