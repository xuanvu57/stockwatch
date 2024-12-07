namespace Domain.Entities.Bases
{
    public abstract record StockPriceBaseEntity
    {
        public required string SymbolId { get; init; }
        public decimal Price { get; init; }
        public DateTime AtTime { get; init; }
    }
}
