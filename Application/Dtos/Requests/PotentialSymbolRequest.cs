using static Domain.Constants.StockWatchEnums;

namespace Application.Dtos.Requests
{
    public record PotentialSymbolRequest
    {
        public Market? Market { get; init; }
        public IEnumerable<string> Symbols { get; init; } = [];
        public GroupPriceDataBy GroupDataBy { get; init; } = GroupPriceDataBy.Day;
        public PotentialAlgorithm Algorithm { get; init; } = PotentialAlgorithm.Amplitude;
        public PriceType PriceType { get; init; } = PriceType.Price;
        public decimal ExpectedAmplitudePercentage { get; init; }
    }
}
