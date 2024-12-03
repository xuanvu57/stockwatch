using stockwatch.Attributes;
using stockwatch.Models;
using stockwatch.Models.StockWatchModels;
using stockwatch.Services.Interfaces;
using static stockwatch.Constants.StockWatchEnums;

namespace stockwatch.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class AnalyzorService(IPushNotificationService pushNotificationService) : IAnalyzorService
    {
        public async Task Analyze(SymbolInfo symbol, ReferenceSymbolInfo targetSymbol)
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
