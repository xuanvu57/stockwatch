using stockwatch.Attributes;
using stockwatch.Clients.Ssi.Constants;
using stockwatch.Clients.Ssi.Interfaces;
using stockwatch.Models.StockWatchModels;
using stockwatch.Services.Interfaces;
using static stockwatch.Constants.StockWatchEnums;

namespace stockwatch.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class SsiStockDataService(ISsiClient ssiClient) : IStockDataService
    {
        public async Task<StockWatchResponse> GetAll()
        {
            var symbols = await GetStockDataFromSsi(string.Empty);

            return Convert(symbols);
        }

        public async Task<StockWatchResponse> GetBySymbolId(string symbolId)
        {
            var symbols = await GetStockDataFromSsi(symbolId);

            return Convert(symbols);
        }

        private async Task<IEnumerable<SymbolInfo>> GetStockDataFromSsi(string symbolId)
        {
            var currentDate = GetLatestAvailableDate();

            var stockData = await ssiClient.DailyStockPrice(currentDate, currentDate, symbolId);

            if (stockData.Status == SsiConstants.ResponseStatus.Success)
            {
                return stockData.Data!.Select(x => new SymbolInfo()
                {
                    SymbolId = x.Symbol,
                    Price = decimal.Parse(x.RefPrice) + decimal.Parse(x.PriceChange)
                });
            }

            return [];
        }

        private static DateOnly GetLatestAvailableDate()
        {
            var currentDate = DateTime.Now;

            var latestAvailableDate = currentDate.DayOfWeek switch
            {
                DayOfWeek.Saturday => currentDate.AddDays(-1),
                DayOfWeek.Sunday => currentDate.AddDays(-2),
                _ => currentDate
            };

            return DateOnly.FromDateTime(latestAvailableDate);
        }

        private static StockWatchResponse Convert(IEnumerable<SymbolInfo> symbols)
        {
            return new StockWatchResponse()
            {
                Time = DateTime.Now,
                Symbols = symbols
            };
        }
    }
}
