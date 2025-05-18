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

            [Description("Average price")]
            AveragePrice,

            [Description("Changed price")]
            ChangedPrice,

            [Description("Price at opened time")]
            OpenPrice,

            [Description("Price at closed time")]
            ClosePrice,

            [Description("Total match volumn")]
            TotalMatchVolumn,
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
