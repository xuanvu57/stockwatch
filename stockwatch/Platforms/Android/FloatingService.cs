using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Application.Dtos;
using Application.Services.Interfaces;
using stockwatch.Services.Providers;
using static Android.Views.View;
using Color = Android.Graphics.Color;
using Format = Android.Graphics.Format;
using View = Android.Views.View;

namespace stockwatch.Platforms.Android
{
    [Service]
    public class FloatingService : Service, IOnTouchListener, IBackgroundServiceSubscriber
    {
        private IBackgroundService? backgroundService;

        private readonly DisplayMetrics displayMetrics = new();
        private IWindowManager? windowManager;
        private readonly WindowManagerLayoutParams layoutParams = new();
        private View? floatView;

        private int positionX;
        private int positionY;
        private bool isMoving;

        public override IBinder? OnBind(Intent? intent)
        {
            throw new NotImplementedException();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            InitializeParamsToShowFloatingWindow();

            windowManager?.AddView(floatView, layoutParams);

            SubscribeBackgroundService(this);

            return StartCommandResult.NotSticky;
        }

        public override void OnDestroy()
        {
            windowManager?.RemoveView(floatView);

            backgroundService?.Unsubscribe(this);

            base.OnDestroy();
        }

        public Task HandleBackgroundServiceEvent<T>(T data)
        {
            if (data is SymbolAnalyzingResultDto symbolAnalyzingResult)
            {
                UpdatePercentageView(symbolAnalyzingResult);
            }

            return Task.CompletedTask;
        }

        public bool OnTouch(global::Android.Views.View? v, MotionEvent? e)
        {
            if (e is null)
                return false;

            switch (e.Action)
            {
                case MotionEventActions.Down:
                    SetCurrentPosition(e);
                    break;

                case MotionEventActions.Move:
                    isMoving = true;
                    DragFloatingWindow(e);
                    break;

                case MotionEventActions.Up:
                    if (isMoving)
                    {
                        MoveFloatingWindowToEdge();
                    }
                    else
                    {
                        LaunchApp();
                    }
                    isMoving = false;
                    break;

                default:
                    break;
            }
            return false;
        }

        private void InitializeParamsToShowFloatingWindow()
        {
            windowManager = GetSystemService(WindowService).JavaCast<IWindowManager>()!;

#pragma warning disable CA1422
            windowManager?.DefaultDisplay?.GetMetrics(displayMetrics);
#pragma warning restore CA1422

            var mLayoutInflater = LayoutInflater.From(ApplicationContext)!;
            floatView = mLayoutInflater.Inflate(Resource.Layout.floatview, null)!;
            floatView.SetOnTouchListener(this);

            SetLayoutParams(displayMetrics);
        }

        private void SetLayoutParams(DisplayMetrics displayMetrics)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                layoutParams.Type = WindowManagerTypes.ApplicationOverlay;
            }
            else
            {
                layoutParams.Type = WindowManagerTypes.Phone;
            }
            layoutParams.Flags =
                WindowManagerFlags.LayoutNoLimits |
                WindowManagerFlags.NotFocusable |
                WindowManagerFlags.NotTouchModal |
                WindowManagerFlags.WatchOutsideTouch;

            layoutParams.Format = Format.Translucent;
            layoutParams.Gravity = GravityFlags.Top | GravityFlags.Left;
            layoutParams.Width = 150;
            layoutParams.Height = 150;
            layoutParams.X = displayMetrics.WidthPixels - layoutParams.Width;
            layoutParams.Y = 300;
        }

        private void SetCurrentPosition(MotionEvent e)
        {
            positionX = (int)e.RawX;
            positionY = (int)e.RawY;
        }

        private void DragFloatingWindow(MotionEvent e)
        {
            var nowX = (int)e.RawX;
            var nowY = (int)e.RawY;
            var movedX = nowX - positionX;
            var movedY = nowY - positionY;
            positionX = nowX;
            positionY = nowY;
            layoutParams.X = layoutParams.X + movedX;
            layoutParams.Y = layoutParams.Y + movedY;

            EnsureFloatingWindowInsideScreenView();
            windowManager?.UpdateViewLayout(floatView, layoutParams);
        }

        private void MoveFloatingWindowToEdge()
        {
            if (layoutParams.X < displayMetrics.WidthPixels / 2)
            {
                layoutParams.X = 0;
            }
            else
            {
                layoutParams.X = displayMetrics.WidthPixels - layoutParams.Width;
            }

            windowManager?.UpdateViewLayout(floatView, layoutParams);
        }

        private void EnsureFloatingWindowInsideScreenView()
        {
            layoutParams.X = Math.Max(0, Math.Min(displayMetrics.WidthPixels - layoutParams.Width, layoutParams.X));
            layoutParams.Y = Math.Max(0, Math.Min(displayMetrics.HeightPixels - layoutParams.Height, layoutParams.Y));
        }

        private void LaunchApp()
        {
            var main = PackageManager!.GetLaunchIntentForPackage(PackageName!);
            StartActivity(main);
        }

        private void SubscribeBackgroundService(IBackgroundServiceSubscriber subscriber)
        {
            backgroundService = PlatformsServiceProvider.ServiceProvider.GetRequiredService<IBackgroundService>();
            if (backgroundService?.IsRunning == true)
            {
                backgroundService.Subscribe(subscriber);
            }
        }

        private void UpdatePercentageView(SymbolAnalyzingResultDto? symbolAnalyzingResult)
        {
            var percentageView = floatView?.FindViewById<TextView>(Resource.Id.tv1);
            if (percentageView is not null)
            {
                percentageView.SetTextColor(Color.ParseColor(symbolAnalyzingResult?.Percentage > 0 ? "#00FFAA" : "#FF5555"));
                percentageView.SetText($"{symbolAnalyzingResult?.Percentage ?? 0:F2}%", TextView.BufferType.Editable);
            }
        }
    }
}
