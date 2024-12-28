namespace Domain.Entities
{
    public record FavoriteSymbolEntity
    {
        public required string SymbolId { get; init; }
        public DateTime AtTime { get; init; }
    }
}
