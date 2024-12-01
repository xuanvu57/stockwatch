using stockwatch.Models.StockWatchModels;
using static stockwatch.Constants.StockWatchEnums;

namespace stockwatch.Services.Interfaces
{
    public interface IPushNotificationService
    {
        public Task Notify(SymbolInfo symbol, NotificationTypes notificationType);
    }
}
