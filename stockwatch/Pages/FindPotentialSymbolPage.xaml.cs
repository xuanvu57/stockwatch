using Application.Dtos;
using Application.Dtos.Requests;
using Application.Extensions;
using Application.Services.Interfaces;
using static Domain.Constants.StockWatchEnums;

namespace stockwatch.Pages;

public partial class FindPotentialSymbolPage : ContentPage
{
    private readonly ILoadingService loadingService;
    private readonly IToastManagerService toastManagerService;
    private readonly IPotentialSymbolsAnalyzingService potentialSymbolsAnalyzingService;

    public static IEnumerable<string> Markets { get; } = Enum<Market>.ToDescriptions();
    public static IEnumerable<string> Periods { get; } = Enum<GroupPriceDataBy>.ToDescriptions();
    public static IEnumerable<string> PotentialAlgorithms { get; } = Enum<PotentialAlgorithm>.ToDescriptions();
    public static IEnumerable<string> PriceTypes { get; } = Enum<PriceType>.ToDescriptions();

    public FindPotentialSymbolPage(
        ILoadingService loadingService,
        IToastManagerService toastManagerService,
        IPotentialSymbolsAnalyzingService potentialSymbolsAnalyzingService)
    {
        InitializeComponent();
        this.loadingService = loadingService;
        this.toastManagerService = toastManagerService;
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
        var groupBy = Enum<GroupPriceDataBy>.ToEnum(pckGroupByPeriod.SelectedItem);
        var algorithm = Enum<PotentialAlgorithm>.ToEnum(pckAlgorithm.SelectedItem);
        var priceType = Enum<PriceType>.ToEnum(pckPriceType.SelectedItem);
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

    private void FavoriteItem_Invoked(object sender, EventArgs e)
    {
        var swipeItem = sender as SwipeItem;
        var item = (PotentialSymbol)swipeItem!.CommandParameter;

        toastManagerService.Show(item.SymbolId);
    }
}