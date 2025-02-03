using static Domain.Constants.StockWatchEnums;

namespace Application.Dtos.Requests
{
    public record PotentialSymbolRequest
    {
        public Market? Market { get; init; }
        public IEnumerable<string> Symbols { get; init; } = [];
        public GroupPriceDataBy GroupDataBy { get; init; }
        public PotentialAlgorithm Algorithm { get; init; }
        public PriceType PriceType { get; init; }
        public decimal ExpectedAmplitudePercentage { get; init; }
    }
}
