using Application.Dtos.Requests;
using Application.Services;
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

        clvResult.ItemsSource = potentialSymbols.Data;

        await loadingService.Hide();
    }

    private PotentialSymbolRequest CreateRequest()
    {
        var market = pckMarket.SelectedItem?.ToString() ?? string.Empty;
        var groupBy = StockRulesService.ConvertToEnum<GroupPriceDataBy>(pckGroupByPeriod.SelectedItem);
        var algorithm = StockRulesService.ConvertToEnum<PotentialAlgorithm>(pckAlgorithm.SelectedItem);
        var priceType = StockRulesService.ConvertToEnum<PriceTypes>(pckPriceType.SelectedItem);
        var expectedPercentage = decimal.Parse(entExpectedPercentage.Text?.Replace(",", string.Empty) ?? "0");

        return new()
        {
            Algorithm = algorithm,
            ExpectedPercentage = expectedPercentage,
            GroupDataBy = groupBy,
            Market = market,
            PriceType = priceType
        };
    }
}