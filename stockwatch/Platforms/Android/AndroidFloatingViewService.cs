using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Application.Dtos;
using Application.Services.Interfaces;
using stockwatch.Constants;
using stockwatch.Services.Interfaces;
using stockwatch.Services.Providers;
using static Android.Views.View;
using Color = Android.Graphics.Color;
using Format = Android.Graphics.Format;
using View = Android.Views.View;

namespace stockwatch.Platforms.Android
{
    [Service]
    public class AndroidFloatingViewService : Service, IOnTouchListener, IBackgroundServiceSubscriber, IFloatingViewMovingHandlerService
    {
        private IBackgroundService? backgroundService;
        private IFloatingViewMovingService? floatingViewMovingService;

        private readonly DisplayMetrics displayMetrics = new();
        private readonly WindowManagerLayoutParams layoutParams = new();
        private IWindowManager? windowManager;
        private View? floatView;

        public override IBinder? OnBind(Intent? intent)
        {
            throw new NotImplementedException();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            SubscribeBackgroundService();
            InitializeParamsToShowFloatingWindow();
            SubcribeFloatingViewMovingService();

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
                    floatView?.Post(() => AnimateTouchFloatView(floatView));
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
            windowManager = GetSystemService(WindowService).JavaCast<IWindowManager>()!;

#pragma warning disable CA1422
            windowManager?.DefaultDisplay?.GetMetrics(displayMetrics);
#pragma warning restore CA1422

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
            layoutParams.X = displayMetrics.WidthPixels - layoutParams.Width;
            layoutParams.Y = displayMetrics.HeightPixels / 2;
        }

        private void SubcribeFloatingViewMovingService()
        {
            floatingViewMovingService = PlatformsServiceProvider.ServiceProvider.GetRequiredService<IFloatingViewMovingService>();
            floatingViewMovingService!.InitFloatingWindow(
                (displayMetrics.HeightPixels, displayMetrics.WidthPixels),
                (layoutParams.X, layoutParams.Y),
                this);
        }

        private void SubscribeBackgroundService()
        {
            backgroundService = PlatformsServiceProvider.ServiceProvider.GetRequiredService<IBackgroundService>();
            if (backgroundService?.IsRunning == true)
            {
                backgroundService.AddSubscriber(this);
                backgroundService.Restart();
            }
        }

        private void ConsiderActionWhenMotionUp(MotionEvent e)
        {
            var isMoved = floatingViewMovingService!.ConsiderToMoveFloatingWindow((int)e.RawX, (int)e.RawY);

            if (isMoved)
            {
                floatView?.Post(() => AnimateDropFloatView(floatView, layoutParams.X));
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

                percentageView.Post(() => AnimateUpdatingPercentageView(floatView!, percentageView, finalText, textColor));
            }
        }

        private static void AnimateUpdatingPercentageView(View floatView, TextView percentageView, string finalText, Color textColor)
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

        private static void AnimateTouchFloatView(View floatView)
        {
            const long duration = 500;
            var scaleX = ObjectAnimator.OfFloat(floatView, "ScaleX", 1.0f, 0.8f, 1.0f)!;
            var scaleY = ObjectAnimator.OfFloat(floatView, "ScaleY", 1.0f, 0.8f, 1.0f)!;

            var animatorSet = new AnimatorSet();
            animatorSet.PlayTogether(scaleX, scaleY);
            animatorSet.SetInterpolator(new AccelerateInterpolator());
            animatorSet.SetInterpolator(new BounceInterpolator());
            animatorSet.SetDuration(duration);
            animatorSet.Start();
        }

        private static void AnimateDropFloatView(View floatView, int positionX)
        {
            const long duration = 500;
            var bounceX = (positionX == 0 ? -1 : 1) * DisplayConstants.FloatingWindowBounceLength;
            floatView.TranslationX = bounceX;

            var translationX = ObjectAnimator.OfFloat(floatView, "TranslationX", 0)!;
            translationX.SetDuration(duration);
            translationX.SetInterpolator(new AccelerateInterpolator());
            translationX.SetInterpolator(new BounceInterpolator());
            translationX.Start();
        }
    }
}
