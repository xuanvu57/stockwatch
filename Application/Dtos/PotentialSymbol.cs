namespace Application.Dtos
{
    public record PotentialSymbol
    {
        public required string SymbolId { get; init; }
        public bool IsFavorite { get; init; } = false;
        public int MatchedRecordCount { get; init; }
        public decimal AverageAmplitude { get; init; }
        public decimal AverageAmplitudeInPercentage { get; init; }
    }
}
