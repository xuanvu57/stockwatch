using Application.Dtos;

namespace Application.Services.Interfaces
{
    public interface IPriceHistoryCollectingService
    {
        public Task<Dictionary<string, IEnumerable<StockPriceHistory>>> GetByMarket(string market, int months);
        public Task<Dictionary<string, IEnumerable<StockPriceHistory>>> GetBySymbols(IEnumerable<string> symbolIds, int months);
    }
}
