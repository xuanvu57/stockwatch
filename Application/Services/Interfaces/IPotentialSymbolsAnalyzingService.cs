using Application.Dtos;
using Application.Dtos.Requests;
using Application.Dtos.Responses;

namespace Application.Services.Interfaces
{
    public interface IPotentialSymbolsAnalyzingService
    {
        public Task<BaseResponse<PotentialSymbol>> Analyze(PotentialSymbolRequest request);
    }
}
