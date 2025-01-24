using Domain.Entities.Bases;
using System.ComponentModel;

namespace Domain.Entities
{
    [DisplayName("FavoriteSymbol")]
    public record FavoriteSymbolEntity : StockBaseEntity
    {
        public DateTime AtTime { get; init; }
    }
}
