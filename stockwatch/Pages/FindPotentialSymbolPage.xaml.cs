using Application.Dtos.Requests;
using Application.Services.Interfaces;
using static Domain.Constants.StockWatchEnums;

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

        var request = CreateRequest();
        var potentialSymbols = await potentialSymbolsAnalyzingService.Analyze(request);

        clvResult.ItemsSource = potentialSymbols.PotentialSymbols;

        await loadingService.Hide();
    }

    private PotentialSymbolRequest CreateRequest()
    {
        var market = pckMarket.SelectedItem.ToString() ?? "";
        var groupBy = ConvertPeriodToGroupPriceData(pckGroupByPeriod.SelectedItem.ToString());
        var algorithm = ConvertToPotentialAlgorithm(pckAlgorithm.SelectedItem.ToString());

        return new()
        {
            Algorithm = algorithm,
            ExpectedPercentage = 5,
            GroupDataBy = groupBy,
            Market = market,
        };
    }

    private static GroupPriceDataBy ConvertPeriodToGroupPriceData(string? periodName)
    {
        if (!Enum.TryParse(typeof(GroupPriceDataBy), periodName, out var groupBy))
        {
            groupBy = GroupPriceDataBy.Day;
        }

        return (GroupPriceDataBy)groupBy;
    }

    private static PotentialAlgorithm ConvertToPotentialAlgorithm(string? algorithmName)
    {
        if (!Enum.TryParse(typeof(PotentialAlgorithm), algorithmName, out var algorithm))
        {
            algorithm = PotentialAlgorithm.Amplitude;
        }

        return (PotentialAlgorithm)algorithm;
    }
}