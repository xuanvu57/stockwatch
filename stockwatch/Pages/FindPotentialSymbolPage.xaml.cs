using Application.Services.Interfaces;

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
        var market = pckMarket.SelectedItem.ToString();

        await loadingService.Show();

        var potentialSymbols = await potentialSymbolsAnalyzingService.Analyze(market!, 1, 2);

        lblResult.Text = string.Join(Environment.NewLine, potentialSymbols.Select(x => $"{x.SymbolId}: {x.AverageAmplitudeInPercentage:F}%"));

        await loadingService.Hide();
    }
}