using Application.Algorithms.PotentialSymbols.Abstracts;
using Application.Dtos;
using Application.Dtos.Requests;
using Domain.Services;

namespace Application.Algorithms.PotentialSymbols
{
    public class AmplitudeAlgorithm : AbstractPotentialSymbolAlgorithm
    {
        protected override List<StockPriceHistoryDto> FilterPriceHistory(IEnumerable<StockPriceHistoryDto> priceHistories, PotentialSymbolRequest request)
        {
            return priceHistories
                    .Where(x => StockRulesService.CalculatePercentage(x.HighestPrice, x.LowestPrice) >= request.ExpectedAmplitudePercentage)
                    .ToList();
        }
    }
}
