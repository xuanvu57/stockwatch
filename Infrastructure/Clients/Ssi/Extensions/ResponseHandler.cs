using Infrastructure.Clients.Ssi.Exceptions;
using Infrastructure.Clients.Ssi.Models;
using System.Net;
using System.Text.Json;

namespace Infrastructure.Clients.Ssi.Extensions
{
    public static class ResponseHandler
    {
        private readonly static JsonSerializerOptions serializeOptions = new() { PropertyNameCaseInsensitive = true };

        public static async Task<BaseResponse<T>> ConvertToBaseResponse<T>(this HttpResponseMessage response) where T : class
        {
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<BaseResponse<T>>(responseContent, serializeOptions)!;

                return responseData;
            }

            throw new SsiException($"There is an error with {response.StatusCode}");
        }

        public static async Task<AccessTokenResponse> ConvertToAuthenticationResponse(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<AccessTokenResponse>(responseContent, serializeOptions)!;

                if (responseData.Status == (int)HttpStatusCode.OK)
                {
                    return responseData;
                }
            }

            throw new SsiException($"There is an error with {response.StatusCode}");
        }
    }
}
