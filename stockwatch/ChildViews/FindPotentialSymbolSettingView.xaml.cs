using Application.Dtos.Requests;
using Application.Extensions;
using Domain.Repositories.Interfaces;
using stockwatch.Resources.Strings;
using stockwatch.Services.Providers;
using static Domain.Constants.StockWatchEnums;

namespace stockwatch.ChildViews;

public partial class FindPotentialSymbolSettingView : ContentView
{
    private readonly IFavoriteSymbolRepository favoriteSymbolRepository;

    public static IEnumerable<string> Markets { get; } = [AppResources.LBL_FavoriteSymbolsItem, .. Enum<Market>.ToDescriptions()];
    public static IEnumerable<string> Periods { get; } = Enum<GroupPriceDataBy>.ToDescriptions();
    public static IEnumerable<string> PotentialAlgorithms { get; } = Enum<PotentialAlgorithm>.ToDescriptions();
    public static IEnumerable<string> PriceTypes { get; } = Enum<PriceType>.ToDescriptions();

    public FindPotentialSymbolSettingView()
    {
        InitializeComponent();
        favoriteSymbolRepository = PlatformsServiceProvider.ServiceProvider.GetRequiredService<IFavoriteSymbolRepository>();
    }

    public async Task<PotentialSymbolRequest> CreateRequest()
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