namespace Application.Dtos.Responses
{
    public record StockWatchResponse
    {
        public required IEnumerable<StockPriceInRealtime> Symbols { get; init; }
        public DateTime Time { get; init; }
    }
}
