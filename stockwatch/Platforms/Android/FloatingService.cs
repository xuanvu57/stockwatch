using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Application.Dtos;
using Application.Services.Interfaces;
using stockwatch.Constants;
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
        private int touchedPositionX;
        private int touchedPositionY;

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

            backgroundService?.RemoveSubscriber(this);

            base.OnDestroy();
        }

        public Task HandleBackgroundServiceEvent<T>(T data)
        {
            if (data is SymbolAnalyzingResultDto symbolAnalyzingResult)
            {
                UpdatePercentage(symbolAnalyzingResult);
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
                    DragFloatingWindow(e);
                    break;

                case MotionEventActions.Up:
                    DecideActionWhenMotionUp(e);
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
            layoutParams.Width = DisplayConstants.FloatingWindowWidth;
            layoutParams.Height = DisplayConstants.FloatingWindowHeight;
            layoutParams.X = displayMetrics.WidthPixels - layoutParams.Width;
            layoutParams.Y = displayMetrics.HeightPixels / 2;
        }

        private void SetCurrentPosition(MotionEvent e)
        {
            positionX = (int)e.RawX;
            positionY = (int)e.RawY;

            touchedPositionX = (int)e.RawX;
            touchedPositionY = (int)e.RawY;
        }

        private void DragFloatingWindow(MotionEvent e)
        {
            var nowX = (int)e.RawX;
            var nowY = (int)e.RawY;
            var movedX = nowX - positionX;
            var movedY = nowY - positionY;
            positionX = nowX;
            positionY = nowY;
            layoutParams.X += movedX;
            layoutParams.Y += movedY;

            EnsureFloatingWindowInsideScreenView();
            windowManager?.UpdateViewLayout(floatView, layoutParams);
        }

        private void DecideActionWhenMotionUp(MotionEvent e)
        {
            var nowX = (int)e.RawX;
            var nowY = (int)e.RawY;

            var movingDistance = GetDistance(touchedPositionX, touchedPositionY, nowX, nowY);
            var isMoving = movingDistance > DisplayConstants.MinimumDistanceToConsiderMoving;

            if (isMoving)
            {
                MoveFloatingWindowToEdge();
            }
            else
            {
                LaunchApp();
            }
        }

        private static double GetDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
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
                backgroundService.AddSubscriber(subscriber);
                backgroundService.Restart();
            }
        }

        private void UpdatePercentage(SymbolAnalyzingResultDto? symbolAnalyzingResult)
        {
            UpdatePercentageView(Resource.Id.tv1, symbolAnalyzingResult?.Percentage);
            UpdatePercentageView(Resource.Id.tv2, symbolAnalyzingResult?.PercentageInDay);
        }

        private void UpdatePercentageView(int viewId, decimal? percentage)
        {
            var percentageView = floatView?.FindViewById<TextView>(viewId);
            if (percentageView is not null)
            {
                var signUpDown = percentage > 0 ? DisplayConstants.ArrowUp : DisplayConstants.ArrowDown;
                var textColor = Color.ParseColor(percentage > 0 ? DisplayConstants.ColorUp : DisplayConstants.ColorDown);

                var absPercentage = Math.Abs(percentage ?? 0);
                var formattedAbsPercentage = absPercentage < 100 ? $"{absPercentage:F2}" : $"{absPercentage:F0}";

                if (percentage is null)
                {
                    signUpDown = string.Empty;
                    formattedAbsPercentage = DisplayConstants.NotAvailableValue;
                }
                var finalText = $"{signUpDown}{formattedAbsPercentage}%";

                percentageView.Post(() => AnimatePercentageView(floatView!, percentageView, finalText, textColor));
            }
        }

        private static void AnimatePercentageView(View floatView, TextView percentageView, string finalText, Color textColor)
        {
            const long durationOfStarting = 1000;
            const long durationOfReverse = 500;

            var alpha = ObjectAnimator.OfFloat(percentageView, "Alpha", 1.0f, 0.0f)!;
            var rotation = ObjectAnimator.OfFloat(floatView, "Rotation", 0, 360)!;

            var animatorSet = new AnimatorSet();
            animatorSet.PlayTogether(alpha, rotation);
            animatorSet.SetDuration(durationOfStarting);
            animatorSet.Start();

            animatorSet.AnimationEnd += (_, _) =>
            {
                percentageView.SetTextColor(textColor);
                percentageView.SetText(finalText, TextView.BufferType.Editable);

                animatorSet.RemoveAllListeners();
                animatorSet.SetDuration(durationOfReverse);
                animatorSet.Reverse();
            };
        }
    }
}
