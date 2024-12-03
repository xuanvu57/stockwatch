using Infrastructure.Clients.Ssi.Models;

namespace Infrastructure.Clients.Ssi.Interfaces
{
    public interface ISsiClient
    {
        public Task<BaseResponse<DailyStockPriceResponse[]>> DailyStockPrice(DateOnly fromDate, DateOnly toDate, string symbol = "", int? pageIndex = null, int? pageSize = null);
    }
}
