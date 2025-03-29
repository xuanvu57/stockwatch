using Android.Animation;
using Android.Views.Animations;
using Android.Widget;
using Application.Attributes;
using Application.Dtos;
using stockwatch.Constants;
using stockwatch.Services.Interfaces;
using static Application.Constants.ApplicationEnums;
using Color = Android.Graphics.Color;
using View = Android.Views.View;

namespace stockwatch.Platforms.Android
{
    [DIService(DIServiceLifetime.Singleton)]
    public class AndroidFloatViewAnimationService : IFloatingViewAnimationService
    {
        public void DropFloatView(object view, int positionX)
        {
            if (view is View floatingView)
            {
                floatingView?.Post(() => AnimateDropFloatView(floatingView!, positionX));
            }
        }

        public void TouchFloatView(object view)
        {
            if (view is View floatingView)
            {
                floatingView?.Post(() => AnimateTouchFloatView(floatingView!));
            }
        }

        public void UpdateContent(object view, SymbolAnalyzingResultDto? symbolAnalyzingResult)
        {
            if (view is View floatingView)
            {
                UpdatePercentageView(floatingView, Resource.Id.tv1, symbolAnalyzingResult?.Percentage);
                UpdatePercentageView(floatingView, Resource.Id.tv2, symbolAnalyzingResult?.PercentageInDay);
            }
        }

        private static void UpdatePercentageView(View floatView, int viewId, decimal? percentage)
        {
            var percentageView = floatView?.FindViewById<TextView>(viewId);
            if (percentageView is not null)
            {
                var textColor = Color.ParseColor(percentage > 0 ? DisplayConstants.ColorUp : DisplayConstants.ColorDown);
                var percentageText = FormatPercentageText(percentage);

                percentageView.Post(() =>
                {
                    AnimateUpdatingPercentageView(floatView!, percentageView, percentageText, textColor);
                });
            }
        }

        private static string FormatPercentageText(decimal? percentage)
        {
            var signUpDown = percentage > 0 ? DisplayConstants.ArrowUp : DisplayConstants.ArrowDown;

            var absPercentage = Math.Abs(percentage ?? 0);
            var formattedAbsPercentage = absPercentage < 100 ? $"{absPercentage:F2}" : $"{absPercentage:F0}";

            if (percentage is null)
            {
                signUpDown = string.Empty;
                formattedAbsPercentage = DisplayConstants.NotAvailableValue;
            }
            return $"{signUpDown}{formattedAbsPercentage}%";
        }

        private static void AnimateDropFloatView(View floatingView, int positionX)
        {
            const long duration = 500;
            var bounceX = (positionX == 0 ? -1 : 1) * DisplayConstants.FloatingWindowBounceLength;
            floatingView.TranslationX = bounceX;

            var translationX = ObjectAnimator.OfFloat(floatingView, "TranslationX", 0)!;
            translationX.SetDuration(duration);
            translationX.SetInterpolator(new AccelerateInterpolator());
            translationX.SetInterpolator(new BounceInterpolator());
            translationX.Start();
        }

        private static void AnimateTouchFloatView(View floatingView)
        {
            const long duration = 500;
            var scaleX = ObjectAnimator.OfFloat(floatingView, "ScaleX", 1.0f, 0.8f, 1.0f)!;
            var scaleY = ObjectAnimator.OfFloat(floatingView, "ScaleY", 1.0f, 0.8f, 1.0f)!;

            var animatorSet = new AnimatorSet();
            animatorSet.PlayTogether(scaleX, scaleY);
            animatorSet.SetInterpolator(new AccelerateInterpolator());
            animatorSet.SetInterpolator(new BounceInterpolator());
            animatorSet.SetDuration(duration);
            animatorSet.Start();
        }

        private static void AnimateUpdatingPercentageView(View floatView, TextView percentageView, string percentageText, Color textColor)
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
                percentageView.SetText(percentageText, TextView.BufferType.Editable);

                animatorSet.RemoveAllListeners();
                animatorSet.SetDuration(durationOfReverse);
                animatorSet.Reverse();
            };
        }
    }
}
