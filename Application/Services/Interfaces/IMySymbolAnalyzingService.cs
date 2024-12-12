using Application.Dtos;
using Domain.Entities;

namespace Application.Services.Interfaces
{
    public interface IMySymbolAnalyzingService
    {
        public Task Analyze(StockPriceInRealtime stockPrice, ReferenceSymbolEntity targetSymbol);
    }
}
