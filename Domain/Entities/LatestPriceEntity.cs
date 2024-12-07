using Domain.Entities.Bases;

namespace Domain.Entities
{
    public record LatestPriceEntity : StockPriceBaseEntity
    {
        public decimal PriceChange { get; init; }
        public decimal PriceChangeInPercentage { get; init; }
    }
}
