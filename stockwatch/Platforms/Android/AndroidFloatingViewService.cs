using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using AndroidX.Core.App;
using Application.Constants;
using Application.Dtos;
using Application.Services.Interfaces;
using stockwatch.Constants;
using stockwatch.Services.Interfaces;
using stockwatch.Services.Providers;
using static Android.Views.View;
using Format = Android.Graphics.Format;
using View = Android.Views.View;

namespace stockwatch.Platforms.Android
{
    [Service]
    public class AndroidFloatingViewService : Service, IOnTouchListener, IBackgroundServiceSubscriber, IFloatingViewMovingHandlerService
    {
        private const string ForegroundNotificationChannelId = "1000";
        private const string ForegroundNotificationChannelName = "notification";
        private const int ForegroundNotificationId = 1001;

        private IBackgroundService? backgroundService;
        private IFloatingViewMovingService? floatingViewMovingService;
        private IFloatingViewAnimationService? floatingViewAnimationService;

        private readonly DisplayMetrics displayMetrics = new();
        private readonly WindowManagerLayoutParams layoutParams = new();
        private IWindowManager? windowManager;
        private View? floatView;

        public override IBinder? OnBind(Intent? intent)
        {
            return null;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            windowManager = GetSystemService(WindowService).JavaCast<IWindowManager>()!;
#pragma warning disable CA1422
            windowManager?.DefaultDisplay?.GetMetrics(displayMetrics);
#pragma warning restore CA1422

            SubscribeBackgroundService();
            SubcribeMovingAndAnimationServices();
            InitializeParamsToShowFloatingWindow();

            StartAsForegroundService();

            return StartCommandResult.NotSticky;
        }

        public override void OnDestroy()
        {
            windowManager?.RemoveView(floatView);

            backgroundService?.RemoveSubscriber(this);

            base.OnDestroy();
        }

        public Task HandleBackgroundServiceEvent<T>(T data)
        {
            if (data is SymbolAnalyzingResultDto symbolAnalyzingResult)
            {
                floatingViewAnimationService?.UpdateContent(floatView!, symbolAnalyzingResult);
            }

            return Task.CompletedTask;
        }

        public void MovingHandler((int x, int y) toPosition)
        {
            (layoutParams.X, layoutParams.Y) = toPosition;
            windowManager?.UpdateViewLayout(floatView, layoutParams);
        }

        public bool OnTouch(global::Android.Views.View? v, MotionEvent? e)
        {
            if (e is null)
                return false;

            switch (e.Action)
            {
                case MotionEventActions.Down:
                    floatingViewMovingService!.SetTouchDownPosition((int)e.RawX, (int)e.RawY);
                    floatingViewAnimationService!.TouchFloatView(floatView!);
                    break;

                case MotionEventActions.Move:
                    floatingViewMovingService!.MoveFloatingWindow((int)e.RawX, (int)e.RawY);
                    break;

                case MotionEventActions.Up:
                    ConsiderActionWhenMotionUp(e);
                    break;

                default:
                    break;
            }
            return false;
        }

        private void InitializeParamsToShowFloatingWindow()
        {
            var mLayoutInflater = LayoutInflater.From(ApplicationContext)!;
            floatView = mLayoutInflater.Inflate(Resource.Layout.floatview, null)!;
            floatView.SetOnTouchListener(this);

            InitLayoutParams();

            windowManager?.AddView(floatView, layoutParams);
        }

        private void InitLayoutParams()
        {
            layoutParams.Type = Build.VERSION.SdkInt >= BuildVersionCodes.O
                ? WindowManagerTypes.ApplicationOverlay
                : WindowManagerTypes.Phone;

            layoutParams.Flags =
                WindowManagerFlags.LayoutNoLimits |
                WindowManagerFlags.NotFocusable |
                WindowManagerFlags.NotTouchModal |
                WindowManagerFlags.WatchOutsideTouch;

            layoutParams.Format = Format.Translucent;
            layoutParams.Gravity = GravityFlags.Top | GravityFlags.Left;
            layoutParams.Width = DisplayConstants.FloatingWindowWidth;
            layoutParams.Height = DisplayConstants.FloatingWindowHeight;

            (layoutParams.X, layoutParams.Y) = floatingViewMovingService!.GetLatestPosition();
        }

        private void SubcribeMovingAndAnimationServices()
        {
            floatingViewAnimationService = PlatformsServiceProvider.ServiceProvider.GetRequiredService<IFloatingViewAnimationService>();
            floatingViewMovingService = PlatformsServiceProvider.ServiceProvider.GetRequiredService<IFloatingViewMovingService>();

            floatingViewMovingService!.InitFloatingWindow(
                (displayMetrics.HeightPixels, displayMetrics.WidthPixels),
                this);
        }

        private void SubscribeBackgroundService()
        {
            var backgroundServices = PlatformsServiceProvider.ServiceProvider.GetRequiredService<IEnumerable<IBackgroundService>>();
            backgroundService = backgroundServices.First(x => x.ServiceName == ApplicationConsts.MySymbolWatchingBackgroundServiceName);
            if (backgroundService?.IsRunning == true)
            {
                backgroundService.AddSubscriber(this);
                backgroundService.Restart();
            }
        }

        private void StartAsForegroundService()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                CreateNotificationChannel();
            }

            var notification = new NotificationCompat.Builder(this, ForegroundNotificationChannelId);
            notification.SetAutoCancel(false);
            notification.SetOngoing(true);
            notification.SetSmallIcon(Resource.Mipmap.appicon);
            notification.SetContentTitle("continue monitoring your symbol");
            StartForeground(ForegroundNotificationId, notification.Build());
        }

        private void CreateNotificationChannel()
        {
            var notifcationManager = GetSystemService(Context.NotificationService) as NotificationManager;
            var channel = new NotificationChannel(ForegroundNotificationChannelId, ForegroundNotificationChannelName, NotificationImportance.Low);
            notifcationManager?.CreateNotificationChannel(channel);
        }

        private void ConsiderActionWhenMotionUp(MotionEvent e)
        {
            var isMoved = floatingViewMovingService!.ConsiderOfMovingActionAfterUntouch((int)e.RawX, (int)e.RawY);

            if (isMoved)
            {
                floatingViewAnimationService?.DropFloatView(floatView!, layoutParams.X);
            }
            else
            {
                LaunchApp();
            }
        }

        private void LaunchApp()
        {
            var main = PackageManager!.GetLaunchIntentForPackage(PackageName!);
            StartActivity(main);
        }
    }
}
