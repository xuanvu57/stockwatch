using Application.Attributes;
using Application.Dtos;
using Application.Services.Abstracts;
using Application.Services.Interfaces;
using Domain.Repositories.Interfaces;
using Domain.Services;
using Infrastructure.Clients.Ssi.Constants;
using Infrastructure.Clients.Ssi.Interfaces;
using Infrastructure.Clients.Ssi.Models.Responses;
using static Application.Constants.ApplicationEnums;

namespace Infrastructure.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class SsiRealtimePriceService(
        ISsiClient ssiClient,
        IDateTimeService dateTimeService,
        ILatestPriceRepository latestPriceRepository) : AbstractRealtimePriceService(dateTimeService, latestPriceRepository)
    {
        override protected async Task<StockPriceInRealtimeDto?> GetCurrentPriceInMarketBySymbolId(string symbolId)
        {
            return await GetCurrentPriceBySymbolIdFromSsi(symbolId);
        }

        private async Task<StockPriceInRealtimeDto?> GetCurrentPriceBySymbolIdFromSsi(string symbolId)
        {
            var today = await dateTimeService.GetCurrentBusinessDateTime();
            var latestDate = StockRulesService.GetLatestAvailableDate(today);

            var intradayOhlc = await ssiClient.IntradayOhlc(latestDate, latestDate, symbolId, pageIndex: 1, pageSize: 1);

            if (intradayOhlc.Status == SsiConstants.ResponseStatus.Success)
            {
                var refPrice = await GetReferencePrice(symbolId, latestDate);

                return await Convert(intradayOhlc.Data![0], refPrice);
            }

            return null;
        }

        private async Task<decimal?> GetReferencePrice(string symbolId, DateOnly date)
        {
            var latestPriceInMemory = await latestPriceRepository.Get(symbolId);

            if (latestPriceInMemory is not null)
            {
                return latestPriceInMemory.RefPrice;
            }

            var previousDate = StockRulesService.GetLatestAvailableDate(date.AddDays(-1));
            var dailyStockPriceInPreviousDate = await ssiClient.DailyStockPrice(previousDate, previousDate, symbolId, pageIndex: 1, pageSize: 1);
            if (dailyStockPriceInPreviousDate.Status == SsiConstants.ResponseStatus.Success)
            {
                return decimal.Parse(dailyStockPriceInPreviousDate.Data![0].ClosePriceAdjusted);
            }
            else
            {
                return null;
            }
        }

        private async Task<StockPriceInRealtimeDto> Convert(IntradayOhlcResponse intradayOhlcResponse, decimal? refPrice)
        {
            var currentTime = await dateTimeService.GetCurrentSystemDateTime();

            return new()
            {
                SymbolId = intradayOhlcResponse.Symbol,
                Price = decimal.Parse(intradayOhlcResponse.Value),
                RefPrice = refPrice,
                HighestPrice = decimal.Parse(intradayOhlcResponse.High),
                LowestPrice = decimal.Parse(intradayOhlcResponse.Low),
                AtTime = currentTime
            };
        }
    }
}
