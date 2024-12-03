using stockwatch.Clients.Ssi.Models;

namespace stockwatch.Clients.Ssi.Interfaces
{
    public interface ISsiClient
    {
        public Task<BaseResponse<DailyStockPriceResponse[]>> DailyStockPrice(DateOnly fromDate, DateOnly toDate, string symbol = "", int? pageIndex = null, int? pageSize = null);
    }
}
