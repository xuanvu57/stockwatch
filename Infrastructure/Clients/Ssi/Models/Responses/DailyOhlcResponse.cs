using Application.Dtos;
using Infrastructure.Clients.Ssi.Constants;
using System.Globalization;

namespace Infrastructure.Clients.Ssi.Models.Responses
{
    public record DailyOhlcResponse
    {
        public string Symbol { get; init; } = string.Empty;
        public string Market { get; init; } = string.Empty;
        public string TradingDate { get; init; } = string.Empty;
        public string Time { get; init; } = string.Empty;
        public string Open { get; init; } = string.Empty;
        public string High { get; init; } = string.Empty;
        public string Low { get; init; } = string.Empty;
        public string Close { get; init; } = string.Empty;
        public string Volume { get; init; } = string.Empty;
        public string Value { get; init; } = string.Empty;

        public StockPriceHistoryDto ToStockPriceHistoryDto()
        {
            return new StockPriceHistoryDto()
            {
                SymbolId = Symbol,
                AtDate = DateOnly.ParseExact(TradingDate, SsiConstants.Format.Date, CultureInfo.InvariantCulture),
                Price = decimal.Parse(Close),
                HighestPrice = decimal.Parse(High),
                LowestPrice = decimal.Parse(Low),
                OpenPrice = decimal.Parse(Open),
                ClosePrice = decimal.Parse(Close),
                ChangedPrice = 0,
                ChangedPricePercent = 0,
                AveragePrice = (decimal.Parse(High) + decimal.Parse(Low)) / 2,
                TotalMatchVolumn = 0,
                TotalMatchValue = 0
            };
        }
    }
}
