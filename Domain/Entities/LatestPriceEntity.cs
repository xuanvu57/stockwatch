using Domain.Entities.Bases;

namespace Domain.Entities
{
    public record LatestPriceEntity : StockPriceBaseEntity
    {
        public decimal RefPrice { get; init; }
    }
}
