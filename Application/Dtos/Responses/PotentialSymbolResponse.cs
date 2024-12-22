namespace Application.Dtos.Responses
{
    public record PotentialSymbolResponse
    {
        public IEnumerable<PotentialSymbol> PotentialSymbols { get; init; } = [];
        public DateTime AtTime { get; init; }
    }
}
