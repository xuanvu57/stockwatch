using Application.Algorithms.PotentialSymbols.Interfaces;
using Application.Dtos;
using Application.Dtos.Requests;
using Domain.Services;
using static Domain.Constants.StockWatchEnums;

namespace Application.Algorithms.PotentialSymbols.Abstracts
{
    public abstract class AbstractPotentialSymbolAlgorithm : IPotentialSymbolsAlgorithm
    {
        public PotentialSymbolDto? Verify(IEnumerable<StockPriceHistoryDto> priceHistories, PotentialSymbolRequest request)
        {
            var groupedPriceHistory = GroupPriceByPeriod(priceHistories, request.GroupDataBy);

            var validPrices = FilterPriceHistory(groupedPriceHistory, request);

            return validPrices.Count == 0 ? null : ConvertToPotentialSymbol(validPrices);
        }

        protected abstract List<StockPriceHistoryDto> FilterPriceHistory(IEnumerable<StockPriceHistoryDto> priceHistories, PotentialSymbolRequest request);

        private static IEnumerable<StockPriceHistoryDto> GroupPriceByPeriod(IEnumerable<StockPriceHistoryDto> priceHistory, GroupPriceDataBy groupPriceDataBy)
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
                .Select(g => new StockPriceHistoryDto()
                {
                    SymbolId = g.Key.SymbolId,
                    AtDate = g.Key.AtDate,
                    Price = g.Average(x => x.Price),
                    HighestPrice = g.Max(x => x.HighestPrice),
                    LowestPrice = g.Min(x => x.LowestPrice)
                });
        }

        private static PotentialSymbolDto ConvertToPotentialSymbol(List<StockPriceHistoryDto> stockPriceHistories)
        {
            return new()
            {
                SymbolId = stockPriceHistories[0].SymbolId,
                MatchedRecordCount = stockPriceHistories.Count,
                AverageAmplitude = stockPriceHistories.Average(x => x.HighestPrice - x.LowestPrice),
                AverageAmplitudeInPercentage = stockPriceHistories.Average(x => StockRulesService.CalculatePercentage(x.HighestPrice, x.LowestPrice))
            };
        }
    }
}
