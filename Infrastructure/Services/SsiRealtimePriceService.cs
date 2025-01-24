using Application.Attributes;
using Application.Dtos;
using Application.Services.Abstracts;
using Domain.Repositories.Interfaces;
using Domain.Services;
using Infrastructure.Clients.Ssi.Constants;
using Infrastructure.Clients.Ssi.Interfaces;
using Infrastructure.Clients.Ssi.Models;
using static Domain.Constants.StockWatchEnums;

namespace Infrastructure.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class SsiRealtimePriceService(
        ISsiClient ssiClient,
        ILatestPriceRepository latestPriceRepository) : AbstractRealtimePriceService(latestPriceRepository)
    {
        override protected async Task<StockPriceInRealtime?> GetCurrentPriceInMarketBySymbolId(string symbolId)
        {
            return await GetCurrentPriceBySymbolIdFromSsi(symbolId);
        }

        private async Task<StockPriceInRealtime?> GetCurrentPriceBySymbolIdFromSsi(string symbolId)
        {
            var latestDate = StockRulesService.GetLatestAvailableDate();

            var intradayOhlc = await ssiClient.IntradayOhlc(latestDate, latestDate, symbol: symbolId, pageIndex: 1, pageSize: 1);

            if (intradayOhlc.Status == SsiConstants.ResponseStatus.Success)
            {
                var refPrice = await GetReferencePrice(symbolId, latestDate);

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

        private static StockPriceInRealtime Convert(IntradayOhlcResponse intradayOhlcResponse, decimal? refPrice)
        {
            return new()
            {
                SymbolId = intradayOhlcResponse.Symbol,
                Price = decimal.Parse(intradayOhlcResponse.Value),
                RefPrice = refPrice,
                HighestPrice = decimal.Parse(intradayOhlcResponse.High),
                LowestPrice = decimal.Parse(intradayOhlcResponse.Low),
                AtTime = DateTime.Now
            };
        }
    }
}
