using Application.Dtos;
using static Domain.Constants.StockWatchEnums;

namespace Application.Services.Interfaces
{
    public interface IPriceHistoryCollectingService
    {
        public Task<Dictionary<string, IEnumerable<StockPriceHistoryDto>>> GetByMarket(Market market, int maxSymbolCountFromMarket, int months);
        public Task<Dictionary<string, IEnumerable<StockPriceHistoryDto>>> GetBySymbols(IEnumerable<string> symbolIds, int months);
    }
}
