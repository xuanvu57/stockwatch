namespace Infrastructure.Clients.Ssi.Models.Responses
{
    public record BroadcastResponse
    {
        public required string DataType { get; init; }
        public required string Content { get; init; }
    }

    public record BroadcastXTradeResponse
    {
        public required string RType { get; init; }
        public required string TradingDate { get; init; }
        public required string Time { get; init; }
        public required string Isin { get; init; }
        public required string Symbol { get; init; }
        public decimal Ceiling { get; init; }
        public decimal Floor { get; init; }
        public decimal RefPrice { get; init; }
        public decimal AvgPrice { get; init; }
        public decimal PriorVal { get; init; }
        public decimal LastPrice { get; init; }
        public decimal LastVol { get; init; }
        public decimal TotalVal { get; init; }
        public decimal TotalVol { get; init; }
        public required string MarketId { get; init; }
        public required string Exchange { get; init; }
        public required string TradingSession { get; init; }
        public required string TradingStatus { get; init; }
        public decimal Change { get; init; }
        public decimal RatioChange { get; init; }
        public decimal EstMatchedPrice { get; init; }
        public decimal Highest { get; init; }
        public decimal Lowest { get; init; }
        public required string Side { get; init; }
    }
}
