namespace Application.Constants
{
    public static class ApplicationEnums
    {
        public enum DIServiceLifetime
        {
            Singleton,
            Scoped,
            Transient,
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
