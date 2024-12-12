using Application.Dtos.Responses;

namespace Application.Services.Interfaces
{
    public interface IRealtimePriceService
    {
        public Task<StockWatchResponse> GetBySymbolId(string symbolId);
    }
}
