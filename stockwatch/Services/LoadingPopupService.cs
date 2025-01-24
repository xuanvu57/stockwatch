using Application.Attributes;
using Application.Services.Interfaces;
using Mopups.Interfaces;
using Mopups.Services;
using stockwatch.Popups;
using static Application.Constants.ApplicationEnums;

namespace stockwatch.Services
{
    [DIService(DIServiceLifetime.Singleton)]
    public class LoadingPopupService : ILoadingService
    {
        private readonly IPopupNavigation popupNavigation;
        private LoadingPopup? loadingPopup;

        public LoadingPopupService()
        {
            popupNavigation = MopupService.Instance;
        }
        public async Task Hide()
        {
            await popupNavigation.PopAsync();
            loadingPopup = null;
        }

        public async Task Show(string message = "")
        {
            if (loadingPopup is null)
            {
                loadingPopup = new LoadingPopup();
                loadingPopup.UpdateMessage(message);
                await popupNavigation.PushAsync(loadingPopup);
            }
            else
            {
                loadingPopup.UpdateMessage(message);
            }
        }
    }
}
