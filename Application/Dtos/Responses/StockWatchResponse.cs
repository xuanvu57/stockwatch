namespace Application.Dtos.Responses
{
    public record StockWatchResponse
    {
        public required IEnumerable<StockPriceData> Symbols { get; init; }
        public DateTime Time { get; init; }
    }
}
