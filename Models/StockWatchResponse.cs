namespace stockwatch.Models.StockWatchModels
{
    public record StockWatchResponse
    {
        public required IEnumerable<SymbolInfo> Symbols { get; init; }
        public DateTime Time { get; init; }
    }
}
