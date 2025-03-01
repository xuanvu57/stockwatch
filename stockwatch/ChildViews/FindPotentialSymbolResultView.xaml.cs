using Application.Services.Interfaces;
using Domain.Constants;
using Domain.Repositories.Interfaces;
using stockwatch.Models;
using stockwatch.Services.Providers;
using System.Collections.ObjectModel;

namespace stockwatch.ChildViews;

public partial class FindPotentialSymbolResultView : ContentView
{
    public static readonly BindableProperty SelectedGroupByPeriodProperty = BindableProperty.Create(
        propertyName: nameof(SelectedGroupByPeriod),
        returnType: typeof(string),
        declaringType: typeof(FindPotentialSymbolResultView),
        defaultValue: default(string),
        defaultBindingMode: BindingMode.TwoWay);

    public string SelectedGroupByPeriod
    {
        get => GetValue(SelectedGroupByPeriodProperty) as string ?? string.Empty;
        set => SetValue(SelectedGroupByPeriodProperty, value);
    }

    private ObservableCollection<PotentialSymbolModel>? PotentialSymbols { get; set; }

    private readonly IToastManagerService toastManagerService;
    private readonly IMessageService messageService;
    private readonly IFavoriteSymbolRepository favoriteSymbolRepository;

    public FindPotentialSymbolResultView()
    {
        InitializeComponent();

        toastManagerService = PlatformsServiceProvider.ServiceProvider.GetRequiredService<IToastManagerService>();
        messageService = PlatformsServiceProvider.ServiceProvider.GetRequiredService<IMessageService>();
        favoriteSymbolRepository = PlatformsServiceProvider.ServiceProvider.GetRequiredService<IFavoriteSymbolRepository>();
    }

    public void SetItemSource(IEnumerable<PotentialSymbolModel> potentialSymbolModels)
    {
        PotentialSymbols = new(potentialSymbolModels);

        clvResult.ItemsSource = PotentialSymbols;
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
}