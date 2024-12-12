namespace stockwatch.Popups;

public partial class LoadingPopup : Mopups.Pages.PopupPage
{
    public LoadingPopup()
    {
        InitializeComponent();
    }

    public void UpdateMessage(string message)
    {
        lblLoadingMessage.Text = message;
    }
}