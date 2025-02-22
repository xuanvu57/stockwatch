namespace Infrastructure.Clients.Ssi.Models.Responses
{
    public record DailyStockPriceResponse
    {
        public required string TradingDate { get; init; } = string.Empty;
        public required string PriceChange { get; init; } = string.Empty;
        public required string PerPriceChange { get; init; } = string.Empty;
        public required string CeilingPrice { get; init; } = string.Empty;
        public required string FloorPrice { get; init; } = string.Empty;
        public required string RefPrice { get; init; } = string.Empty;
        public required string HighestPrice { get; init; } = string.Empty;
        public required string LowestPrice { get; init; } = string.Empty;
        public required string ClosePrice { get; init; } = string.Empty;
        public required string AveragePrice { get; init; } = string.Empty;
        public required string ClosePriceAdjusted { get; init; } = string.Empty;
        public required string TotalMatchVol { get; init; } = string.Empty;
        public required string TotalMatchVal { get; init; } = string.Empty;
        public required string TotalDealVal { get; init; } = string.Empty;
        public required string TotalDealVol { get; init; } = string.Empty;
        public required string ForeignBuyVolTotal { get; init; } = string.Empty;
        public required string ForeignCurrentRoom { get; init; } = string.Empty;
        public required string ForeignSellVolTotal { get; init; } = string.Empty;
        public required string ForeignBuyValTotal { get; init; } = string.Empty;
        public required string ForeignSellValTotal { get; init; } = string.Empty;
        public required string TotalBuyTrade { get; init; } = string.Empty;
        public required string TotalBuyTradeVol { get; init; } = string.Empty;
        public required string TotalSellTrade { get; init; } = string.Empty;
        public required string TotalSellTradeVol { get; init; } = string.Empty;
        public required string NetBuySellVol { get; init; } = string.Empty;
        public required string NetBuySellVal { get; init; } = string.Empty;
        public required string TotalTradedVol { get; init; } = string.Empty;
        public required string TotalTradedValue { get; init; } = string.Empty;
        public required string Symbol { get; init; } = string.Empty;
        public required string Time { get; init; } = string.Empty;
    }
}
