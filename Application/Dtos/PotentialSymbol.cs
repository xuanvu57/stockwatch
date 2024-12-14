namespace Application.Dtos
{
    public record PotentialSymbol
    {
        public required string SymbolId { get; init; }
        public int MatchedRecordCount { get; init; }
        public decimal AverageAmplitude { get; init; }
        public decimal AverageAmplitudeInPercentage { get; init; }
    }
}
