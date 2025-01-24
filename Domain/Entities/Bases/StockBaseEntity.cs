namespace Domain.Entities.Bases
{
    public abstract record StockBaseEntity
    {
        public required string Id { get; init; }
    }
}
