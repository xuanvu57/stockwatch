using Application.Dtos;

namespace Application.Services.Interfaces
{
    public interface IStockDataService
    {
        public Task<StockWatchResponse> GetAll();
        public Task<StockWatchResponse> GetBySymbolId(string symbolId);
    }
}
