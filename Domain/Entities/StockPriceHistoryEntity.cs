using Domain.Entities.Bases;
using System.ComponentModel;

namespace Domain.Entities
{
    [DisplayName("StockPriceHistory")]
    public record StockPriceHistoryEntity : StockPriceBaseEntity
    {
        public DateOnly AtDate { get; init; }
        public required decimal HighestPrice { get; init; }
        public required decimal LowestPrice { get; init; }
        public required decimal ChangedPrice { get; init; }
        public required decimal ChangedPricePercent { get; init; }
        public required decimal OpenPrice { get; init; }
        public required decimal ClosePrice { get; init; }
        public required decimal AveragePrice { get; init; }
        public required decimal TotalMatchVolumn { get; init; }
        public required decimal TotalMatchValue { get; init; }
    }
}
