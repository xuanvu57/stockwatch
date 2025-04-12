namespace Application.Constants
{
    public static class ApplicationEnums
    {
        public enum DIServiceLifetime
        {
            Singleton,
            Scoped,
            Transient,
            Skipped
        }

        public enum RequestInputTypes
        {
            Parameter,
            JsonBody,
        }

        public enum ResponseStatus
        {
            Success,
            Error
        }
    }
}
