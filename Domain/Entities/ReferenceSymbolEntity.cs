namespace Domain.Entities
{
    public record ReferenceSymbolEntity
    {
        public required string SymbolId { get; init; }
        public decimal InitializedPrice { get; init; }
        public decimal FloorPricePercentage { get; init; } = 0;
        public decimal CeilingPricePercentage { get; init; } = 0;
    }
}
