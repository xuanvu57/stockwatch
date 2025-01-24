using static Application.Constants.ApplicationEnums;

namespace Application.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DIServiceAttribute(DIServiceLifetime lifetime) : Attribute
    {
        public DIServiceLifetime Lifetime { get; } = lifetime;
    }

}