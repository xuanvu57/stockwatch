using Application.Attributes;
using Application.Extensions;
using Infrastructure.Clients.Settings;
using Infrastructure.Clients.Ssi.Constants;
using Infrastructure.Clients.Ssi.DelegatingHandlers;
using Infrastructure.Clients.Ssi.Extensions;
using Infrastructure.Clients.Ssi.Interfaces;
using Infrastructure.Clients.Ssi.Models.Requests;
using Infrastructure.Clients.Ssi.Models.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static Application.Constants.ApplicationEnums;

namespace Infrastructure.Clients.Ssi
{
    [DIService(DIServiceLifetime.Scoped)]
    public class SsiClient : ISsiClient
    {
        private readonly HttpClient client;
        private readonly ILogger<SsiClient> logger;
        private static readonly SemaphoreSlim semaphoreSlim = new(1, 1);

        public SsiClient(ILogger<SsiClient> logger, IConfiguration configuration, ISsiClientTokenManager ssiClientTokenManager)
        {
            this.logger = logger;
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

            return await ExecuteGetMethod<DailyStockPriceRequest, DailyStockPriceResponse[]>(request, SsiConstants.Endpoints.DailyStockPrice);
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

            return await ExecuteGetMethod<IntradayOhlcRequest, IntradayOhlcResponse[]>(request, SsiConstants.Endpoints.IntradayOhlc);
        }

        public async Task<BaseResponse<DailyOhlcResponse[]>> DailyOhlc(DateOnly fromDate, DateOnly toDate, string symbol = "", int? pageIndex = null, int? pageSize = null, bool ascending = false)
        {
            var request = new DailyOhlcRequest()
            {
                FromDate = fromDate.ToString("dd/MM/yyyy"),
                ToDate = toDate.ToString("dd/MM/yyyy"),
                Symbol = symbol,
                PageIndex = pageIndex ?? SsiConstants.Request.DefaultPageIndex,
                PageSize = pageSize ?? SsiConstants.Request.DefaultPageSize,
                Ascending = ascending
            };

            return await ExecuteGetMethod<DailyOhlcRequest, DailyOhlcResponse[]>(request, SsiConstants.Endpoints.DailyOhlc);
        }

        public async Task<BaseResponse<SecuritiesResponse[]>> Securities(string market = "", int? pageIndex = null, int? pageSize = null)
        {
            var request = new SecuritiesRequest()
            {
                Market = market,
                PageIndex = pageIndex ?? SsiConstants.Request.DefaultPageIndex,
                PageSize = pageSize ?? SsiConstants.Request.DefaultPageSize,
            };

            return await ExecuteGetMethod<SecuritiesRequest, SecuritiesResponse[]>(request, SsiConstants.Endpoints.Securities);
        }

        private async Task<BaseResponse<TResponse>> ExecuteGetMethod<TRequest, TResponse>(TRequest request, string endpoint)
            where TRequest : class
            where TResponse : class
        {
            try
            {
                await semaphoreSlim.WaitAsync();

                await Task.Delay(TimeSpan.FromSeconds(SsiConstants.MinSecondBetweenApiCalls));

                var parameters = RequestSerializer.Serialize(request, RequestInputTypes.Parameter);
                var response = await client.GetAsync($"{endpoint}?{parameters}");

                return await response.ConvertToBaseResponse<TResponse>();
            }
            catch (Exception ex)
            {
                return HandleException<TResponse>(ex);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private BaseResponse<T> HandleException<T>(Exception ex) where T : class
        {
            logger.LogError(ex, $"Error in calling to Ssi API");

            var response = new BaseResponse<T>()
            {
                Message = ex.Message,
                Status = SsiConstants.ResponseStatus.SsiClientException
            };

            return response;
        }
    }
}
