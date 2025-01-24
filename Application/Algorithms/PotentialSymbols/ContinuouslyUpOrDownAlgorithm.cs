using Application.Algorithms.PotentialSymbols.Abstracts;
using Application.Dtos;
using Application.Dtos.Requests;
using Domain.Services;
using static Domain.Constants.StockWatchEnums;

namespace Application.Algorithms.PotentialSymbols
{
    public class ContinuouslyUpOrDownAlgorithm : AbstractPotentialSymbolAlgorithm
    {
        protected override List<StockPriceHistoryDto> FilterPriceHistory(IEnumerable<StockPriceHistoryDto> priceHistories, PotentialSymbolRequest request)
        {
            if (!priceHistories.Any())
            {
                return [];
            }

            var orderedPriceHistory = priceHistories
                .OrderBy(x => x.AtDate);

            var validHitoreis = new List<StockPriceHistoryDto>();

            StockPriceHistoryDto? previousPriceHistory = null;
            foreach (var priceHistory in orderedPriceHistory)
            {
                if (previousPriceHistory is not null)
                {
                    var currentPrice = GetPriceByType(priceHistory, request.PriceType);
                    var previousPrices = GetPriceByType(previousPriceHistory, request.PriceType);

                    var percentage = request.Algorithm switch
                    {
                        PotentialAlgorithm.ContinuouslyUp => StockRulesService.CalculatePercentage(currentPrice, previousPrices),
                        _ => StockRulesService.CalculatePercentage(previousPrices, currentPrice)
                    };

                    if (percentage >= request.ExpectedPercentage)
                    {
                        validHitoreis.Add(priceHistory);
                    }
                }

                previousPriceHistory = priceHistory;
            }

            return validHitoreis;
        }
        private static decimal GetPriceByType(StockPriceHistoryDto priceHistory, PriceType priceType)
        {
            return priceType switch
            {
                PriceType.HighestPrice => priceHistory.HighestPrice,
                PriceType.LowestPrice => priceHistory.LowestPrice,
                _ => priceHistory.Price
            };
        }
    }
}
