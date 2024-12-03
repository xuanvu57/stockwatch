using Application.Dtos;
using Domain.Entities;

namespace Application.Services.Interfaces
{
    public interface IAnalyzorService
    {
        public Task Analyze(SymbolInfo symbol, ReferenceSymbolInfo targetSymbol);
    }
}
