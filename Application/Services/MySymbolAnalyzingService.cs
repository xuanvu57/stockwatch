using Application.Attributes;
using Application.Dtos;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Services;
using static Application.Constants.ApplicationEnums;
using static Domain.Constants.StockWatchEnums;

namespace Application.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class MySymbolAnalyzingService(IPushNotificationService pushNotificationService) : IMySymbolAnalyzingService
    {
        public async Task Analyze(StockPriceInRealtimeDto stockPrice, ReferenceSymbolEntity targetSymbol)
        {
            var percentage = StockRulesService.CalculatePercentage(stockPrice.Price, targetSymbol.InitializedPrice);

            if (percentage > targetSymbol.CeilingPricePercentage)
            {
                await pushNotificationService.NotifyWhenExpectationReached(stockPrice, UpDownStatus.Up);
            }
            else if (percentage < targetSymbol.FloorPricePercentage)
            {
                await pushNotificationService.NotifyWhenExpectationReached(stockPrice, UpDownStatus.Down);
            }
        }
    }
}
