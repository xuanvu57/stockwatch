using Application.Algorithms.PotentialSymbols.Interfaces;
using Application.Attributes;
using Application.Dtos;
using Application.Dtos.Bases;
using Application.Dtos.Requests;
using Application.Services.Interfaces;
using Domain.Repositories.Interfaces;
using static Domain.Constants.StockWatchEnums;

namespace Application.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class PotentialSymbolsAnalyzingService(
        IPriceHistoryCollectingService priceHistoryCollectingService,
        IPotentialSymbolsAlgorithmFactory algorithmFactory,
        IFavoriteSymbolRepository favoriteSymbolRepository) : IPotentialSymbolsAnalyzingService
    {
        public async Task<BaseResponse<PotentialSymbolDto>> Analyze(PotentialSymbolRequest request)
        {
            Dictionary<string, IEnumerable<StockPriceHistoryDto>> priceHistory;
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

        private async Task<List<PotentialSymbolDto>> AnalyzeBaseOnPriceHistory(Dictionary<string, IEnumerable<StockPriceHistoryDto>> priceHistory, PotentialSymbolRequest request)
        {
            var favoriteSymbolIds = await favoriteSymbolRepository.Get();

            var potentialSymbols = new List<PotentialSymbolDto>();
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

        private static PotentialSymbolDto UpdateFavoriteAttribute(PotentialSymbolDto symbol, IEnumerable<string> favoriteSymbolIds)
        {
            return symbol with { IsFavorite = favoriteSymbolIds.Contains(symbol.SymbolId) };
        }

        private static BaseResponse<PotentialSymbolDto> ConvertToResponse(IEnumerable<PotentialSymbolDto> potentialSymbols)
        {
            return new()
            {
                Data = potentialSymbols is null ? [] : potentialSymbols
            };
        }
    }
}
