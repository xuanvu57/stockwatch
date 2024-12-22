using Application.Algorithms.Abstracts;
using Application.Dtos;
using Application.Dtos.Requests;
using Application.Services;
using static Domain.Constants.StockWatchEnums;

namespace Application.Algorithms
{
    public class ContinuouslyUpOrDownAlgorithm : AbstractPotentialSymbolAlgorithm
    {
        protected override List<StockPriceHistory> FilterPriceHistory(IEnumerable<StockPriceHistory> priceHistories, PotentialSymbolRequest request)
        {
            if (!priceHistories.Any())
            {
                return [];
            }

            var orderedPriceHistory = priceHistories
                .OrderBy(x => x.AtDate);

            var validHitoreis = new List<StockPriceHistory>();

            StockPriceHistory? previousPriceHistory = null;
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
        private static decimal GetPriceByType(StockPriceHistory priceHistory, PriceTypes priceType)
        {
            return priceType switch
            {
                PriceTypes.HighestPrice => priceHistory.HighestPrice,
                PriceTypes.LowestPrice => priceHistory.LowestPrice,
                _ => priceHistory.Price
            };
        }
    }
}
