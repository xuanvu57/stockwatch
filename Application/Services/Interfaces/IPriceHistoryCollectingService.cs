using Application.Dtos;

namespace Application.Services.Interfaces
{
    public interface IPriceHistoryCollectingService
    {
        public Task<Dictionary<string, IEnumerable<StockPriceBaseData>>> GetByMarket(string market, int months);
        public Task<Dictionary<string, IEnumerable<StockPriceBaseData>>> GetBySymbols(IEnumerable<string> symbolIds, int months);
    }
}
