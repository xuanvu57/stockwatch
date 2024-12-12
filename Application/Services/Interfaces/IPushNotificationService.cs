using Application.Dtos;
using static Domain.Constants.StockWatchEnums;

namespace Application.Services.Interfaces
{
    public interface IPushNotificationService
    {
        public Task Notify(StockPriceInRealtime stockPrice, NotificationTypes notificationType);
    }
}
