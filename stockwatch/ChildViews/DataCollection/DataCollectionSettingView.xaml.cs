using Application.Dtos.Requests;
using Application.Extensions;
using Application.Services.Interfaces;
using Domain.Repositories.Interfaces;
using stockwatch.Resources.Strings;
using stockwatch.Services.Providers;
using static Domain.Constants.StockWatchEnums;

namespace stockwatch.ChildViews.DataCollection;

public partial class DataCollectionSettingView : ContentView
{
    public static IEnumerable<string> Markets { get; } = [AppResources.LBL_FavoriteSymbolsItem, .. Enum<Market>.ToDescriptions()];

    private readonly IMessageService messageService;
    private readonly IFavoriteSymbolRepository favoriteSymbolRepository;

    public DataCollectionSettingView()
    {
        InitializeComponent();

        favoriteSymbolRepository = PlatformsServiceProvider.ServiceProvider.GetRequiredService<IFavoriteSymbolRepository>();
        messageService = PlatformsServiceProvider.ServiceProvider.GetRequiredService<IMessageService>();
    }

    public async Task<DataCollectionRequest> CreateRequest()
    {
        var market = pckMarket.SelectedIndex == 0 ? (Market?)null : Enum<Market>.ToEnum(pckMarket.SelectedItem);
        var symbols = pckMarket.SelectedIndex == 0 ? await favoriteSymbolRepository.Get() : [];
        var fromDate = dpkFromDate.SelectedDate!.Value;
        var toDate = dpkToDate.SelectedDate!.Value;
        if (fromDate > toDate)
        {
            var errorMessage = messageService.GetMessage(
                AppResources.MSG_Date1MustBeGreaterThanOrEqualDate2,
                AppResources.LBL_ToDate,
                AppResources.LBL_FromDate.ToLower());

            throw new ArgumentException(errorMessage);
        }

        return new()
        {
            FromDate = DateOnly.FromDateTime(fromDate),
            ToDate = DateOnly.FromDateTime(toDate),
            Market = market
        };
    }
}