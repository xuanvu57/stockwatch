namespace Infrastructure.Clients.Ssi.Models
{
    public record DailyOhlcResponse
    {
        public string Symbol { get; init; } = string.Empty;
        public string Market { get; init; } = string.Empty;
        public string TradingDate { get; init; } = string.Empty;
        public string Time { get; init; } = string.Empty;
        public string Open { get; init; } = string.Empty;
        public string High { get; init; } = string.Empty;
        public string Low { get; init; } = string.Empty;
        public string Close { get; init; } = string.Empty;
        public string Volume { get; init; } = string.Empty;
        public string Value { get; init; } = string.Empty;
    }
}
