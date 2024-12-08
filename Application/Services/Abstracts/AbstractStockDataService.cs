using Application.Dtos;
using Application.Dtos.Responses;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities;

namespace Application.Services.Abstracts
{
    public abstract class AbstractStockDataService(IPriceHistoryRepository priceHistoryRepository, ILatestPriceRepository latestPriceRepository) : IStockDataService
    {
        protected readonly ILatestPriceRepository latestPriceRepository = latestPriceRepository;

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

        private async Task SaveStockPrice(StockPriceData stockPriceData)
        {
            await priceHistoryRepository.Save(new()
            {
                SymbolId = stockPriceData.SymbolId,
                Price = stockPriceData.Price,
                PriceChange = stockPriceData.RefPrice is null ? null : stockPriceData.Price - stockPriceData.RefPrice.Value,
                PriceChangeInPercentage = stockPriceData.RefPrice is null ? null : ((stockPriceData.Price / stockPriceData.RefPrice.Value) - 1),
                RefPrice = stockPriceData.RefPrice,
                AtTime = stockPriceData.AtTime,
                HighestPrice = stockPriceData.HighestPrice,
                LowestPrice = stockPriceData.LowestPrice
            });

            await UpdateLatestPrice(stockPriceData);
        }

        private async Task UpdateLatestPrice(StockPriceData stockPriceData)
        {
            if (stockPriceData.RefPrice is null)
            {
                return;
            }

            await latestPriceRepository.Save(new()
            {
                SymbolId = stockPriceData.SymbolId,
                Price = stockPriceData.Price,
                RefPrice = stockPriceData.RefPrice.Value,
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
            return latestPrice is null ?
                null :
                new()
                {
                    SymbolId = latestPrice.SymbolId,
                    Price = latestPrice.Price,
                    HighestPrice = 0,
                    LowestPrice = 0,
                    AtTime = latestPrice.AtTime,
                };
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
