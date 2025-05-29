namespace Domain.Entities.Bases
{
    public abstract record StockPriceBaseEntity : StockBaseEntity
    {
        public decimal Price { get; init; }
    }
}
