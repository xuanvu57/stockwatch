using Application.Dtos;
using Application.Dtos.Bases;
using Application.Dtos.Requests;

namespace Application.Services.Interfaces
{
    public interface IPotentialSymbolsAnalyzingService
    {
        public Task<BaseResponse<PotentialSymbolDto>> Analyze(PotentialSymbolRequest request);
    }
}
