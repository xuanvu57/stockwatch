using Application.Algorithms.PotentialSymbols.Interfaces;
using Application.Attributes;
using Application.Dtos;
using Application.Dtos.Bases;
using Application.Dtos.Requests;
using Application.Services.Interfaces;
using Application.Settings;
using Domain.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using static Application.Constants.ApplicationEnums;

namespace Application.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class PotentialSymbolsAnalyzingService(
        IConfiguration configuration,
        IPriceHistoryCollectingService priceHistoryCollectingService,
        IPotentialSymbolsAlgorithmFactory algorithmFactory,
        IFavoriteSymbolRepository favoriteSymbolRepository) : IPotentialSymbolsAnalyzingService
    {
        public async Task<BaseResponse<PotentialSymbolDto>> Analyze(PotentialSymbolRequest request)
        {
            var potentialSymbolSettings = configuration.GetRequiredSection(nameof(PotentialSymbolSettings)).Get<PotentialSymbolSettings>()!;

            Dictionary<string, IEnumerable<StockPriceHistoryDto>> priceHistory;
            if (request.Market is null)
            {
                priceHistory = await priceHistoryCollectingService.GetBySymbols(request.Symbols, potentialSymbolSettings.MaxMonthsToAnalyze);
            }
            else
            {
                priceHistory = await priceHistoryCollectingService.GetByMarket(
                    request.Market.Value,
                    potentialSymbolSettings.MaxSymbolsFromMarket == 0 ? int.MaxValue : potentialSymbolSettings.MaxSymbolsFromMarket,
                    potentialSymbolSettings.MaxMonthsToAnalyze);
            }

            var potentialSymbols = await AnalyzeBaseOnPriceHistory(priceHistory, request);

            return ConvertToResponse(potentialSymbols);
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
