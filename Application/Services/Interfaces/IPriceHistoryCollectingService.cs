using Application.Dtos;

namespace Application.Services.Interfaces
{
    public interface IPriceHistoryCollectingService
    {
        public Task<Dictionary<string, IEnumerable<StockPriceHistoryDto>>> GetByMarket(string market, int months);
        public Task<Dictionary<string, IEnumerable<StockPriceHistoryDto>>> GetBySymbols(IEnumerable<string> symbolIds, int months);
    }
}
