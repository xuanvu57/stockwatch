using Application.Dtos.Responses;

namespace Application.Services.Interfaces
{
    public interface IStockDataService
    {
        public Task<StockWatchResponse> GetBySymbolId(string symbolId);
    }
}
