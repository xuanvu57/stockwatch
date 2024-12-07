using Application.Dtos;
using Application.Dtos.Responses;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities;

namespace Application.Services.Abstracts
{
    public abstract class AbstractStockDataService(IPriceHistoryRepository priceHistoryRepository, ILatestPriceRepository latestPriceRepository) : IStockDataService
    {
        public async Task<StockWatchResponse> GetBySymbolId(string symbolId)
        {
            var stockPriceData = await GetCurrentPriceBySymbolId(symbolId);
            var latestPriceInMemory = await latestPriceRepository.Get(symbolId);

            if (stockPriceData is not null)
            {
                if (HasPriceChanged(stockPriceData, latestPriceInMemory))
                {
                    await SaveStockPrice(stockPriceData);
                }
            }
            else
            {
                stockPriceData = ConvertFromLatestStockPrice(latestPriceInMemory);
            }

            return ConvertToResponse(stockPriceData);
        }

        protected abstract Task<StockPriceData?> GetCurrentPriceBySymbolId(string symbolId);

        protected static DateOnly GetLatestAvailableDate()
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

        private async Task SaveStockPrice(StockPriceData stockPriceData)
        {
            await priceHistoryRepository.Save(new()
            {
                SymbolId = stockPriceData.SymbolId,
                Price = stockPriceData.Price,
                PriceChange = stockPriceData.PriceChange,
                PriceChangeInPercentage = stockPriceData.PriceChangeInPercentage,
                RefPrice = stockPriceData.RefPrice,
                AtTime = stockPriceData.AtTime,
            });

            await UpdateLatestPrice(stockPriceData);
        }

        private async Task UpdateLatestPrice(StockPriceData stockPriceData)
        {
            await latestPriceRepository.Save(new()
            {
                SymbolId = stockPriceData.SymbolId,
                Price = stockPriceData.Price,
                PriceChange = stockPriceData.PriceChange,
                PriceChangeInPercentage = stockPriceData.PriceChangeInPercentage,
                AtTime = stockPriceData.AtTime,
            });
        }

        private static bool HasPriceChanged(StockPriceData stockPrice, LatestPriceEntity? latestPrice)
        {
            if (latestPrice is null)
                return true;

            return stockPrice.Price != latestPrice.Price;
        }

        private static StockPriceData? ConvertFromLatestStockPrice(LatestPriceEntity? latestPrice)
        {
            if (latestPrice is not null)
            {
                return new()
                {
                    SymbolId = latestPrice.SymbolId,
                    Price = latestPrice.Price,
                    PriceChange = latestPrice.PriceChange,
                    PriceChangeInPercentage = latestPrice.PriceChangeInPercentage,
                    AtTime = latestPrice.AtTime,
                };
            }

            return null;
        }

        private static StockWatchResponse ConvertToResponse(StockPriceData? stockPrice)
        {
            return new()
            {
                Time = DateTime.Now,
                Symbols = stockPrice is null ? [] : [stockPrice]
            };
        }
    }
}
