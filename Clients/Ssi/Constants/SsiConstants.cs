namespace stockwatch.Clients.Ssi.Constants
{
    public static class SsiConstants
    {
        public const string AuthorizationSchema = "Bearer";

        public static class Endpoints
        {
            public const string AccessToken = "Market/AccessToken";
            public const string DailyStockPrice = "Market/DailyStockPrice";
        }

        public static class Request
        {
            public const int DefaultPageIndex = 1;
            public const int DefaultPageSize = 10;
        }

        public static class ResponseStatus
        {
            public const string Success = "Success";
            public const string NotFound = "NoDataFound";
        }
    }
}
