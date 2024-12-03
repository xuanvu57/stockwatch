using Microsoft.Extensions.Configuration;
using stockwatch.Configurations.Models;
using stockwatch.Models;
using stockwatch.Models.StockWatchModels;
using stockwatch.Repositories.Interfaces;
using stockwatch.Resources.Strings;
using stockwatch.Services.Interfaces;

namespace stockwatch
{
    public partial class MainPage : ContentPage
    {
        private readonly IToastManagerService toastManagerService;
        private readonly IStockDataService stockDataService;
        private readonly IAnalyzorService stockAnalyzorService;
        private readonly IReferenceSymbolRepository referenceSymbolRepository;

        private readonly IDispatcherTimer timer;
        private ReferenceSymbolInfo? targetSymbol;

        public MainPage(
            IConfiguration configuration,
            IToastManagerService toastManagerService,
            IStockDataService stockDataService,
            IAnalyzorService stockAnalyzorService,
            IReferenceSymbolRepository referenceSymbolRepository)
        {
            InitializeComponent();

            var scheduleSettings = configuration.GetRequiredSection(nameof(ScheduleSettings)).Get<ScheduleSettings>()!;

            timer = Application.Current!.Dispatcher.CreateTimer();
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
            if (!await IsValidate())
            {
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

        private void OnEntSymbolTextChanged(object sender, TextChangedEventArgs e)
        {
            btnWatch.IsEnabled = true;
        }

        private async Task<bool> IsValidate()
        {
            if (string.IsNullOrWhiteSpace(entSymbol.Text))
            {
                await toastManagerService.Show(AppResources.MSG_PleaseInputSymbol);
                return false;
            }

            var isNumber = decimal.TryParse(entReferencePrice.Text?.Replace(",", string.Empty) ?? "0", out var price);
            if (!isNumber || price <= 0)
            {
                await toastManagerService.Show(AppResources.MSG_PleaseInputValidPrice);
                return false;
            }

            return true;
        }

        private void SetUIByReferenceSymbolInfo(ReferenceSymbolInfo? symbol)
        {
            if (symbol is not null)
            {
                entSymbol.Text = symbol.SymbolId;
                entReferencePrice.Text = symbol.Price.ToString();
                entCeilingPrice.Text = symbol.CeilingPrice.ToString();
                entFloorPrice.Text = symbol.FloorPrice.ToString();
            }
        }

        private ReferenceSymbolInfo CreateReferenceSymbolInfo()
        {
            _ = decimal.TryParse(entReferencePrice.Text?.Replace(",", string.Empty) ?? "0", out var price);
            _ = decimal.TryParse(entCeilingPrice.Text?.Replace(",", string.Empty) ?? "0", out var ceilingPrice);
            _ = decimal.TryParse(entFloorPrice.Text?.Replace(",", string.Empty) ?? "0", out var floorPrice);

            return new ReferenceSymbolInfo()
            {
                SymbolId = entSymbol.Text,
                Price = price,
                CeilingPrice = ceilingPrice,
                FloorPrice = floorPrice
            };
        }

        private void SetLatestDataForUI(StockWatchResponse stockData, SymbolInfo? symbol)
        {
            lblPrice.Text = symbol?.Price.ToString() ?? "N/A";
            lblLatestDataAt.Text = stockData.Time.ToString("yyyy-MM-dd HH:mm:ss");
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

                SetLatestDataForUI(stockData, stockData.Symbols.FirstOrDefault());
            }
            catch (Exception ex)
            {
                await toastManagerService.Show(ex.Message);
            }
        }
    }
}
