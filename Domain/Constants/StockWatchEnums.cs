namespace Domain.Constants
{
    public class StockWatchEnums
    {
        public enum DIServiceLifetime
        {
            Singleton,
            Scoped,
            Transient,
        }

        public enum UpDownStatus
        {
            Up,
            Down,
        }

        public enum PriceTypes
        {
            Price,
            HighestPrice,
            LowestPrice,
        }

        public enum RequestInputTypes
        {
            Parameter,
            JsonBody,
        }

        public enum GroupPriceDataBy
        {
            Day,
            Week,
            Month,
        }

        public enum PotentialAlgorithm
        {
            Amplitude,
            ContinuouslyUp,
            ContinuouslyDown,
        }
    }
}
