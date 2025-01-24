using Application.Dtos;
using Application.Dtos.Bases;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Repositories.Interfaces;

namespace Application.Services.Abstracts
{
    public abstract class AbstractRealtimePriceService(
        ILatestPriceRepository latestPriceRepository) : IRealtimePriceService
    {
        protected readonly ILatestPriceRepository latestPriceRepository = latestPriceRepository;

        public async Task<BaseResponse<StockPriceInRealtime>> GetBySymbolId(string symbolId)
        {
            var currentPriceInMarket = await GetCurrentPriceInMarketBySymbolId(symbolId);
            var latestPriceInMemory = await latestPriceRepository.Get(symbolId);

            if (currentPriceInMarket is not null)
            {
                if (HasPriceChanged(currentPriceInMarket, latestPriceInMemory))
                {
                    await UpdateLatestPriceInDB(currentPriceInMarket);
                }
            }
            else
            {
                currentPriceInMarket = StockPriceInRealtime.ConvertFromLatestStockPrice(latestPriceInMemory);
            }

            return ConvertToResponse(currentPriceInMarket);
        }

        protected abstract Task<StockPriceInRealtime?> GetCurrentPriceInMarketBySymbolId(string symbolId);

        private async Task UpdateLatestPriceInDB(StockPriceInRealtime currentPriceInMarket)
        {
            var latestPricess = currentPriceInMarket.ToLatestPriceEntity();

            if (latestPricess is not null)
            {
                await latestPriceRepository.Save(latestPricess);
            }
        }

        private static bool HasPriceChanged(StockPriceInRealtime currentPriceInMarket, LatestPriceEntity? latestPriceInDB)
        {
            if (latestPriceInDB is null)
                return true;

            return currentPriceInMarket.Price != latestPriceInDB.Price;
        }

        private static BaseResponse<StockPriceInRealtime> ConvertToResponse(StockPriceInRealtime? stockPrice)
        {
            return new()
            {
                Data = stockPrice is null ? [] : [stockPrice]
            };
        }
    }
}
