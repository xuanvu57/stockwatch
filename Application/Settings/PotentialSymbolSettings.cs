namespace Application.Settings
{
    public record PotentialSymbolSettings
    {
        public required int MaxSymbolsFromMarket { get; init; }
        public required int MaxMonthsToAnalyze { get; init; }
    }
}
