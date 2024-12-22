using Application.Dtos;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using stockwatch.Configurations.Models;
using stockwatch.Resources.Strings;

namespace stockwatch.Pages
{
    public partial class MainPage : ContentPage
    {
        private readonly IToastManagerService toastManagerService;
        private readonly IRealtimePriceService stockDataService;
        private readonly IMySymbolAnalyzingService stockAnalyzorService;
        private readonly IReferenceSymbolRepository referenceSymbolRepository;

        private readonly IDispatcherTimer timer;
        private ReferenceSymbolEntity? targetSymbol;

        public MainPage(
            IConfiguration configuration,
            IToastManagerService toastManagerService,
            IRealtimePriceService stockDataService,
            IMySymbolAnalyzingService stockAnalyzorService,
            IReferenceSymbolRepository referenceSymbolRepository)
        {
            InitializeComponent();

            var scheduleSettings = configuration.GetRequiredSection(nameof(ScheduleSettings)).Get<ScheduleSettings>()!;

            timer = Microsoft.Maui.Controls.Application.Current!.Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(scheduleSettings.FetchDataIntervalInSecond);
            timer.Tick += OnTimerTick;

            this.toastManagerService = toastManagerService;
            this.stockDataService = stockDataService;
            this.stockAnalyzorService = stockAnalyzorService;
            this.referenceSymbolRepository = referenceSymbolRepository;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            targetSymbol = await referenceSymbolRepository.Get();
            SetUIByReferenceSymbolInfo(targetSymbol);
        }

        private async void OnTimerTick(object? sender, EventArgs e)
        {
            await DoApiExecution();
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

            timer.Stop();
            await DoApiExecution();
            timer.Start();

            await toastManagerService.Show($"Start following {targetSymbol.SymbolId}");
        }

        private void OnInputsTextChanged(object sender, TextChangedEventArgs e)
        {
            btnWatch.IsEnabled = true;
        }

        private bool AreValidInputs(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(entSymbol.Text))
            {
                errorMessage = AppResources.MSG_PleaseInputSymbol;
                return false;
            }

            var isNumber = decimal.TryParse(entReferencePrice.Text?.Replace(",", string.Empty) ?? "0", out var price);
            if (!isNumber || price <= 0)
            {
                errorMessage = AppResources.MSG_PleaseInputValidPrice;
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        private void SetUIByReferenceSymbolInfo(ReferenceSymbolEntity? symbol)
        {
            if (symbol is not null)
            {
                entSymbol.Text = symbol.SymbolId;
                entReferencePrice.Text = symbol.InitializedPrice.ToString();
                entCeilingPrice.Text = symbol.CeilingPricePercentage.ToString();
                entFloorPrice.Text = symbol.FloorPricePercentage.ToString();

                btnWatch.IsEnabled = false;
            }
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

        private void SetLatestDataForUI(StockPriceInRealtime? stockPrice, DateTime time)
        {
            lblPrice.Text = stockPrice?.Price.ToString() ?? "N/A";
            lblLatestDataAt.Text = time.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private async Task DoApiExecution()
        {
            try
            {
                if (targetSymbol is null)
                    return;

                var stockData = await stockDataService.GetBySymbolId(targetSymbol.SymbolId);

                if (stockData.Symbols.Any())
                {
                    await stockAnalyzorService.Analyze(stockData.Symbols.First(), targetSymbol);
                }

                SetLatestDataForUI(stockData.Symbols.FirstOrDefault(), stockData.AtTime);
            }
            catch (Exception ex)
            {
                await toastManagerService.Show(ex.Message);
            }
        }
    }
}
