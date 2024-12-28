using Application.Attributes;
using Application.Services.Interfaces;
using stockwatch.Resources.Strings;
using static Domain.Constants.StockWatchEnums;

namespace stockwatch.Services
{
    [DIService(DIServiceLifetime.Singleton)]
    public class MessageService : IMessageService
    {
        public string GetMessage(string messageKey, params object[] arguments)
        {
            var message = AppResources.ResourceManager.GetString(messageKey) ?? messageKey;

            return string.Format(message, arguments);
        }
    }
}
