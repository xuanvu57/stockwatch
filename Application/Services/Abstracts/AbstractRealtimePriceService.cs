using Application.Dtos;
using Application.Dtos.Responses;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities;

namespace Application.Services.Abstracts
{
    public abstract class AbstractRealtimePriceService(
        IRealtimePriceHistoryRepository realtimePriceHistoryRepository,
        ILatestPriceRepository latestPriceRepository) : IRealtimePriceService
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
                stockPriceData = StockPriceInRealtime.ConvertFromLatestStockPrice(latestPriceInMemory);
            }

            return ConvertToResponse(stockPriceData);
        }

        protected abstract Task<StockPriceInRealtime?> GetCurrentPriceBySymbolId(string symbolId);

        private async Task SaveStockPrice(StockPriceInRealtime stockPriceData)
        {
            var priceHistory = stockPriceData.ToPriceHistoryEntity();

            await realtimePriceHistoryRepository.Save(priceHistory);

            await UpdateLatestPrice(stockPriceData);
        }

        private async Task UpdateLatestPrice(StockPriceInRealtime stockPriceData)
        {
            var latestPricess = stockPriceData.ToLatestPriceEntity();

            if (latestPricess is not null)
            {
                await latestPriceRepository.Save(latestPricess);
            }
        }

        private static bool HasPriceChanged(StockPriceInRealtime stockPrice, LatestPriceEntity? latestPrice)
        {
            if (latestPrice is null)
                return true;

            return stockPrice.Price != latestPrice.Price;
        }

        private static StockWatchResponse ConvertToResponse(StockPriceInRealtime? stockPrice)
        {
            return new()
            {
                Time = DateTime.Now,
                Symbols = stockPrice is null ? [] : [stockPrice]
            };
        }
    }
}
