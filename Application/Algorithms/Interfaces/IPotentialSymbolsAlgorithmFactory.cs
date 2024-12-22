using static Domain.Constants.StockWatchEnums;

namespace Application.Algorithms.Interfaces
{
    public interface IPotentialSymbolsAlgorithmFactory
    {
        public IPotentialSymbolsAlgorithm CreateAlgorithm(PotentialAlgorithm algorithm);
    }
}
