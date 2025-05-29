namespace Application.Constants
{
    public static class ApplicationConsts
    {
        public const string CurrencyVND = "VND";
        public const int CurrencyDecimalPlace = 0;
        public const int MinAndroidVersionSupportFirebaseSdk = 29;

        public const string MySymbolWatchingBackgroundServiceName = "MySymbolWatchingBackgroundService";
        public const string DataCollectionBackgroundServiceName = "DataCollectionBackgroundService";

        public static class LoggedErrorMessage
        {
            public const string ErrorMessageWhenAnalyzeMySymbolFailed = "An error occurred while analyzing your symbol";
            public const string ErrorMessageInReadingData = "Error in reading data from";
            public const string ErrorMessageInSavingData = "Error in saving data to";
            public const string ErrorMessageInDeletingData = "Error in deleting data to";
            public const string ErrorMessageWhenFileNotFound = " file not found";
        }
    }
}
