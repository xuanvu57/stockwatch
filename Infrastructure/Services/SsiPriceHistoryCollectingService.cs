using Application.Attributes;
using Application.Dtos;
using Application.Services;
using Application.Services.Interfaces;
using Infrastructure.Clients.Ssi.Constants;
using Infrastructure.Clients.Ssi.Interfaces;
using System.Globalization;
using static Domain.Constants.StockWatchEnums;

namespace Infrastructure.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class SsiPriceHistoryCollectingService(ILoadingService loadingService, ISsiClient ssiClient) : IPriceHistoryCollectingService
    {
        public async Task<Dictionary<string, IEnumerable<StockPriceHistory>>> GetByMarket(string market, int months)
        {
            var stockPriceHistoryByMarket = new Dictionary<string, IEnumerable<StockPriceHistory>>();

            // TODO: debugging, add fix number
            var pageIndex = 1_000_000;      //SsiConstants.Request.DefaultPageIndex;
            var pageSize = 10;              //SsiConstants.Request.DefaultPageSize;

            var response = await ssiClient.Securities(market, 1, pageSize);
            while (response.Status == SsiConstants.ResponseStatus.Success &&
                response.Data is not null &&
                response.Data.Length > 0)
            {
                var stockPrices = await GetBySymbols(response.Data.Select(x => x.Symbol), months);
                foreach (var key in stockPrices.Keys)
                {
                    stockPriceHistoryByMarket.Add(key, stockPrices[key]);
                }

                pageIndex++;
                if (pageIndex * pageSize > response.TotalRecord)
                    break;

                response = await ssiClient.Securities(market, pageIndex, pageSize);
            }

            return stockPriceHistoryByMarket;
        }

        public async Task<Dictionary<string, IEnumerable<StockPriceHistory>>> GetBySymbols(IEnumerable<string> symbolIds, int months)
        {
            var stockPrices = new Dictionary<string, IEnumerable<StockPriceHistory>>();

            foreach (var symbolId in symbolIds)
            {
                var stockPriceHistoryBySymbol = await GetBySymbol(symbolId, months);

                stockPrices.Add(symbolId, stockPriceHistoryBySymbol);
            }

            return stockPrices;
        }

        private async Task<IEnumerable<StockPriceHistory>> GetBySymbol(string symbolId, int months)
        {
            await loadingService.Show(symbolId);

            var toDate = StockRulesService.GetLatestAvailableDate();
            var fromDate = toDate.AddMonths(-1 * months);

            var stockPriceHistory = new List<StockPriceHistory>();

            var pageIndex = 1;
            var pageSize = SsiConstants.Request.DefaultPageSize;

            var response = await ssiClient.DailyOhlc(fromDate, toDate, symbolId, pageIndex, pageSize);
            while (response.Status == SsiConstants.ResponseStatus.Success &&
                response.Data is not null &&
                response.Data.Length > 0)
            {
                stockPriceHistory.AddRange(response.Data.Select(x => new StockPriceHistory()
                {
                    SymbolId = x.Symbol,
                    AtDate = DateOnly.ParseExact(x.TradingDate, SsiConstants.Format.Date, CultureInfo.InvariantCulture),
                    Price = decimal.Parse(x.Close),
                    HighestPrice = decimal.Parse(x.High),
                    LowestPrice = decimal.Parse(x.Low)
                }));

                pageIndex++;
                if (pageIndex * pageSize > response.TotalRecord)
                    break;

                response = await ssiClient.DailyOhlc(fromDate, toDate, symbolId, pageIndex, pageSize);
            }

            return stockPriceHistory;
        }
    }
}
