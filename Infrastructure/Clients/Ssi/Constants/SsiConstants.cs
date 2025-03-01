namespace Infrastructure.Clients.Ssi.Constants
{
    public static class SsiConstants
    {
        public const string AuthorizationSchema = "Bearer";
        public const int MinSecondBetweenApiCalls = 1;

        public static class Endpoints
        {
            public const string AccessToken = "Market/AccessToken";
            public const string Securities = "Market/Securities";
            public const string DailyStockPrice = "Market/DailyStockPrice";
            public const string IntradayOhlc = "Market/IntradayOhlc";
            public const string DailyOhlc = "Market/DailyOhlc";
        }

        public static class Request
        {
            public const int DefaultPageIndex = 1;
            public const int DefaultPageSize = 50;
        }

        public static class Response
        {
            public const string DefaultResponseNonSuccessMessage = "There is an error with status code";
            public const string DefaultErrorLogMessage = "An error occurred while processing your request to SSI API";
        }

        public static class ResponseStatus
        {
            public const string SsiClientException = "SsiClientException";
            public const string Exception = "Exception";
            public const string Success = "Success";
            public const string NotFound = "NoDataFound";
        }

        public static class Format
        {
            public const string Date = "dd/MM/yyyy";
            public const string Time = "HH:mm:ss";
        }
    }
}
