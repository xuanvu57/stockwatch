using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using static Android.Views.View;
using Format = Android.Graphics.Format;
using View = Android.Views.View;

namespace stockwatch.Platforms.Android
{
    [Service]
    public class FloatingService : Service, IOnTouchListener
    {
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

            return StartCommandResult.NotSticky;
        }

        public override void OnDestroy()
        {
            windowManager?.RemoveView(floatView);

            base.OnDestroy();
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
            //ImageView iv1 = floatView.FindViewById<ImageView>(Resource.Id.iv1);

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
                    MoveFloatingWindow(e);
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

        private void SetCurrentPosition(MotionEvent e)
        {
            positionX = (int)e.RawX;
            positionY = (int)e.RawY;
        }

        private void MoveFloatingWindow(MotionEvent e)
        {
            var nowX = (int)e.RawX;
            var nowY = (int)e.RawY;
            var movedX = nowX - positionX;
            var movedY = nowY - positionY;
            positionX = nowX;
            positionY = nowY;
            layoutParams.X = layoutParams.X + movedX;
            layoutParams.Y = layoutParams.Y + movedY;

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

        private void LaunchApp()
        {
            var main = PackageManager!.GetLaunchIntentForPackage(PackageName!);
            StartActivity(main);
        }
    }
}
