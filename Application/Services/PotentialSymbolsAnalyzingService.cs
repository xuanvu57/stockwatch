using Application.Algorithms.PotentialSymbols.Fatories.Interfaces;
using Application.Attributes;
using Application.Dtos;
using Application.Dtos.Bases;
using Application.Dtos.Requests;
using Application.Services.Interfaces;
using Application.Settings;
using Domain.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using static Application.Constants.ApplicationEnums;
using static Domain.Constants.StockWatchEnums;

namespace Application.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class PotentialSymbolsAnalyzingService(
        IConfiguration configuration,
        IDateTimeService dateTimeService,
        IPriceHistoryCollectingService priceHistoryCollectingService,
        IPotentialSymbolsAlgorithmFactory algorithmFactory,
        IFavoriteSymbolRepository favoriteSymbolRepository) : IPotentialSymbolsAnalyzingService
    {
        public async Task<BaseResponse<PotentialSymbolDto>> Analyze(PotentialSymbolRequest request)
        {
            var potentialSymbolSettings = configuration.GetRequiredSection(nameof(PotentialSymbolSettings)).Get<PotentialSymbolSettings>()!;

            var monthsToAnalyze = potentialSymbolSettings.MaxMonthsToAnalyze;
            var maxSymbolsFromMarket = potentialSymbolSettings.MaxSymbolsFromMarket == 0 ? int.MaxValue : potentialSymbolSettings.MaxSymbolsFromMarket;

            monthsToAnalyze = Math.Min(request.MonthsToAnalyze, monthsToAnalyze);
            maxSymbolsFromMarket = Math.Min(request.MaxSymbolsFromMarket, maxSymbolsFromMarket);
            var advancedData = IsAdvanceDataNecessary(request);

            Dictionary<string, IEnumerable<StockPriceHistoryDto>> priceHistory;
            if (request.Market is null)
            {
                priceHistory = await priceHistoryCollectingService.GetBySymbols(
                    request.Symbols,
                    monthsToAnalyze,
                    advancedData);
            }
            else
            {
                priceHistory = await priceHistoryCollectingService.GetByMarket(
                    request.Market.Value,
                    maxSymbolsFromMarket,
                    monthsToAnalyze,
                    advancedData);
            }

            var potentialSymbols = await AnalyzeBaseOnPriceHistory(priceHistory, request);

            return await ConvertToResponse(potentialSymbols);
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

        private async Task<BaseResponse<PotentialSymbolDto>> ConvertToResponse(IEnumerable<PotentialSymbolDto> potentialSymbols)
        {
            var currentTime = await dateTimeService.GetCurrentSystemDateTime();
            return new()
            {
                Data = potentialSymbols is null ? [] : potentialSymbols,
                AtTime = currentTime
            };
        }

        private static bool IsAdvanceDataNecessary(PotentialSymbolRequest request)
        {
            switch (request.PriceType)
            {
                case PriceType.Price:
                case PriceType.OpenPrice:
                case PriceType.ClosePrice:
                case PriceType.HighestPrice:
                case PriceType.LowestPrice:
                    return false;
                default: return true;
            }
            ;
        }

        private static PotentialSymbolDto UpdateFavoriteAttribute(PotentialSymbolDto symbol, IEnumerable<string> favoriteSymbolIds)
        {
            return symbol with { IsFavorite = favoriteSymbolIds.Contains(symbol.SymbolId) };
        }
    }
}
