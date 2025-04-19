using Application.Services.Interfaces;
using stockwatch.Models;

namespace stockwatch.Pages;

public partial class FindPotentialSymbolPage : ContentPage
{
    private readonly ILoadingService loadingService;
    private readonly IPotentialSymbolsAnalyzingService potentialSymbolsAnalyzingService;

    public FindPotentialSymbolPage(
        ILoadingService loadingService,
        IPotentialSymbolsAnalyzingService potentialSymbolsAnalyzingService)
    {
        InitializeComponent();
        this.loadingService = loadingService;
        this.potentialSymbolsAnalyzingService = potentialSymbolsAnalyzingService;
    }

    private async void OnAnalyzeButtonClicked(object sender, EventArgs e)
    {
        await loadingService.Show();

        var request = await mySymbolSettingView.CreateRequest();

        var analyziedResponse = await potentialSymbolsAnalyzingService.Analyze(request);
        var potentialSymbols = analyziedResponse.Data.Select(x => PotentialSymbolModel.FromPotentialSymbol(x));

        resultView.SetItemSource(potentialSymbols, mySymbolSettingView.SelectedPeriod);

        await loadingService.Hide();
    }
}