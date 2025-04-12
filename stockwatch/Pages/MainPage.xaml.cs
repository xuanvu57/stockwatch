using Application.Dtos;
using Application.Services.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Repositories.Interfaces;

namespace stockwatch.Pages
{
    public partial class MainPage : ContentPage, IBackgroundServiceSubscriber
    {
        private readonly IToastManagerService toastManagerService;
        private readonly IMessageService messageService;
        private readonly IReferenceSymbolRepository referenceSymbolRepository;
        private readonly IBackgroundService backgroundService;

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

        private ReferenceSymbolEntity? targetSymbol;

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
                mySymbolSettingView.SetReferenceSymbolInfo(targetSymbol);

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

            backgroundService.RemoveSubscriber(this);
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
            if (!mySymbolSettingView.AreValidInputs(out var errorMessage))
            {
                await toastManagerService.Show(errorMessage);
                return;
            }

            targetSymbol = mySymbolSettingView.CreateReferenceSymbolInfo();
            await referenceSymbolRepository.Save(targetSymbol);

            await toastManagerService.Show(messageService.GetMessage(MessageConstants.MSG_StartFollowingSymbol, targetSymbol.SymbolId));
            mySymbolAnalyzingResultView.ClearLatestData();

            StartWatching();
        }

        private void StartWatching()
        {
            IsWatcherStopped = false;

            backgroundService.AddSubscriber(this);
            backgroundService.Restart();
        }

        private void StopWatching()
        {
            backgroundService.RemoveSubscriber(this);
            backgroundService.Stop();

            IsWatcherStopped = true;
        }

        public Task HandleBackgroundServiceEvent<T>(T data)
        {
            if (data is SymbolAnalyzingResultDto symbolAnalyzingResult)
            {
                mySymbolAnalyzingResultView.SetLatestData(symbolAnalyzingResult);
            }

            return Task.CompletedTask;
        }
    }
}
