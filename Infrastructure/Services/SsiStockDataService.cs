using Application.Attributes;
using Application.Dtos;
using Application.Repositories.Interfaces;
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
            return await GetStockDataFromSsi(symbolId);
        }

        private async Task<StockPriceData?> GetStockDataFromSsi(string symbolId)
        {
            var currentDate = GetLatestAvailableDate();

            var stockData = await ssiClient.DailyStockPrice(currentDate, currentDate, symbolId);

            if (stockData.Status == SsiConstants.ResponseStatus.Success)
            {
                var stockPrice = stockData.Data![0];

                return Convert(stockPrice);
            }

            return null;
        }

        private static StockPriceData Convert(DailyStockPriceResponse stockPrice)
        {
            return new()
            {
                SymbolId = stockPrice.Symbol,
                Price = decimal.Parse(stockPrice.RefPrice) + decimal.Parse(stockPrice.PriceChange),
                PriceChange = decimal.Parse(stockPrice.PriceChange),
                PriceChangeInPercentage = decimal.Parse(stockPrice.PerPriceChange),
                RefPrice = decimal.Parse(stockPrice.RefPrice),
                AtTime = DateTime.Now
            };
        }
    }
}
