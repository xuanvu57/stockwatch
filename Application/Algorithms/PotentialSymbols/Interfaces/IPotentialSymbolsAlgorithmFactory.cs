using static Domain.Constants.StockWatchEnums;

namespace Application.Algorithms.PotentialSymbols.Interfaces
{
    public interface IPotentialSymbolsAlgorithmFactory
    {
        public IPotentialSymbolsAlgorithm CreateAlgorithm(PotentialAlgorithm algorithm);
    }
}
