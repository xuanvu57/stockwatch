using Application.Dtos;
using Application.Dtos.Requests;

namespace Application.Algorithms.PotentialSymbols.Interfaces
{
    public interface IPotentialSymbolsAlgorithm
    {
        public PotentialSymbolDto? Verify(IEnumerable<StockPriceHistoryDto> priceHistories, PotentialSymbolRequest request);
    }
}
