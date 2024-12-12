using Application.Attributes;
using Application.Dtos;
using Application.Services;
using Application.Services.Interfaces;
using Infrastructure.Clients.Ssi.Constants;
using Infrastructure.Clients.Ssi.Interfaces;
using static Domain.Constants.StockWatchEnums;

namespace Infrastructure.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class SsiPriceHistoryCollectingService(ILoadingService loadingService, ISsiClient ssiClient) : IPriceHistoryCollectingService
    {
        public async Task<Dictionary<string, IEnumerable<StockPriceBaseData>>> GetByMarket(string market, int months)
        {
            var stockPriceDataByMarket = new Dictionary<string, IEnumerable<StockPriceBaseData>>();

            var pageIndex = 1;
            // TODO: debugging
            var pageSize = 10;
            //var pageSize = SsiConstants.Request.DefaultPageSize;

            var response = await ssiClient.Securities(market, pageIndex, pageSize);
            while (response.Status == SsiConstants.ResponseStatus.Success &&
                response.Data is not null &&
                response.Data.Length > 0)
            {
                var stockPrices = await GetBySymbols(response.Data.Select(x => x.Symbol), months);
                foreach (var key in stockPrices.Keys)
                {
                    stockPriceDataByMarket.Add(key, stockPrices[key]);
                }

                // TODO: debugging
                break;

                pageIndex++;
                if (pageIndex * pageSize > response.TotalRecord)
                    break;

                response = await ssiClient.Securities(market, pageIndex, pageSize);
            }

            return stockPriceDataByMarket;
        }

        public async Task<Dictionary<string, IEnumerable<StockPriceBaseData>>> GetBySymbols(IEnumerable<string> symbolIds, int months)
        {
            var stockPrices = new Dictionary<string, IEnumerable<StockPriceBaseData>>();

            foreach (var symbolId in symbolIds)
            {
                var stockPriceDataBySymbol = await GetBySymbol(symbolId, months);

                stockPrices.Add(symbolId, stockPriceDataBySymbol);
            }

            return stockPrices;
        }

        private async Task<IEnumerable<StockPriceBaseData>> GetBySymbol(string symbolId, int months)
        {
            await loadingService.Show(symbolId);

            var toDate = StockRulesService.GetLatestAvailableDate();
            var fromDate = toDate.AddMonths(-1 * months);

            var stockPriceBaseData = new List<StockPriceBaseData>();

            var pageIndex = 1;
            var pageSize = SsiConstants.Request.DefaultPageSize;

            var response = await ssiClient.DailyOhlc(fromDate, toDate, symbolId, pageIndex, pageSize);
            while (response.Status == SsiConstants.ResponseStatus.Success &&
                response.Data is not null &&
                response.Data.Length > 0)
            {
                stockPriceBaseData.AddRange(response.Data.Select(x => new StockPriceBaseData()
                {
                    SymbolId = x.Symbol,
                    Price = decimal.Parse(x.Close),
                    HighestPrice = decimal.Parse(x.High),
                    LowestPrice = decimal.Parse(x.Low)
                }));

                pageIndex++;
                if (pageIndex * pageSize > response.TotalRecord)
                    break;

                response = await ssiClient.DailyOhlc(fromDate, toDate, symbolId, pageIndex, pageSize);
            }

            return stockPriceBaseData;
        }
    }
}
