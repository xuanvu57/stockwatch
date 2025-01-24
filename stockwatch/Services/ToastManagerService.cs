using Application.Attributes;
using Application.Services.Interfaces;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using static Application.Constants.ApplicationEnums;

namespace stockwatch.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class ToastManagerService : IToastManagerService
    {
        public async Task Show(string message)
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            await Toast
                .Make(message, ToastDuration.Short)
                .Show(cancellationTokenSource.Token);
        }
    }
}
