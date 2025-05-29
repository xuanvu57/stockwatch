namespace stockwatch.Pages;

public partial class DataCollectionPage : ContentPage
{
    public DataCollectionPage()
    {
        InitializeComponent();
    }

    private async void OnProceButtonClicked(object sender, EventArgs e)
    {
        var request = await dataCollectionSettingView.CreateRequest();

        lblResult.Text = $"""
            Completed
            {request}
            """;
    }
}