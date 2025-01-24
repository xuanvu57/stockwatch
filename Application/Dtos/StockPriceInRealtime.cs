using Application.Dtos.Bases;
using Domain.Entities;

namespace Application.Dtos
{
    public record StockPriceInRealtime : StockPriceBaseData
    {
        public decimal? RefPrice { get; init; }
        public DateTime AtTime { get; init; }

        public static StockPriceInRealtime? ConvertFromLatestStockPrice(LatestPriceEntity? latestPrice)
        {
            return latestPrice is null ?
                null :
                new()
                {
                    SymbolId = latestPrice.Id,
                    Price = latestPrice.Price,
                    HighestPrice = 0,
                    LowestPrice = 0,
                    AtTime = latestPrice.AtTime,
                };
        }

        public LatestPriceEntity? ToLatestPriceEntity()
        {
            return RefPrice is null ?
                null :
                new()
                {
                    Id = SymbolId,
                    Price = Price,
                    RefPrice = RefPrice.Value,
                    AtTime = AtTime,
                };
        }
    }
}
