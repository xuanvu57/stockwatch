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
        var market = pckMarket.SelectedItem.ToString();
        var groupBy = ConvertPeriodToGroupPriceData(pckGroupByPeriod.SelectedItem.ToString());

        await loadingService.Show();

        var potentialSymbols = await potentialSymbolsAnalyzingService.Analyze(market!, 1, 5, groupBy);

        clvResult.ItemsSource = potentialSymbols;

        await loadingService.Hide();
    }

    private static GroupPriceDataBy ConvertPeriodToGroupPriceData(string? period)
    {
        if (!Enum.TryParse(typeof(GroupPriceDataBy), period, out var groupBy))
        {
            groupBy = GroupPriceDataBy.Day;
        }

        return (GroupPriceDataBy)groupBy;
    }
}