using Application.Attributes;
using Application.Dtos;
using Application.Services.Interfaces;
using Application.Settings;
using Domain.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static Application.Constants.ApplicationEnums;

namespace Application.Services
{
    [DIService(DIServiceLifetime.Singleton)]
    public class BackgroundService(
        ILogger<BackgroundService> logger,
        IToastManagerService toastManagerService,
        IConfiguration configuration,
        IRealtimePriceService realTimePriceService,
        IMySymbolAnalyzingService mySymbolAnalyzingService,
        IReferenceSymbolRepository referenceSymbolRepository) : IBackgroundService
    {
        private Timer? timer;
        private readonly List<IBackgroundServiceSubscriber> subscribers = [];
        public bool IsRunning { get; private set; } = false;

        public void Start()
        {
            var scheduleSettings = configuration.GetRequiredSection(nameof(ScheduleSettings)).Get<ScheduleSettings>()!;

            timer ??= new(HandleTimerCallback);
            timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(scheduleSettings.FetchDataIntervalInSecond));
            IsRunning = true;
        }

        public void Stop()
        {
            timer?.Change(Timeout.Infinite, 0);
            IsRunning = false;
        }

        public void Subscribe(IBackgroundServiceSubscriber subscriber)
        {
            if (!subscribers.Contains(subscriber))
            {
                subscribers.Add(subscriber);
            }
        }

        public void Unsubscribe(IBackgroundServiceSubscriber subscriber)
        {
            subscribers.Remove(subscriber);
        }

        private void HandleTimerCallback(object? state)
        {
            _ = DoAnalyzeMySymbol();
        }

        private async Task DoAnalyzeMySymbol()
        {
            try
            {
                var targetSymbol = await referenceSymbolRepository.Get();
                if (targetSymbol is null)
                {
                    return;
                }

                var stockPrice = await realTimePriceService.GetBySymbolId(targetSymbol.Id);
                if (stockPrice.Data.Any())
                {
                    await mySymbolAnalyzingService.Analyze(stockPrice.Data.First(), targetSymbol);
                }

                var symbolAnalyzingResult = SymbolAnalyzingResultDto.FromStockPriceInRealtimeDto(
                    stockPrice.Data.FirstOrDefault(),
                    targetSymbol,
                    stockPrice.AtTime);

                await NotifyToSubscribers(symbolAnalyzingResult);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error run my symbol analyzation");

                if (subscribers.Count > 0)
                {
                    await toastManagerService.Show(ex.Message);
                }
            }
        }

        private async Task NotifyToSubscribers(SymbolAnalyzingResultDto symbolAnalyzingResult)
        {
            foreach (var subscriber in subscribers)
            {
                await subscriber.HandleBackgroundServiceEvent(symbolAnalyzingResult);
            }
        }
    }
}
