using Domain.Entities.Bases;
using System.ComponentModel;

namespace Domain.Entities
{
    [DisplayName("LatestPrice")]
    public record LatestPriceEntity : StockPriceBaseEntity
    {
        public decimal RefPrice { get; init; }
    }
}
