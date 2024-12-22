using Domain.Constants;
using static Domain.Constants.StockWatchEnums;

namespace Application.Dtos.Requests
{
    public record PotentialSymbolRequest
    {
        public string Market { get; init; } = string.Empty;
        public IEnumerable<string> Symbols { get; init; } = [];
        public int Months { get; init; } = StockWatchConstants.DefaultMonthToAnalyzePotentialSymbol;
        public GroupPriceDataBy GroupDataBy { get; init; } = GroupPriceDataBy.Day;
        public PotentialAlgorithm Algorithm { get; init; } = PotentialAlgorithm.Amplitude;
        public PriceTypes PriceType { get; init; } = PriceTypes.Price;
        public decimal ExpectedPercentage { get; init; }

    }
}
