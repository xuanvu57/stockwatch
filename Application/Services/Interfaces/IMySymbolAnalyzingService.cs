using Application.Dtos;
using Domain.Entities;

namespace Application.Services.Interfaces
{
    public interface IMySymbolAnalyzingService
    {
        public Task Analyze(StockPriceInRealtimeDto stockPrice, ReferenceSymbolEntity targetSymbol);
    }
}
