namespace Infrastructure.Clients.Ssi.Models
{
    public record DailyOhlcResponse
    {
        public string Symbol { get; init; } = string.Empty;
        public string Market { get; init; } = string.Empty;
        public DateOnly TradingDate { get; init; }
        public TimeOnly Time { get; init; }
        public decimal Open { get; init; }
        public decimal High { get; init; }
        public decimal Low { get; init; }
        public decimal Close { get; init; }
        public decimal Volume { get; init; }
        public decimal Value { get; init; }
    }
}
