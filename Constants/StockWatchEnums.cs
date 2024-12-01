namespace stockwatch.Constants
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
    }
}
