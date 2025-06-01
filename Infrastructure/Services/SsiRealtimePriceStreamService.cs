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
    [DIService(DIServiceLifetime.Singleton)]
    public class SsiRealtimePriceStreamService(
            ISsiStreamClient ssiStreamClient,
            IDateTimeService dateTimeService,
            ILatestPriceRepository latestPriceRepository) : AbstractRealtimePriceService(dateTimeService, latestPriceRepository)
    {
        private const int MaxSecondToWaitForStreamEvent = 5;

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
            var taskCompletionSource = new TaskCompletionSource<StockPriceInRealtimeDto?>();

            async Task OnDataReceivedHandler(string data)
            {
                ssiStreamClient.OnDataReceivedHandler -= OnDataReceivedHandler;

                var broadcastResponse = data.ConvertToStreamResponse<BroadcastResponse>();
                var broadcastXTradeResponse = broadcastResponse.Content.ConvertToStreamResponse<BroadcastXTradeResponse>();
                var realtimeStockPriceDto = await Convert(broadcastXTradeResponse);

                taskCompletionSource.TrySetResult(realtimeStockPriceDto);
            }

            ssiStreamClient.OnDataReceivedHandler += OnDataReceivedHandler;

            var xTradehannel = $"{SsiConstants.StreamChannels.X_Trade}:{symbolId}";
            await ssiStreamClient.SwitchChannel(xTradehannel);

            var completedTask = await Task.WhenAny(taskCompletionSource.Task, Task.Delay(TimeSpan.FromSeconds(MaxSecondToWaitForStreamEvent)));
            return completedTask == taskCompletionSource.Task ? taskCompletionSource.Task.Result : null;
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
