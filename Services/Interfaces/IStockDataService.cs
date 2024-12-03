
using stockwatch.Models.StockWatchModels;

namespace stockwatch.Services.Interfaces
{
    public interface IStockDataService
    {
        public Task<StockWatchResponse> GetAll();
        public Task<StockWatchResponse> GetBySymbolId(string symbolId);
    }
}
