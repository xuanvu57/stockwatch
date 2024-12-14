using Application.Attributes;
using Application.Dtos;
using Application.Services.Interfaces;
using static Domain.Constants.StockWatchEnums;

namespace Application.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class PotentialSymbolsAnalyzingService(IPriceHistoryCollectingService priceHistoryCollectingService) : IPotentialSymbolsAnalyzingService
    {
        public async Task<IEnumerable<PotentialSymbol>> Analyze(string market, int months, decimal expectedAmplitudeInPercentage, GroupPriceDataBy groupPriceDataBy)
        {
            var potentialSymbols = new List<PotentialSymbol>();

            var priceHistory = await priceHistoryCollectingService.GetByMarket(market, months);

            foreach (var symbolId in priceHistory.Keys)
            {
                var groupedPriceHistory = GroupPriceByPeriod(priceHistory[symbolId], groupPriceDataBy);
                var validPrices = groupedPriceHistory
                    .Where(x => StockRulesService.CalculatePercentage(x.HighestPrice, x.LowestPrice) >= expectedAmplitudeInPercentage)
                    .ToList();

                if (validPrices.Count > 0)
                {
                    potentialSymbols.Add(new()
                    {
                        SymbolId = symbolId,
                        MatchedRecordCount = validPrices.Count,
                        AverageAmplitude = validPrices.Average(x => x.HighestPrice - x.LowestPrice),
                        AverageAmplitudeInPercentage = validPrices.Average(x => StockRulesService.CalculatePercentage(x.HighestPrice, x.LowestPrice))
                    });
                }
            }

            return potentialSymbols;
        }

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
    }
}
