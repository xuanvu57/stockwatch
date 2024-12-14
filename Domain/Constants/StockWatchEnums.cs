namespace Domain.Constants
{
    public class StockWatchEnums
    {
        public enum DIServiceLifetime
        {
            Singleton,
            Scoped,
            Transient
        }

        public enum NotificationTypes
        {
            Up,
            Down
        }

        public enum RequestInputTypes
        {
            Parameter,
            JsonBody
        }

        public enum GroupPriceDataBy
        {
            Day,
            Week,
            Month
        }
    }
}
