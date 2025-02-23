using Application.Dtos.Bases;
using Domain.Entities;
using Domain.Services;

namespace Application.Dtos
{
    public record SymbolAnalyzingResultDto : StockPriceBaseDto
    {
        public new decimal? Price { get; init; }
        public decimal? Percentage { get; init; }
        public DateTime AtTime { get; init; }

        public static SymbolAnalyzingResultDto FromStockPriceInRealtimeDto(
            StockPriceInRealtimeDto? stockPrice,
            ReferenceSymbolEntity referenceSymbolEntity,
            DateTime atTime)
        {
            decimal? percentage = null;
            if (stockPrice is not null)
            {
                percentage = StockRulesService.CalculatePercentage(stockPrice.Price, referenceSymbolEntity!.InitializedPrice);
            }

            return new()
            {
                SymbolId = referenceSymbolEntity!.Id,
                Price = stockPrice?.Price,
                Percentage = percentage,
                AtTime = atTime,
            };
        }
    }
}
