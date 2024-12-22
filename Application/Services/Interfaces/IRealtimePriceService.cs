using Application.Dtos;
using Application.Dtos.Responses;

namespace Application.Services.Interfaces
{
    public interface IRealtimePriceService
    {
        public Task<BaseResponse<StockPriceInRealtime>> GetBySymbolId(string symbolId);
    }
}
