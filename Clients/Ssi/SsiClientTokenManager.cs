using Microsoft.Extensions.Configuration;
using stockwatch.Attributes;
using stockwatch.Clients.Ssi.Constants;
using stockwatch.Clients.Ssi.Extensions;
using stockwatch.Clients.Ssi.Interfaces;
using stockwatch.Clients.Ssi.Models;
using stockwatch.Configurations.Models;
using stockwatch.Extensions;
using System.Text;
using static stockwatch.Constants.StockWatchEnums;

namespace stockwatch.Clients.Ssi
{
    [DIService(DIServiceLifetime.Singleton)]
    public class SsiClientTokenManager(IConfiguration configuration) : ISsiClientTokenManager
    {
        private readonly SsiSettings ssiSettings = configuration.GetRequiredSection(nameof(SsiSettings)).Get<SsiSettings>()!;

        private string acccessToken = string.Empty;

        public async Task<string> GetToken()
        {
            if (string.IsNullOrEmpty(acccessToken))
            {
                var request = new AccessTokenRequest()
                {
                    ConsumerId = ssiSettings.ConsumerId,
                    ConsumerSecret = ssiSettings.ConsumerSecrect
                };

                var content = new StringContent(RequestSerializer.Serialize(request), Encoding.UTF8, "application/json");

                var client = new HttpClient()
                {
                    BaseAddress = new Uri(ssiSettings.SsiBaseAddress)
                };

                var response = await client.PostAsync(SsiConstants.Endpoints.AccessToken, content);

                var accessTokenResponse = await response.ConvertToAuthenticationResponse();

                acccessToken = accessTokenResponse.Data!.AccessToken;
            }

            return acccessToken;
        }

        public void SetToken(string token)
        {
            acccessToken = token;
        }
    }
}
