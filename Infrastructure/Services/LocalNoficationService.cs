using Application.Attributes;
using Application.Dtos;
using Application.Services.Interfaces;
using Plugin.LocalNotification;
using static Domain.Constants.StockWatchEnums;

namespace Infrastructure.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class LocalNoficationService : IPushNotificationService
    {
        public async Task Notify(SymbolInfo symbol, NotificationTypes notificationType)
        {
            var notificationId = 1000;
            var title = notificationType == NotificationTypes.Up ?
                $"{symbol.SymbolId}'s price is UP over the expectation" :
                $"{symbol.SymbolId}'s price is DOWN over the expectation";

            var request = new NotificationRequest
            {
                NotificationId = notificationId,
                Title = title,
                Subtitle = "You should be noticed",
                Description = $"It was {symbol.Price:N2} at {DateTime.Now:HH:mm:ss}",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(1),
                }
            };

            LocalNotificationCenter.Current.Cancel(notificationId);
            await LocalNotificationCenter.Current.Show(request);
        }
    }
}
