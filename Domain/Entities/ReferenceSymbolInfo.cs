namespace Domain.Entities
{
    public record ReferenceSymbolInfo
    {
        public required string SymbolId { get; init; }
        public string SymbolName { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public decimal FloorPrice { get; init; }
        public decimal CeilingPrice { get; init; }
    }
}
