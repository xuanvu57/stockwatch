using Application.Attributes;
using Application.Dtos;
using Application.Services.Interfaces;
using Domain.Entities;
using static Application.Constants.ApplicationEnums;
using static Domain.Constants.StockWatchEnums;

namespace Application.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class MySymbolAnalyzingService(IPushNotificationService pushNotificationService) : IMySymbolAnalyzingService
    {
        public async Task Analyze(StockPriceInRealtimeDto stockPrice, ReferenceSymbolEntity targetSymbol)
        {
            var floorPrice = targetSymbol.InitializedPrice * (1 - (targetSymbol.FloorPricePercentage / 100));
            var ceilingPrice = targetSymbol.InitializedPrice * (1 + (targetSymbol.CeilingPricePercentage / 100));

            if (stockPrice.Price > ceilingPrice)
            {
                await pushNotificationService.NotifyWhenExpectationReached(stockPrice, UpDownStatus.Up);
            }
            else if (stockPrice.Price < floorPrice)
            {
                await pushNotificationService.NotifyWhenExpectationReached(stockPrice, UpDownStatus.Down);
            }
        }
    }
}
