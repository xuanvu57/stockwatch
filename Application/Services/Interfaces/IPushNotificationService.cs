using Application.Dtos;
using static Domain.Constants.StockWatchEnums;

namespace Application.Services.Interfaces
{
    public interface IPushNotificationService
    {
        public Task NotifyWhenExpectationReached(StockPriceInRealtimeDto stockPrice, UpDownStatus upDownStatus);
    }
}
