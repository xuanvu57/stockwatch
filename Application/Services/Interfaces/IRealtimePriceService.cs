using Application.Dtos;
using Application.Dtos.Bases;

namespace Application.Services.Interfaces
{
    public interface IRealtimePriceService
    {
        public Task<BaseResponse<StockPriceInRealtimeDto>> GetBySymbolId(string symbolId);
    }
}
