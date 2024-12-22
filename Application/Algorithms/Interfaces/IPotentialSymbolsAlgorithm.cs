using Application.Dtos;
using Application.Dtos.Requests;

namespace Application.Algorithms.Interfaces
{
    public interface IPotentialSymbolsAlgorithm
    {
        public PotentialSymbol? Verify(IEnumerable<StockPriceHistory> priceHistories, PotentialSymbolRequest request);
    }
}
