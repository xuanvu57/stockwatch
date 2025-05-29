using static Domain.Constants.StockWatchEnums;

namespace Application.Dtos.Requests
{
    public record DataCollectionRequest
    {
        public DateOnly FromDate { get; init; }
        public DateOnly ToDate { get; init; }
        public Market? Market { get; init; }
        public IEnumerable<string> Symbols { get; init; } = [];
    }
}
