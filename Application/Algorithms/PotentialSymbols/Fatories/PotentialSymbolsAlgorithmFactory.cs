using Application.Algorithms.PotentialSymbols.Fatories.Interfaces;
using Application.Algorithms.PotentialSymbols.Interfaces;
using Application.Attributes;
using static Application.Constants.ApplicationEnums;
using static Domain.Constants.StockWatchEnums;

namespace Application.Algorithms.PotentialSymbols.Fatories
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
