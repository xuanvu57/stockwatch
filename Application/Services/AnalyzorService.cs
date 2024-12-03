using Application.Attributes;
using Application.Dtos;
using Application.Services.Interfaces;
using Domain.Entities;
using static Domain.Constants.StockWatchEnums;

namespace Application.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class AnalyzorService(IPushNotificationService pushNotificationService) : IAnalyzorService
    {
        public async Task Analyze(SymbolInfo symbol, ReferenceSymbolEntity targetSymbol)
        {
            var floorPrice = targetSymbol.Price * (1 - (targetSymbol.FloorPrice / 100));
            var ceilingPrice = targetSymbol.Price * (1 + (targetSymbol.CeilingPrice / 100));

            if (symbol.Price > ceilingPrice)
            {
                await pushNotificationService.Notify(symbol, NotificationTypes.Up);
            }
            else if (symbol.Price < floorPrice)
            {
                await pushNotificationService.Notify(symbol, NotificationTypes.Down);
            }
        }
    }
}
