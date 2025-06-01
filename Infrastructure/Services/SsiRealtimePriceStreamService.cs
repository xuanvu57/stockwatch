using Application.Attributes;
using Application.Dtos;
using Application.Services.Abstracts;
using Application.Services.Interfaces;
using Domain.Repositories.Interfaces;
using Infrastructure.Clients.Ssi.Constants;
using Infrastructure.Clients.Ssi.Extensions;
using Infrastructure.Clients.Ssi.Interfaces;
using Infrastructure.Clients.Ssi.Models.Responses;
using static Application.Constants.ApplicationEnums;

namespace Infrastructure.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class SsiRealtimePriceStreamService : AbstractRealtimePriceService
    {
        private readonly ISsiStreamClient ssiStreamClient;
        private StockPriceInRealtimeDto? realtimeStockPriceDto;

        public SsiRealtimePriceStreamService(
            ISsiStreamClient ssiStreamClient,
            IDateTimeService dateTimeService,
            ILatestPriceRepository latestPriceRepository) : base(dateTimeService, latestPriceRepository)
        {
            this.ssiStreamClient = ssiStreamClient;

            this.ssiStreamClient.OnDataReceivedHandler += SsiStreamClient_OnDataReceivedHandler;
        }

        override protected async Task<StockPriceInRealtimeDto?> GetCurrentPriceInMarketBySymbolId(string symbolId)
        {
            return await GetCurrentPriceBySymbolIdFromSsi(symbolId);
        }

        public override async Task ConnectAsync()
        {
            await ssiStreamClient.ConnectAsync();
        }

        public override Task DisconnectAsync()
        {
            ssiStreamClient.Disconnect();
            return Task.CompletedTask;
        }

        private async Task<StockPriceInRealtimeDto?> GetCurrentPriceBySymbolIdFromSsi(string symbolId)
        {
            realtimeStockPriceDto = null;

            var xTradehannel = $"{SsiConstants.StreamChannels.X_Trade}:{symbolId}";
            await ssiStreamClient.SwitchChannel(xTradehannel);

            var secondToWaitForResponse = 5;
            while (realtimeStockPriceDto is null && secondToWaitForResponse > 0)
            {
                Thread.Sleep(1000);
                secondToWaitForResponse--;
            }

            return realtimeStockPriceDto;
        }

        private async Task SsiStreamClient_OnDataReceivedHandler(string data)
        {
            var broadcastResponse = data.ConvertToStreamResponse<BroadcastResponse>();
            var broadcastXTradeResponse = broadcastResponse.Content.ConvertToStreamResponse<BroadcastXTradeResponse>();
            realtimeStockPriceDto = await Convert(broadcastXTradeResponse);
        }

        private async Task<StockPriceInRealtimeDto> Convert(BroadcastXTradeResponse broadcastXTradeResponse)
        {
            var currentTime = await dateTimeService.GetCurrentSystemDateTime();

            return new()
            {
                SymbolId = broadcastXTradeResponse.Symbol,
                Price = broadcastXTradeResponse.LastPrice,
                RefPrice = broadcastXTradeResponse.RefPrice,
                HighestPrice = broadcastXTradeResponse.Highest,
                LowestPrice = broadcastXTradeResponse.Lowest,
                AtTime = currentTime
            };
        }
    }
}
