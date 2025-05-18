using Application.Algorithms.PotentialSymbols.Interfaces;
using static Domain.Constants.StockWatchEnums;

namespace Application.Algorithms.PotentialSymbols.Fatories.Interfaces
{
    public interface IPotentialSymbolsAlgorithmFactory
    {
        public IPotentialSymbolsAlgorithm CreateAlgorithm(PotentialAlgorithm algorithm);
    }
}
