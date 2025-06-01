using Application.Dtos;
using Application.Dtos.Bases;

namespace Application.Services.Interfaces
{
    public interface IRealtimePriceService
    {
        public Task ConnectAsync();
        public Task DisconnectAsync();
        public Task<BaseResponse<StockPriceInRealtimeDto>> GetBySymbolId(string symbolId);
    }
}
