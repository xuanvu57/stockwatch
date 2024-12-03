namespace Application.Dtos
{
    public record SymbolInfo
    {
        public required string SymbolId { get; init; }
        public string SymbolName { get; init; } = string.Empty;
        public decimal Price { get; init; }
    }
}
