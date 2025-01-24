using Application.Attributes;
using Application.Extensions;
using Infrastructure.Clients.Settings;
using Infrastructure.Clients.Ssi.Constants;
using Infrastructure.Clients.Ssi.Extensions;
using Infrastructure.Clients.Ssi.Interfaces;
using Infrastructure.Clients.Ssi.Models;
using Microsoft.Extensions.Configuration;
using System.Text;
using static Application.Constants.ApplicationEnums;

namespace Infrastructure.Clients.Ssi
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
