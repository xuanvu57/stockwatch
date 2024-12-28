using Application.Algorithms.Abstracts;
using Application.Dtos;
using Application.Dtos.Requests;
using Application.Services;

namespace Application.Algorithms
{
    public class AmplitudeAlgorithm : AbstractPotentialSymbolAlgorithm
    {
        protected override List<StockPriceHistory> FilterPriceHistory(IEnumerable<StockPriceHistory> priceHistories, PotentialSymbolRequest request)
        {
            return priceHistories
                    .Where(x => StockRulesService.CalculatePercentage(x.HighestPrice, x.LowestPrice) >= request.ExpectedPercentage)
                    .ToList();
        }
    }
}
