using Application.Algorithms.Interfaces;
using Application.Attributes;
using static Domain.Constants.StockWatchEnums;

namespace Application.Algorithms
{
    [DIService(DIServiceLifetime.Scoped)]
    public class PotentialSymbolsAlgorithmFactory : IPotentialSymbolsAlgorithmFactory
    {
        public IPotentialSymbolsAlgorithm CreateAlgorithm(PotentialAlgorithm algorithm)
        {
            return algorithm switch
            {
                PotentialAlgorithm.Amplitude => new AmplitudeAlgorithm(),
                _ => new ContinuouslyUpOrDownAlgorithm(),
            };
        }
    }
}
