namespace stockwatch.Popups;

public partial class LoadingPopup : Mopups.Pages.PopupPage
{
    private const double DefaultHeightWithoutMessage = 0;
    private const double DefaultHeightWithMessage = 40;

    public LoadingPopup()
    {
        InitializeComponent();
    }

    public void UpdateMessage(string message)
    {
        lblLoadingMessage.Text = message;
        lblLoadingMessage.HeightRequest = string.IsNullOrEmpty(message)
            ? DefaultHeightWithoutMessage
            : DefaultHeightWithMessage;
    }
}