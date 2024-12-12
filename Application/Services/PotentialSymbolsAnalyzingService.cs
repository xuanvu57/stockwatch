using Application.Attributes;
using Application.Dtos;
using Application.Services.Interfaces;
using static Domain.Constants.StockWatchEnums;

namespace Application.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class PotentialSymbolsAnalyzingService(IPriceHistoryCollectingService priceHistoryCollectingService) : IPotentialSymbolsAnalyzingService
    {
        public async Task<IEnumerable<PotentialSymbol>> Analyze(string market, int months, decimal expectedAmplitudeInPercentage)
        {
            var potentialSymbols = new List<PotentialSymbol>();

            var priceHistory = await priceHistoryCollectingService.GetByMarket(market, months);

            foreach (var symbolId in priceHistory.Keys)
            {
                var averageAmplitude = priceHistory[symbolId].Average(x => x.HighestPrice - x.LowestPrice);
                var averageAmplitudeInPercentage = priceHistory[symbolId].Average(x => StockRulesService.CalculatePercentage(x.HighestPrice, x.LowestPrice));

                if (averageAmplitudeInPercentage > expectedAmplitudeInPercentage)
                {
                    potentialSymbols.Add(new()
                    {
                        SymbolId = symbolId,
                        AverageAmplitude = averageAmplitude,
                        AverageAmplitudeInPercentage = averageAmplitudeInPercentage
                    });
                }
            }

            return potentialSymbols;
        }
    }
}
