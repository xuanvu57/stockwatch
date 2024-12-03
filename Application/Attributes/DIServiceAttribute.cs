using static Domain.Constants.StockWatchEnums;

namespace Application.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DIServiceAttribute(DIServiceLifetime lifetime) : Attribute
    {
        public DIServiceLifetime Lifetime { get; } = lifetime;
    }

}