using Application.Attributes;
using Application.Dtos;
using Application.Repositories.Interfaces;
using Application.Services;
using Application.Services.Abstracts;
using Infrastructure.Clients.Ssi.Constants;
using Infrastructure.Clients.Ssi.Interfaces;
using Infrastructure.Clients.Ssi.Models;
using static Domain.Constants.StockWatchEnums;

namespace Infrastructure.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class SsiStockDataService(ISsiClient ssiClient, IPriceHistoryRepository priceHistoryRepository, ILatestPriceRepository latestPriceRepository) :
        AbstractStockDataService(priceHistoryRepository, latestPriceRepository)
    {
        override protected async Task<StockPriceData?> GetCurrentPriceBySymbolId(string symbolId)
        {
            return await GetCurrentPriceBySymbolIdFromSsi(symbolId);
        }

        private async Task<StockPriceData?> GetCurrentPriceBySymbolIdFromSsi(string symbolId)
        {
            var currentDate = StockRulesService.GetLatestAvailableDate();

            var intradayOhlc = await ssiClient.IntradayOhlc(currentDate, currentDate, symbol: symbolId, pageIndex: 1, pageSize: 1);

            if (intradayOhlc.Status == SsiConstants.ResponseStatus.Success)
            {
                var refPrice = await GetReferencePrice(symbolId, currentDate);

                return Convert(intradayOhlc.Data![0], refPrice);
            }

            return null;
        }

        private async Task<decimal?> GetReferencePrice(string symbolId, DateOnly currentDate)
        {
            var latestPriceInMemory = await latestPriceRepository.Get(symbolId);

            if (latestPriceInMemory is not null)
            {
                return latestPriceInMemory.RefPrice;
            }

            var dailyStockPrice = await ssiClient.DailyStockPrice(currentDate, currentDate, symbol: symbolId, pageIndex: 1, pageSize: 1);
            if (dailyStockPrice.Status == SsiConstants.ResponseStatus.Success)
            {
                return decimal.Parse(dailyStockPrice.Data![0].RefPrice);
            }
            else
            {
                return null;
            }
        }

        private static StockPriceData Convert(IntradayOhlcResponse intradayOhlcResponse, decimal? refPrice)
        {
            return new()
            {
                SymbolId = intradayOhlcResponse.Symbol,
                Price = intradayOhlcResponse.Value,
                RefPrice = refPrice,
                HighestPrice = intradayOhlcResponse.High,
                LowestPrice = intradayOhlcResponse.Low,
                AtTime = DateTime.Now
            };
        }
    }
}
