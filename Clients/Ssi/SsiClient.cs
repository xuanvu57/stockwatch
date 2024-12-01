using Microsoft.Extensions.Configuration;
using stockwatch.Attributes;
using stockwatch.Clients.Ssi.Constants;
using stockwatch.Clients.Ssi.DelegatingHandlers;
using stockwatch.Clients.Ssi.Extensions;
using stockwatch.Clients.Ssi.Interfaces;
using stockwatch.Clients.Ssi.Models;
using stockwatch.Configurations.Models;
using stockwatch.Extensions;
using static stockwatch.Constants.StockWatchEnums;

namespace stockwatch.Clients.Ssi
{
    [DIService(DIServiceLifetime.Scoped)]
    public class SsiClient : ISsiClient
    {
        private readonly HttpClient client;

        public SsiClient(IConfiguration configuration, ISsiClientTokenManager ssiClientTokenManager)
        {
            var ssiSettings = configuration.GetRequiredSection(nameof(SsiSettings)).Get<SsiSettings>()!;

            client = new HttpClient(new AuthenticationDelegatingHandler(ssiClientTokenManager))
            {
                BaseAddress = new Uri(ssiSettings.SsiBaseAddress)
            };
        }

        public async Task<BaseResponse<DailyStockPriceResponse[]>> DailyStockPrice(DateOnly fromDate, DateOnly toDate, string symbol = "", int? pageIndex = null, int? pageSize = null)
        {
            var request = new DailyStockPriceRequest()
            {
                FromDate = fromDate.ToString("dd/MM/yyyy"),
                ToDate = toDate.ToString("dd/MM/yyyy"),
                Symbol = symbol,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var parameters = RequestSerializer.Serialize(request, RequestInputTypes.Parameter);
            var response = await client.GetAsync($"{SsiConstants.Endpoints.DailyStockPrice}?{parameters}");

            return await response.ConvertToBaseResponse<DailyStockPriceResponse[]>();
        }
    }
}
