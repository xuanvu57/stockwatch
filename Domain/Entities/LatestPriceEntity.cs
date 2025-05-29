using Domain.Entities.Bases;
using System.ComponentModel;

namespace Domain.Entities
{
    [DisplayName("LatestPrice")]
    public record LatestPriceEntity : StockPriceBaseEntity
    {
        public DateTime AtTime { get; init; }
        public decimal RefPrice { get; init; }
    }
}
