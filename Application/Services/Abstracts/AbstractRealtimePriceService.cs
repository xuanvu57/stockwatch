using Application.Dtos;
using Application.Dtos.Bases;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Repositories.Interfaces;

namespace Application.Services.Abstracts
{
    public abstract class AbstractRealtimePriceService(
        IDateTimeService dateTimeService,
        ILatestPriceRepository latestPriceRepository) : IRealtimePriceService
    {
        protected readonly ILatestPriceRepository latestPriceRepository = latestPriceRepository;
        protected readonly IDateTimeService dateTimeService = dateTimeService;

        public async Task<BaseResponse<StockPriceInRealtimeDto>> GetBySymbolId(string symbolId)
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
                currentPriceInMarket = StockPriceInRealtimeDto.FromLatestPriceEntity(latestPriceInMemory);
            }

            return await ConvertToResponse(currentPriceInMarket);
        }

        protected abstract Task<StockPriceInRealtimeDto?> GetCurrentPriceInMarketBySymbolId(string symbolId);
        public virtual Task ConnectAsync() => Task.CompletedTask;
        public virtual Task DisconnectAsync() => Task.CompletedTask;

        private async Task UpdateLatestPriceInDB(StockPriceInRealtimeDto currentPriceInMarket)
        {
            var latestPriceEntity = currentPriceInMarket.ToLatestPriceEntity();

            if (latestPriceEntity is not null)
            {
                await latestPriceRepository.Save(latestPriceEntity);
            }
        }

        private static bool HasPriceChanged(StockPriceInRealtimeDto currentPriceInMarket, LatestPriceEntity? latestPriceInDB)
        {
            if (latestPriceInDB is null)
                return true;

            return currentPriceInMarket.Price != latestPriceInDB.Price;
        }

        private async Task<BaseResponse<StockPriceInRealtimeDto>> ConvertToResponse(StockPriceInRealtimeDto? stockPrice)
        {
            var currentTime = await dateTimeService.GetCurrentSystemDateTime();
            return new()
            {
                Data = stockPrice is null ? [] : [stockPrice],
                AtTime = currentTime,
            };
        }
    }
}
