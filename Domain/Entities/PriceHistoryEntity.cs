using Domain.Entities.Bases;

namespace Domain.Entities
{
    public record PriceHistoryEntity : StockPriceBaseEntity
    {
        public decimal? RefPrice { get; init; }
        public decimal? PriceChange { get; init; }
        public decimal? PriceChangeInPercentage { get; init; }
        public decimal HighestPrice { get; init; }
        public decimal LowestPrice { get; init; }
    }
}
