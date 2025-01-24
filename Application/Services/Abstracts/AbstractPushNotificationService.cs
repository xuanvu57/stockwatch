using Application.Dtos;
using Application.Services.Interfaces;
using Domain.Constants;
using static Domain.Constants.StockWatchEnums;

namespace Application.Services.Abstracts
{
    public abstract class AbstractPushNotificationService(IMessageService messageService) : IPushNotificationService
    {
        private const int NotificationIdWhenExpectationReached = 1000;

        public async Task NotifyWhenExpectationReached(StockPriceInRealtimeDto stockPrice, UpDownStatus upDownStatus)
        {
            var title = messageService.GetMessage(
                MessageConstants.MSG_SymbolPriceIsOverExpectation,
                stockPrice.SymbolId,
                upDownStatus.ToString().ToUpper());

            var description = messageService.GetMessage(
                MessageConstants.MSG_PriceDescription,
                $"{stockPrice.Price:N} VND",
                $"{stockPrice.AtTime:HH:mm:ss}");

            await SendNotification(NotificationIdWhenExpectationReached, title, string.Empty, description);
        }

        protected abstract Task SendNotification(int notificationId, string title, string subtitle, string description);
    }
}
