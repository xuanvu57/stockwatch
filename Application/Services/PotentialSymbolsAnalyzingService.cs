using Application.Algorithms.Interfaces;
using Application.Attributes;
using Application.Dtos;
using Application.Dtos.Requests;
using Application.Dtos.Responses;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using static Domain.Constants.StockWatchEnums;

namespace Application.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class PotentialSymbolsAnalyzingService(
        IPriceHistoryCollectingService priceHistoryCollectingService,
        IPotentialSymbolsAlgorithmFactory algorithmFactory,
        IFavoriteSymbolRepository favoriteSymbolRepository) : IPotentialSymbolsAnalyzingService
    {
        public async Task<BaseResponse<PotentialSymbol>> Analyze(PotentialSymbolRequest request)
        {
            Dictionary<string, IEnumerable<StockPriceHistory>> priceHistory;
            if (string.IsNullOrEmpty(request.Market))
            {
                priceHistory = await priceHistoryCollectingService.GetBySymbols(request.Symbols, request.Months);
            }
            else
            {
                priceHistory = await priceHistoryCollectingService.GetByMarket(request.Market, request.Months);
            }

            var validPrices = await AnalyzeBaseOnPriceHistory(priceHistory, request);

            return ConvertToResponse(validPrices);
        }

        private async Task<List<PotentialSymbol>> AnalyzeBaseOnPriceHistory(Dictionary<string, IEnumerable<StockPriceHistory>> priceHistory, PotentialSymbolRequest request)
        {
            var favoriteSymbolIds = await favoriteSymbolRepository.Get();

            var potentialSymbols = new List<PotentialSymbol>();
            foreach (var symbolId in priceHistory.Keys)
            {
                var algorithmInstance = algorithmFactory.CreateAlgorithm(request.Algorithm);

                var potentialSymbol = algorithmInstance.Verify(priceHistory[symbolId], request);

                if (potentialSymbol is not null)
                {
                    potentialSymbols.Add(UpdateFavoriteAttribute(potentialSymbol, favoriteSymbolIds));
                }
            }

            return potentialSymbols;
        }

        private static PotentialSymbol UpdateFavoriteAttribute(PotentialSymbol symbol, IEnumerable<string> favoriteSymbolIds)
        {
            return symbol with { IsFavorite = favoriteSymbolIds.Contains(symbol.SymbolId) };
        }

        private static BaseResponse<PotentialSymbol> ConvertToResponse(IEnumerable<PotentialSymbol> potentialSymbols)
        {
            return new()
            {
                Data = potentialSymbols is null ? [] : potentialSymbols
            };
        }
    }
}
