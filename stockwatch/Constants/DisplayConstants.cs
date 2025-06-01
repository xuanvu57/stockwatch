using Application.Constants;

namespace stockwatch.Constants
{
    internal static class DisplayConstants
    {
        public const double MinimumDistanceToConsiderMoving = 12;
        public const int CurrencyDecimalPlace = ApplicationConsts.CurrencyDecimalPlace;

        public const string ArrowUp = "↑";
        public const string ArrowDown = "↓";

        public const string ColorUp = "#00FFAA";
        public const string ColorDown = "#FF5555";

        public const string NotAvailableValue = "N/A";

        public const int FloatingWindowWidth = 200;
        public const int FloatingWindowHeight = 200;
        public const int FloatingWindowBounceLength = 40;
    }
}
