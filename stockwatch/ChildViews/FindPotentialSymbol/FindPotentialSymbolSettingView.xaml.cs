using Application.Dtos.Requests;
using Application.Extensions;
using Application.Settings;
using Domain.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using stockwatch.Resources.Strings;
using stockwatch.Services.Providers;
using static Domain.Constants.StockWatchEnums;

namespace stockwatch.ChildViews.FindPotentialSymbol;

public partial class FindPotentialSymbolSettingView : ContentView
{
    private readonly IFavoriteSymbolRepository favoriteSymbolRepository;

    public static IEnumerable<string> Markets { get; } = [AppResources.LBL_FavoriteSymbolsItem, .. Enum<Market>.ToDescriptions()];
    public static IEnumerable<string> Periods { get; } = Enum<GroupPriceDataBy>.ToDescriptions();
    public static IEnumerable<string> PotentialAlgorithms { get; } = Enum<PotentialAlgorithm>.ToDescriptions();
    public static IEnumerable<string> PriceTypes { get; } = Enum<PriceType>.ToDescriptions();

    public static readonly BindableProperty MaxMonthsToAnalyzeProperty = BindableProperty.Create(
        propertyName: nameof(MaxMonthsToAnalyze),
        returnType: typeof(int?),
        declaringType: typeof(FindPotentialSymbolSettingView),
        defaultValue: default(int?),
        defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty MaxSymbolsInMarketProperty = BindableProperty.Create(
        propertyName: nameof(MaxSymbolsInMarket),
        returnType: typeof(int?),
        declaringType: typeof(FindPotentialSymbolSettingView),
        defaultValue: default(int?),
        defaultBindingMode: BindingMode.TwoWay);

    public int MaxMonthsToAnalyze
    {
        get { return GetValue(MaxMonthsToAnalyzeProperty) as int? ?? 0; }
        init { SetValue(MaxMonthsToAnalyzeProperty, value); }
    }

    public int MaxSymbolsInMarket
    {
        get { return GetValue(MaxSymbolsInMarketProperty) as int? ?? 0; }
        init { SetValue(MaxSymbolsInMarketProperty, value); }
    }

    public string SelectedPeriod
    {
        get => pckGroupByPeriod.SelectedItem as string ?? string.Empty;
    }

    public FindPotentialSymbolSettingView()
    {
        InitializeComponent();
        favoriteSymbolRepository = PlatformsServiceProvider.ServiceProvider.GetRequiredService<IFavoriteSymbolRepository>();
        var configuration = PlatformsServiceProvider.ServiceProvider.GetRequiredService<IConfiguration>();
        var potentialSymbolSettings = configuration.GetRequiredSection(nameof(PotentialSymbolSettings)).Get<PotentialSymbolSettings>()!;

        MaxMonthsToAnalyze = potentialSymbolSettings.MaxMonthsToAnalyze;
        MaxSymbolsInMarket = potentialSymbolSettings.MaxSymbolsFromMarket;
    }

    public async Task<PotentialSymbolRequest> CreateRequest()
    {
        var market = pckMarket.SelectedIndex == 0 ? (Market?)null : Enum<Market>.ToEnum(pckMarket.SelectedItem);
        var symbols = pckMarket.SelectedIndex == 0 ? await favoriteSymbolRepository.Get() : [];
        var groupBy = Enum<GroupPriceDataBy>.ToEnum(pckGroupByPeriod.SelectedItem);
        var algorithm = Enum<PotentialAlgorithm>.ToEnum(pckAlgorithm.SelectedItem);
        var priceType = Enum<PriceType>.ToEnum(pckPriceType.SelectedItem);
        var expectedAmplitudePercentage = decimal.Parse(entExpectedAmplitudePercentage.NumericText);
        var monthsToAnalyze = int.Parse(entMonthsToAnalyze.NumericText);
        var maxSymbolsFromMarket = int.Parse(entMaxSymbolsFromMarket.NumericText);

        return new()
        {
            Algorithm = algorithm,
            ExpectedAmplitudePercentage = expectedAmplitudePercentage,
            GroupDataBy = groupBy,
            Market = market,
            Symbols = symbols,
            PriceType = priceType,
            MonthsToAnalyze = monthsToAnalyze,
            MaxSymbolsFromMarket = maxSymbolsFromMarket
        };
    }
}