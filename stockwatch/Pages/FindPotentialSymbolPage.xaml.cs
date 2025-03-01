using Application.Dtos.Requests;
using Application.Extensions;
using Application.Services.Interfaces;
using Domain.Repositories.Interfaces;
using stockwatch.Models;
using stockwatch.Resources.Strings;
using static Domain.Constants.StockWatchEnums;

namespace stockwatch.Pages;

public partial class FindPotentialSymbolPage : ContentPage
{
    private readonly ILoadingService loadingService;
    private readonly IPotentialSymbolsAnalyzingService potentialSymbolsAnalyzingService;
    private readonly IFavoriteSymbolRepository favoriteSymbolRepository;

    public static IEnumerable<string> Markets { get; } = [AppResources.LBL_FavoriteSymbolsItem, .. Enum<Market>.ToDescriptions()];
    public static IEnumerable<string> Periods { get; } = Enum<GroupPriceDataBy>.ToDescriptions();
    public static IEnumerable<string> PotentialAlgorithms { get; } = Enum<PotentialAlgorithm>.ToDescriptions();
    public static IEnumerable<string> PriceTypes { get; } = Enum<PriceType>.ToDescriptions();

    public FindPotentialSymbolPage(
        ILoadingService loadingService,
        IPotentialSymbolsAnalyzingService potentialSymbolsAnalyzingService,
        IFavoriteSymbolRepository favoriteSymbolRepository)
    {
        InitializeComponent();
        this.loadingService = loadingService;
        this.potentialSymbolsAnalyzingService = potentialSymbolsAnalyzingService;
        this.favoriteSymbolRepository = favoriteSymbolRepository;
    }

    private async void OnAnalyzeButtonClicked(object sender, EventArgs e)
    {
        await loadingService.Show();

        var request = await CreateRequest();

        var analyziedResponse = await potentialSymbolsAnalyzingService.Analyze(request);
        var potentialSymbols = analyziedResponse.Data.Select(x => PotentialSymbolModel.FromPotentialSymbol(x));

        resultView.SetItemSource(potentialSymbols);

        await loadingService.Hide();
    }

    private async Task<PotentialSymbolRequest> CreateRequest()
    {
        var market = pckMarket.SelectedIndex == 0 ? (Market?)null : Enum<Market>.ToEnum(pckMarket.SelectedItem);
        var symbols = pckMarket.SelectedIndex == 0 ? await favoriteSymbolRepository.Get() : [];
        var groupBy = Enum<GroupPriceDataBy>.ToEnum(pckGroupByPeriod.SelectedItem);
        var algorithm = Enum<PotentialAlgorithm>.ToEnum(pckAlgorithm.SelectedItem);
        var priceType = Enum<PriceType>.ToEnum(pckPriceType.SelectedItem);
        var expectedAmplitudePercentage = decimal.Parse(entExpectedAmplitudePercentage.NumericText);

        return new()
        {
            Algorithm = algorithm,
            ExpectedAmplitudePercentage = expectedAmplitudePercentage,
            GroupDataBy = groupBy,
            Market = market,
            Symbols = symbols,
            PriceType = priceType
        };
    }
}