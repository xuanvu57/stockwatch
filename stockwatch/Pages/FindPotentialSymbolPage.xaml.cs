using Application.Dtos.Requests;
using Application.Extensions;
using Application.Services.Interfaces;
using Domain.Constants;
using Domain.Repositories.Interfaces;
using stockwatch.Models;
using stockwatch.Resources.Strings;
using System.Collections.ObjectModel;
using static Domain.Constants.StockWatchEnums;

namespace stockwatch.Pages;

public partial class FindPotentialSymbolPage : ContentPage
{
    private readonly ILoadingService loadingService;
    private readonly IToastManagerService toastManagerService;
    private readonly IMessageService messageService;
    private readonly IPotentialSymbolsAnalyzingService potentialSymbolsAnalyzingService;
    private readonly IFavoriteSymbolRepository favoriteSymbolRepository;

    public static IEnumerable<string> Markets { get; } = [AppResources.LBL_FavoriteSymbolsItem, .. Enum<Market>.ToDescriptions()];
    public static IEnumerable<string> Periods { get; } = Enum<GroupPriceDataBy>.ToDescriptions();
    public static IEnumerable<string> PotentialAlgorithms { get; } = Enum<PotentialAlgorithm>.ToDescriptions();
    public static IEnumerable<string> PriceTypes { get; } = Enum<PriceType>.ToDescriptions();

    private ObservableCollection<PotentialSymbolModel>? PotentialSymbols { get; set; }

    public FindPotentialSymbolPage(
        ILoadingService loadingService,
        IToastManagerService toastManagerService,
        IMessageService messageService,
        IPotentialSymbolsAnalyzingService potentialSymbolsAnalyzingService,
        IFavoriteSymbolRepository favoriteSymbolRepository)
    {
        InitializeComponent();
        this.loadingService = loadingService;
        this.toastManagerService = toastManagerService;
        this.messageService = messageService;
        this.potentialSymbolsAnalyzingService = potentialSymbolsAnalyzingService;
        this.favoriteSymbolRepository = favoriteSymbolRepository;
    }

    private async void OnAnalyzeButtonClicked(object sender, EventArgs e)
    {
        await loadingService.Show();

        var request = await CreateRequest();
        var analyziedResponse = await potentialSymbolsAnalyzingService.Analyze(request);

        PotentialSymbols = new(analyziedResponse.Data.Select(x => PotentialSymbolModel.FromPotentialSymbol(x)));

        clvResult.ItemsSource = PotentialSymbols;

        await loadingService.Hide();
    }

    private async void FavoriteItem_Invoked(object sender, EventArgs e)
    {
        var swipeItem = sender as SwipeItem;
        var item = (PotentialSymbolModel)swipeItem!.CommandParameter;

        var isInFavoriteList = item.IsFavorite;
        var isSuccess = false;
        if (isInFavoriteList)
        {
            isSuccess = await favoriteSymbolRepository.Remove(item.SymbolId);
        }
        else
        {
            isSuccess = await favoriteSymbolRepository.Add(item.SymbolId);
        }

        if (isSuccess)
        {
            var symbol = PotentialSymbols!.First(x => x.SymbolId == item.SymbolId);
            symbol.ToggleFavorite();

            await toastManagerService.Show(
                isInFavoriteList ?
                messageService.GetMessage(MessageConstants.MSG_RemoveToFavoriteSuccessfully, item.SymbolId) :
                messageService.GetMessage(MessageConstants.MSG_AddToFavoriteSuccessfully, item.SymbolId));
        }
    }

    private async Task<PotentialSymbolRequest> CreateRequest()
    {
        var market = pckMarket.SelectedIndex == 0 ? (Market?)null : Enum<Market>.ToEnum(pckMarket.SelectedItem);
        var symbols = pckMarket.SelectedIndex == 0 ? await favoriteSymbolRepository.Get() : [];
        var groupBy = Enum<GroupPriceDataBy>.ToEnum(pckGroupByPeriod.SelectedItem);
        var algorithm = Enum<PotentialAlgorithm>.ToEnum(pckAlgorithm.SelectedItem);
        var priceType = Enum<PriceType>.ToEnum(pckPriceType.SelectedItem);
        var expectedAmplitudePercentage = decimal.Parse(entExpectedAmplitudePercentage.Text?.Replace(",", string.Empty) ?? "0");

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