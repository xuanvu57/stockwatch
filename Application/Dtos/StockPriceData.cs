using Domain.Entities;

namespace Application.Dtos
{
    public record StockPriceData
    {
        public required string SymbolId { get; init; }
        public decimal Price { get; init; }
        public decimal? RefPrice { get; init; }
        public DateTime AtTime { get; init; }
        public decimal HighestPrice { get; init; }
        public decimal LowestPrice { get; init; }

        public static StockPriceData? ConvertFromLatestStockPrice(LatestPriceEntity? latestPrice)
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
                PriceChangeInPercentage = RefPrice is null ? null : ((Price / RefPrice.Value) - 1),
                RefPrice = RefPrice,
                AtTime = AtTime,
                HighestPrice = HighestPrice,
                LowestPrice = LowestPrice
            };
        }
    }
}
