using Application.Dtos;
using static Domain.Constants.StockWatchEnums;

namespace Application.Services.Interfaces
{
    public interface IPotentialSymbolsAnalyzingService
    {
        public Task<IEnumerable<PotentialSymbol>> Analyze(string market, int months, decimal expectedAmplitudeInPercentage, GroupPriceDataBy groupPriceDataBy);
    }
}
