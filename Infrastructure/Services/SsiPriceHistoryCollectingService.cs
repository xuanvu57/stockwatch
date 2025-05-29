using Application.Attributes;
using Application.Dtos;
using Application.Services.Interfaces;
using Domain.Services;
using Infrastructure.Clients.Ssi.Constants;
using Infrastructure.Clients.Ssi.Interfaces;
using static Application.Constants.ApplicationEnums;
using static Domain.Constants.StockWatchEnums;

namespace Infrastructure.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class SsiPriceHistoryCollectingService(
        ILoadingService loadingService,
        ISsiClient ssiClient,
        IDateTimeService dateTimeService) : IPriceHistoryCollectingService
    {
        public async Task<Dictionary<string, IEnumerable<StockPriceHistoryDto>>> GetByMarket(Market market, int maxSymbolCountFromMarket, int months, bool advancedData)
        {
            var (fromDate, toDate) = await GetFromDateAndToDateByToDay(months);

            return await GetByMarket(market, maxSymbolCountFromMarket, fromDate, toDate, advancedData);
        }

        public async Task<Dictionary<string, IEnumerable<StockPriceHistoryDto>>> GetByMarket(Market market, int maxSymbolCountFromMarket, DateOnly fromDate, DateOnly toDate, bool advancedData)
        {
            var symbolIds = new List<string>();

            var pageIndex = 1;
            var pageSize = 1_000;
            var securitiesResponse = await ssiClient.Securities(market.ToString(), pageIndex, pageSize);

            while (securitiesResponse.Status == SsiConstants.ResponseStatus.Success &&
                securitiesResponse.Data is not null &&
                securitiesResponse.Data.Length > 0)
            {
                var maxSymbolCountInThisRequest = maxSymbolCountFromMarket - symbolIds.Count;

                var symbolIdsFromRequest = securitiesResponse.Data
                    .Select(x => x.Symbol)
                    .Take(maxSymbolCountInThisRequest);

                symbolIds.AddRange(symbolIdsFromRequest);

                if (symbolIds.Count >= maxSymbolCountFromMarket)
                    break;

                pageIndex++;
                securitiesResponse = await ssiClient.Securities(market.ToString(), pageIndex, pageSize);
            }

            return await GetBySymbols(symbolIds, fromDate, toDate, advancedData);
        }

        public async Task<Dictionary<string, IEnumerable<StockPriceHistoryDto>>> GetBySymbols(IEnumerable<string> symbolIds, int months, bool advancedData)
        {
            var (fromDate, toDate) = await GetFromDateAndToDateByToDay(months);

            return await GetBySymbols(symbolIds, fromDate, toDate, advancedData);
        }

        public async Task<Dictionary<string, IEnumerable<StockPriceHistoryDto>>> GetBySymbols(IEnumerable<string> symbolIds, DateOnly fromDate, DateOnly toDate, bool advancedData)
        {
            var stockPrices = new Dictionary<string, IEnumerable<StockPriceHistoryDto>>();

            foreach (var symbolId in symbolIds)
            {
                var stockPriceHistoryBySymbol = await GetBySymbol(symbolId, fromDate, toDate, advancedData);

                stockPrices.Add(symbolId, stockPriceHistoryBySymbol);
            }

            return stockPrices;
        }

        private async Task<IEnumerable<StockPriceHistoryDto>> GetBySymbol(string symbolId, DateOnly fromDate, DateOnly toDate, bool advancedData)
        {
            await loadingService.Show(symbolId);

            return advancedData
                ? await GetDailyPrice(symbolId, fromDate, toDate)
                : await GetDailyOhlcOnly(symbolId, fromDate, toDate);
        }

        private async Task<IEnumerable<StockPriceHistoryDto>> GetDailyPrice(string symbolId, DateOnly fromDate, DateOnly toDate)
        {
            var stockPriceHistory = new List<StockPriceHistoryDto>();

            var pageIndex = 1;
            var pageSize = SsiConstants.Request.DefaultPageSize;

            var response = await ssiClient.DailyStockPrice(fromDate, toDate, symbolId, pageIndex, pageSize);
            while (response.Status == SsiConstants.ResponseStatus.Success &&
                response.Data is not null &&
                response.Data.Length > 0)
            {
                stockPriceHistory.AddRange(response.Data.Select(x => x.ToStockPriceHistoryDto()));

                pageIndex++;
                if (pageIndex * pageSize > response.TotalRecord)
                    break;

                response = await ssiClient.DailyStockPrice(fromDate, toDate, symbolId, pageIndex, pageSize);
            }

            return stockPriceHistory;
        }

        private async Task<IEnumerable<StockPriceHistoryDto>> GetDailyOhlcOnly(string symbolId, DateOnly fromDate, DateOnly toDate)
        {
            var stockPriceHistory = new List<StockPriceHistoryDto>();

            var pageIndex = 1;
            var pageSize = SsiConstants.Request.DefaultPageSize;

            var response = await ssiClient.DailyOhlc(fromDate, toDate, symbolId, pageIndex, pageSize);
            while (response.Status == SsiConstants.ResponseStatus.Success &&
                response.Data is not null &&
                response.Data.Length > 0)
            {
                stockPriceHistory.AddRange(response.Data.Select(x => x.ToStockPriceHistoryDto()));

                pageIndex++;
                if (pageIndex * pageSize > response.TotalRecord)
                    break;

                response = await ssiClient.DailyOhlc(fromDate, toDate, symbolId, pageIndex, pageSize);
            }

            return stockPriceHistory;
        }

        private async Task<(DateOnly, DateOnly)> GetFromDateAndToDateByToDay(int months)
        {
            var today = await dateTimeService.GetCurrentBusinessDateTime();

            var toDate = StockRulesService.GetLatestAvailableDate(today);
            var fromDate = toDate.AddMonths(-1 * months);

            return (fromDate, toDate);
        }
    }
}
