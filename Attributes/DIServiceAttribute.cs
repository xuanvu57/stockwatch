using static stockwatch.Constants.StockWatchEnums;

namespace stockwatch.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DIServiceAttribute(DIServiceLifetime lifetime) : Attribute
    {
        public DIServiceLifetime Lifetime { get; } = lifetime;
    }

}