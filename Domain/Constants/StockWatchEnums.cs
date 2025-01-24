using System.ComponentModel;

namespace Domain.Constants
{
    public class StockWatchEnums
    {
        public enum UpDownStatus
        {
            Up,
            Down,
        }

        public enum Market
        {
            HOSE,
            HNX,
            UPCOM,
            DER,
            BOND
        }

        public enum PriceType
        {
            [Description("Price")]
            Price,

            [Description("Highest price")]
            HighestPrice,

            [Description("Lowest price")]
            LowestPrice,
        }

        public enum GroupPriceDataBy
        {
            Day,
            Week,
            Month,
        }

        public enum PotentialAlgorithm
        {
            [Description("Amplitude")]
            Amplitude,

            [Description("Continuously Up")]
            ContinuouslyUp,

            [Description("Continuously Down")]
            ContinuouslyDown,
        }
    }
}
