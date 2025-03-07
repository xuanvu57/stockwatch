using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Provider;
using Application.Services.Interfaces;
using stockwatch.Platforms.Android;
using stockwatch.Services.Providers;

namespace stockwatch
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnStart()
        {
            StopService(new Intent(this, typeof(AndroidFloatingService)));
            base.OnStart();
        }

        protected override void OnResume()
        {
            StopService(new Intent(this, typeof(AndroidFloatingService)));
            base.OnResume();
        }

        protected override void OnStop()
        {
            if (!Settings.CanDrawOverlays(this))
            {
                StartActivityForResult(new Intent(Settings.ActionManageOverlayPermission, Android.Net.Uri.Parse("package:" + PackageName)), 0);
            }
            else
            {
                StartFloatingService();
            }
            base.OnStop();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
        {
            if (requestCode == 0 && Settings.CanDrawOverlays(this))
            {
                StartFloatingService();
            }
        }

        private void StartFloatingService()
        {
            var backgroundService = PlatformsServiceProvider.ServiceProvider.GetRequiredService<IBackgroundService>();
            if (backgroundService?.IsRunning == true)
            {
                StartService(new Intent(this, typeof(AndroidFloatingService)));
            }
        }
    }
}
