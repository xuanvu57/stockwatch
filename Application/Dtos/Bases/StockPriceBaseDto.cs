namespace Application.Dtos.Bases
{
    public abstract record StockPriceBaseDto
    {
        public required string SymbolId { get; init; }
        public decimal Price { get; init; }
    }
}
