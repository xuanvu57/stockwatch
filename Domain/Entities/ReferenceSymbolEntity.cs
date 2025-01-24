using Domain.Entities.Bases;
using System.ComponentModel;

namespace Domain.Entities
{
    [DisplayName("ReferenceSymbol")]
    public record ReferenceSymbolEntity : StockBaseEntity
    {
        public decimal InitializedPrice { get; init; }
        public decimal FloorPricePercentage { get; init; } = 0;
        public decimal CeilingPricePercentage { get; init; } = 0;
    }
}
