using Infrastructure.Clients.Ssi.Constants;
using Infrastructure.Clients.Ssi.Interfaces;
using System.Net.Http.Headers;

namespace Infrastructure.Clients.Ssi.DelegatingHandlers
{
    public class AuthenticationDelegatingHandler : DelegatingHandler
    {
        private readonly ISsiClientTokenManager ssiClientTokenManager;

        public AuthenticationDelegatingHandler(ISsiClientTokenManager ssiClientTokenManager)
        {
            InnerHandler = new HttpClientHandler();

            this.ssiClientTokenManager = ssiClientTokenManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization is null)
            {
                await SetAccessToken(request);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task SetAccessToken(HttpRequestMessage httpRequestMessage)
        {
            var accessToken = await ssiClientTokenManager.GetToken();

            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(SsiConstants.AuthorizationSchema, accessToken);
        }
    }
}
