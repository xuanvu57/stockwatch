using Application.Attributes;
using Application.Extensions;
using Infrastructure.Clients.Settings;
using Infrastructure.Clients.Ssi.Constants;
using Infrastructure.Clients.Ssi.DelegatingHandlers;
using Infrastructure.Clients.Ssi.Extensions;
using Infrastructure.Clients.Ssi.Interfaces;
using Infrastructure.Clients.Ssi.Models;
using Microsoft.Extensions.Configuration;
using static Domain.Constants.StockWatchEnums;

namespace Infrastructure.Clients.Ssi
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
                PageIndex = pageIndex ?? SsiConstants.Request.DefaultPageIndex,
                PageSize = pageSize ?? SsiConstants.Request.DefaultPageSize
            };

            var parameters = RequestSerializer.Serialize(request, RequestInputTypes.Parameter);
            var response = await client.GetAsync($"{SsiConstants.Endpoints.DailyStockPrice}?{parameters}");

            return await response.ConvertToBaseResponse<DailyStockPriceResponse[]>();
        }

        public async Task<BaseResponse<IntradayOhlcResponse[]>> IntradayOhlc(DateOnly fromDate, DateOnly toDate, string symbol = "", int? pageIndex = null, int? pageSize = null, bool ascending = false, int resolution = 1)
        {
            var request = new IntradayOhlcRequest()
            {
                FromDate = fromDate.ToString("dd/MM/yyyy"),
                ToDate = toDate.ToString("dd/MM/yyyy"),
                Symbol = symbol,
                PageIndex = pageIndex ?? SsiConstants.Request.DefaultPageIndex,
                PageSize = pageSize ?? SsiConstants.Request.DefaultPageSize,
                Ascending = ascending,
                Resolution = resolution
            };

            var parameters = RequestSerializer.Serialize(request, RequestInputTypes.Parameter);
            var response = await client.GetAsync($"{SsiConstants.Endpoints.IntradayOhlc}?{parameters}");

            return await response.ConvertToBaseResponse<IntradayOhlcResponse[]>();
        }

        public async Task<BaseResponse<DailyOhlcResponse[]>> DailyOhlc(DateOnly fromDate, DateOnly toDate, string symbol = "", int? pageIndex = null, int? pageSize = null, bool ascending = false)
        {
            var request = new IntradayOhlcRequest()
            {
                FromDate = fromDate.ToString("dd/MM/yyyy"),
                ToDate = toDate.ToString("dd/MM/yyyy"),
                Symbol = symbol,
                PageIndex = pageIndex ?? SsiConstants.Request.DefaultPageIndex,
                PageSize = pageSize ?? SsiConstants.Request.DefaultPageSize,
                Ascending = ascending
            };

            var parameters = RequestSerializer.Serialize(request, RequestInputTypes.Parameter);
            var response = await client.GetAsync($"{SsiConstants.Endpoints.DailyOhlc}?{parameters}");

            return await response.ConvertToBaseResponse<DailyOhlcResponse[]>();
        }

        private async Task<BaseResponse<T>> HandleException<T>(Exception ex) where T : class
        {
            var response = new BaseResponse<T>()
            {
                Message = ex.Message,
                Status = SsiConstants.ResponseStatus.SsiClientException
            };

            return response;
        }
    }
}
