using Application.Attributes;
using Application.Dtos;
using Application.Services.Interfaces;
using Domain.Entities;
using static Domain.Constants.StockWatchEnums;

namespace Application.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class MySymbolAnalyzingService(IPushNotificationService pushNotificationService) : IMySymbolAnalyzingService
    {
        public async Task Analyze(StockPriceInRealtime stockPrice, ReferenceSymbolEntity targetSymbol)
        {
            var floorPrice = targetSymbol.InitializedPrice * (1 - (targetSymbol.FloorPricePercentage / 100));
            var ceilingPrice = targetSymbol.InitializedPrice * (1 + (targetSymbol.CeilingPricePercentage / 100));

            if (stockPrice.Price > ceilingPrice)
            {
                await pushNotificationService.Notify(stockPrice, NotificationTypes.Up);
            }
            else if (stockPrice.Price < floorPrice)
            {
                await pushNotificationService.Notify(stockPrice, NotificationTypes.Down);
            }
        }
    }
}
