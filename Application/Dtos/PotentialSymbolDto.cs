namespace Application.Dtos
{
    public record PotentialSymbolDto
    {
        public required string SymbolId { get; init; }
        public bool IsFavorite { get; init; }
        public int MatchedRecordCount { get; init; }
        public decimal AverageAmplitude { get; init; }
        public decimal AverageAmplitudeInPercentage { get; init; }
    }
}
