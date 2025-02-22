using Application.Dtos;
using Application.Dtos.Bases;
using Application.Services.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Domain.Services;
using stockwatch.Models;

namespace stockwatch.Pages
{
    public partial class MainPage : ContentPage, IBackgroundServiceSubscriber
    {
        private readonly IToastManagerService toastManagerService;
        private readonly IMessageService messageService;
        private readonly IReferenceSymbolRepository referenceSymbolRepository;
        private readonly IBackgroundService backgroundService;

        private ReferenceSymbolEntity? targetSymbol;

        public LatestPriceModel LatestPrice { get; private set; } = new();

        private readonly BindableProperty IsWatcherStoppedProperty = BindableProperty.Create(
            propertyName: nameof(IsWatcherStopped),
            returnType: typeof(bool),
            declaringType: typeof(MainPage),
            defaultValue: true);
        public bool IsWatcherStopped
        {
            get => (bool)GetValue(IsWatcherStoppedProperty);
            private set => SetValue(IsWatcherStoppedProperty, value);
        }

        public MainPage(
            IToastManagerService toastManagerService,
            IMessageService messageService,
            IReferenceSymbolRepository referenceSymbolRepository,
            IBackgroundService backgroundService)
        {
            InitializeComponent();
            BindingContext = this;

            this.toastManagerService = toastManagerService;
            this.messageService = messageService;
            this.referenceSymbolRepository = referenceSymbolRepository;
            this.backgroundService = backgroundService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            targetSymbol = await referenceSymbolRepository.Get();

            if (targetSymbol is not null)
            {
                InitReferenceSymbolInfo(targetSymbol);

                StartWatching();
            }
            else
            {
                IsWatcherStopped = true;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            backgroundService.Unsubscribe(this);
        }

        private async void OnWatchButtonClicked(object sender, EventArgs e)
        {
            if (IsWatcherStopped)
            {
                await StartNewWatchingManual();
            }
            else
            {
                StopWatching();
            }
        }

        private async Task StartNewWatchingManual()
        {
            if (!AreValidInputs(out var errorMessage))
            {
                await toastManagerService.Show(errorMessage);
                return;
            }

            targetSymbol = CreateReferenceSymbolInfo();
            await referenceSymbolRepository.Save(targetSymbol);

            await toastManagerService.Show(messageService.GetMessage(MessageConstants.MSG_StartFollowingSymbol, targetSymbol.Id));

            StartWatching();
        }

        private void StartWatching()
        {
            IsWatcherStopped = false;

            backgroundService.Subscribe(this);
            backgroundService.Start();
        }

        private void StopWatching()
        {
            backgroundService.Unsubscribe(this);
            backgroundService.Stop();

            IsWatcherStopped = true;
        }

        private bool AreValidInputs(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(entSymbol.Text))
            {
                errorMessage = messageService.GetMessage(MessageConstants.MSG_PleaseInputSymbol);
                return false;
            }

            var isNumber = decimal.TryParse(entReferencePrice.Text?.Replace(",", string.Empty) ?? "0", out var price);
            if (!isNumber || price <= 0)
            {
                errorMessage = messageService.GetMessage(MessageConstants.MSG_PleaseInputValidPrice);
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        private void InitReferenceSymbolInfo(ReferenceSymbolEntity symbol)
        {
            entSymbol.Text = symbol.Id;
            entReferencePrice.Text = symbol.InitializedPrice.ToString();
            entCeilingPrice.Text = symbol.CeilingPricePercentage.ToString();
            entFloorPrice.Text = symbol.FloorPricePercentage.ToString();
        }

        private ReferenceSymbolEntity CreateReferenceSymbolInfo()
        {
            _ = decimal.TryParse(entReferencePrice.Text?.Replace(",", string.Empty) ?? "0", out var price);
            _ = decimal.TryParse(entCeilingPrice.Text?.Replace(",", string.Empty) ?? "0", out var ceilingPrice);
            _ = decimal.TryParse(entFloorPrice.Text?.Replace(",", string.Empty) ?? "0", out var floorPrice);

            return new()
            {
                Id = entSymbol.Text,
                InitializedPrice = price,
                CeilingPricePercentage = ceilingPrice,
                FloorPricePercentage = floorPrice
            };
        }

        private void SetLatestData(string symbolId, StockPriceInRealtimeDto? stockPrice, DateTime time)
        {
            decimal? percentage = null;
            if (stockPrice is not null && targetSymbol is not null)
            {
                percentage = StockRulesService.CalculatePercentage(stockPrice.Price, targetSymbol.InitializedPrice);
            }

            LatestPrice = LatestPrice.With(symbolId, stockPrice?.Price, percentage, time);
            LatestPrice.NotifyPropertyChanged();
        }

        public Task HandleBackgroundServiceEvent<T>(T data)
        {
            if (data is BaseResponse<StockPriceInRealtimeDto> stockPrice)
            {
                SetLatestData(targetSymbol!.Id, stockPrice.Data.FirstOrDefault(), stockPrice.AtTime);
            }

            return Task.CompletedTask;
        }
    }
}
