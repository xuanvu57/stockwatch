namespace Application.Dtos
{
    public record StockPriceData
    {
        public required string SymbolId { get; init; }
        public decimal Price { get; init; }
        public DateTime AtTime { get; init; }
        public decimal RefPrice { get; init; }
        public decimal PriceChange { get; init; }
        public decimal PriceChangeInPercentage { get; init; }
    }
}
