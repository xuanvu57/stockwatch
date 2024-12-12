using Application.Services;
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
                    SymbolId = latestPrice.SymbolId,
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
                    SymbolId = SymbolId,
                    Price = Price,
                    RefPrice = RefPrice.Value,
                    AtTime = AtTime,
                };
        }

        public PriceHistoryEntity ToPriceHistoryEntity()
        {
            return new()
            {
                SymbolId = SymbolId,
                Price = Price,
                PriceChange = RefPrice is null ? null : Price - RefPrice.Value,
                PriceChangeInPercentage = RefPrice is null ? null : StockRulesService.CalculatePercentage(Price, RefPrice.Value),
                RefPrice = RefPrice,
                AtTime = AtTime,
                HighestPrice = HighestPrice,
                LowestPrice = LowestPrice
            };
        }
    }
}
