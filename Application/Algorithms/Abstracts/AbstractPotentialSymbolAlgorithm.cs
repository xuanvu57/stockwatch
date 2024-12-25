using Application.Algorithms.Interfaces;
using Application.Dtos;
using Application.Dtos.Requests;
using Application.Services;
using static Domain.Constants.StockWatchEnums;

namespace Application.Algorithms.Abstracts
{
    public abstract class AbstractPotentialSymbolAlgorithm : IPotentialSymbolsAlgorithm
    {
        public PotentialSymbol? Verify(IEnumerable<StockPriceHistory> priceHistories, PotentialSymbolRequest request)
        {
            var groupedPriceHistory = GroupPriceByPeriod(priceHistories, request.GroupDataBy);

            var validPrices = FilterPriceHistory(groupedPriceHistory, request);

            return validPrices.Count == 0 ? null : ConvertToPotentialSymbol(validPrices);
        }

        protected abstract List<StockPriceHistory> FilterPriceHistory(IEnumerable<StockPriceHistory> priceHistories, PotentialSymbolRequest request);

        private static IEnumerable<StockPriceHistory> GroupPriceByPeriod(IEnumerable<StockPriceHistory> priceHistory, GroupPriceDataBy groupPriceDataBy)
        {
            if (groupPriceDataBy == GroupPriceDataBy.Day)
                return priceHistory;

            return priceHistory
                .Select(x => x with
                {
                    AtDate = groupPriceDataBy == GroupPriceDataBy.Week ?
                        StockRulesService.GetBeginningDateOfWeek(x.AtDate) :
                        StockRulesService.GetBeginningDateOfMonth(x.AtDate)
                })
                .GroupBy(x => new
                {
                    x.SymbolId,
                    x.AtDate
                })
                .Select(g => new StockPriceHistory()
                {
                    SymbolId = g.Key.SymbolId,
                    AtDate = g.Key.AtDate,
                    Price = g.Average(x => x.Price),
                    HighestPrice = g.Max(x => x.HighestPrice),
                    LowestPrice = g.Min(x => x.LowestPrice)
                });
        }

        private static PotentialSymbol ConvertToPotentialSymbol(List<StockPriceHistory> stockPriceHistories)
        {
            return new()
            {
                SymbolId = stockPriceHistories[0].SymbolId,
                IsFavorite = false,
                MatchedRecordCount = stockPriceHistories.Count,
                AverageAmplitude = stockPriceHistories.Average(x => x.HighestPrice - x.LowestPrice),
                AverageAmplitudeInPercentage = stockPriceHistories.Average(x => StockRulesService.CalculatePercentage(x.HighestPrice, x.LowestPrice))
            };
        }
    }
}
