namespace Domain.Entities.Bases
{
    public abstract record StockBaseEntity
    {
        public string Id { get; init; } = Guid.NewGuid().ToString();
        public required string SymbolId { get; init; }
    }
}
