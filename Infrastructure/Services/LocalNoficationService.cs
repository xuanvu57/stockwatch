using Application.Attributes;
using Application.Services.Abstracts;
using Application.Services.Interfaces;
using Plugin.LocalNotification;
using static Application.Constants.ApplicationEnums;

namespace Infrastructure.Services
{
    [DIService(DIServiceLifetime.Singleton)]
    public class LocalNoficationService(IMessageService messageService) : AbstractPushNotificationService(messageService)
    {
        protected override async Task SendNotification(int notificationId, string title, string subtitle, string description)
        {
            const int delaySecondBeforeSendNotification = 1;

            var request = new NotificationRequest
            {
                NotificationId = notificationId,
                Title = title,
                Subtitle = subtitle,
                Description = description,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(delaySecondBeforeSendNotification),
                }
            };

            LocalNotificationCenter.Current.Cancel(notificationId);
            await LocalNotificationCenter.Current.Show(request);
        }
    }
}
