namespace Application.Dtos.Bases
{
    public abstract record StockPriceBaseData
    {
        public required string SymbolId { get; init; }
        public decimal Price { get; init; }
        public decimal HighestPrice { get; init; }
        public decimal LowestPrice { get; init; }
    }
}
