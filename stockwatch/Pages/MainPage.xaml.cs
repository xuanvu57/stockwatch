using Application.Dtos;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using stockwatch.Configurations.Models;
using stockwatch.Models;

namespace stockwatch.Pages
{
    public partial class MainPage : ContentPage
    {
        private readonly IToastManagerService toastManagerService;
        private readonly IMessageService messageService;
        private readonly IRealtimePriceService stockDataService;
        private readonly IMySymbolAnalyzingService stockAnalyzorService;
        private readonly IReferenceSymbolRepository referenceSymbolRepository;

        private readonly IDispatcherTimer timer;
        private ReferenceSymbolEntity? targetSymbol;

        public LatestPriceModel LatestPrice { get; set; } = new LatestPriceModel();


        public MainPage(
            IConfiguration configuration,
            IToastManagerService toastManagerService,
            IMessageService messageService,
            IRealtimePriceService stockDataService,
            IMySymbolAnalyzingService stockAnalyzorService,
            IReferenceSymbolRepository referenceSymbolRepository)
        {
            InitializeComponent();
            BindingContext = this;

            var scheduleSettings = configuration.GetRequiredSection(nameof(ScheduleSettings)).Get<ScheduleSettings>()!;

            timer = Microsoft.Maui.Controls.Application.Current!.Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(scheduleSettings.FetchDataIntervalInSecond);
            timer.Tick += OnTimerTick;

            this.toastManagerService = toastManagerService;
            this.messageService = messageService;
            this.stockDataService = stockDataService;
            this.stockAnalyzorService = stockAnalyzorService;
            this.referenceSymbolRepository = referenceSymbolRepository;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            targetSymbol = await referenceSymbolRepository.Get();

            if (targetSymbol is not null)
            {
                InitReferenceSymbolInfo(targetSymbol);

                btnWatch.IsEnabled = false;
                timer.Start();
            }
        }

        private async void OnTimerTick(object? sender, EventArgs e)
        {
            await DoApiExecution(targetSymbol);
        }

        private async void OnWatchButtonClicked(object sender, EventArgs e)
        {
            if (!AreValidInputs(out var errorMessage))
            {
                await toastManagerService.Show(errorMessage);
                return;
            }

            btnWatch.IsEnabled = false;

            targetSymbol = CreateReferenceSymbolInfo();
            await referenceSymbolRepository.Save(targetSymbol);

            await toastManagerService.Show(messageService.GetMessage(MessageConstants.MSG_StartFollowingSymbol, targetSymbol.SymbolId));

            timer.Stop();
            await DoApiExecution(targetSymbol);
            timer.Start();
        }

        private void OnInputsTextChanged(object sender, TextChangedEventArgs e)
        {
            btnWatch.IsEnabled = true;
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
            entSymbol.Text = symbol.SymbolId;
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
                SymbolId = entSymbol.Text,
                InitializedPrice = price,
                CeilingPricePercentage = ceilingPrice,
                FloorPricePercentage = floorPrice
            };
        }

        private void SetLatestData(StockPriceInRealtime? stockPrice, DateTime time)
        {
            LatestPrice = LatestPrice.With(stockPrice?.SymbolId, stockPrice?.Price, time);
            LatestPrice.NotifyPropertyChanged();
        }

        private async Task DoApiExecution(ReferenceSymbolEntity? symbol)
        {
            try
            {
                if (symbol is null)
                    return;

                var stockData = await stockDataService.GetBySymbolId(symbol.SymbolId);

                if (stockData.Data.Any())
                {
                    await stockAnalyzorService.Analyze(stockData.Data.First(), symbol);
                }

                SetLatestData(stockData.Data.FirstOrDefault(), stockData.AtTime);
            }
            catch (Exception ex)
            {
                await toastManagerService.Show(ex.Message);
            }
        }
    }
}
