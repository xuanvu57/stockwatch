using Application.Algorithms.Interfaces;
using Application.Attributes;
using Application.Dtos;
using Application.Dtos.Requests;
using Application.Dtos.Responses;
using Application.Services.Interfaces;
using static Domain.Constants.StockWatchEnums;

namespace Application.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class PotentialSymbolsAnalyzingService(IPriceHistoryCollectingService priceHistoryCollectingService, IPotentialSymbolsAlgorithmFactory algorithmFactory) : IPotentialSymbolsAnalyzingService
    {
        public async Task<PotentialSymbolResponse> Analyze(PotentialSymbolRequest request)
        {
            Dictionary<string, IEnumerable<StockPriceHistory>> priceHistory;
            if (string.IsNullOrEmpty(request.Market))
            {
                priceHistory = await priceHistoryCollectingService.GetByMarket(request.Market, request.Months);
            }
            else
            {
                priceHistory = await priceHistoryCollectingService.GetBySymbols(request.Symbols, request.Months);
            }

            var validPrices = AnalyzeBaseOnPriceHistory(priceHistory, request);

            return ConvertToResponse(validPrices);
        }

        private List<PotentialSymbol> AnalyzeBaseOnPriceHistory(Dictionary<string, IEnumerable<StockPriceHistory>> priceHistory, PotentialSymbolRequest request)
        {
            var potentialSymbols = new List<PotentialSymbol>();
            foreach (var symbolId in priceHistory.Keys)
            {
                var algorithmInstance = algorithmFactory.CreateAlgorithm(request.Algorithm);

                var potentialSymbol = algorithmInstance.Verify(priceHistory[symbolId], request);

                if (potentialSymbol is not null)
                {
                    potentialSymbols.Add(potentialSymbol);
                }
            }

            return potentialSymbols;
        }

        private static PotentialSymbolResponse ConvertToResponse(IEnumerable<PotentialSymbol> potentialSymbols)
        {
            return new()
            {
                AtTime = DateTime.Now,
                PotentialSymbols = potentialSymbols is null ? [] : potentialSymbols
            };
        }
    }
}
