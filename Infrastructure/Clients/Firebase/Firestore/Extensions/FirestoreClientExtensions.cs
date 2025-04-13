using Application.Extensions;
using Infrastructure.Clients.Firebase.Firestore.Constants;
using Infrastructure.Clients.Firebase.Firestore.Exceptions;
using System.Text;
using System.Text.Json;
using static Application.Constants.ApplicationEnums;

namespace Infrastructure.Clients.Firebase.Firestore.Extensions
{
    public static class FirestoreClientExtensions
    {
        private readonly static JsonSerializerOptions serializeOptions = new() { PropertyNameCaseInsensitive = true };

        public static async Task<TResponse> ExecuteGetMethod<TResponse>(this HttpClient httpClient, string endpoint)
            where TResponse : class
        {
            var response = await httpClient.GetAsync(endpoint);

            return await ConvertToResponse<TResponse>(response);
        }

        public static async Task<TResponse> ExecuteGetMethod<TRequest, TResponse>(this HttpClient httpClient, TRequest request, string endpoint)
            where TRequest : class
            where TResponse : class
        {
            var parameters = RequestSerializer.Serialize(request, RequestInputTypes.Parameter);
            var endpointWithQueryParam = endpoint.Contains('?')
                ? $"{endpoint}&{parameters}"
                : $"{endpoint}?{parameters}";

            return await httpClient.ExecuteGetMethod<TResponse>(endpointWithQueryParam);
        }

        public static async Task<TResponse> ExecutePostMethod<TRequest, TResponse>(this HttpClient httpClient, TRequest request, string endpoint)
            where TRequest : class
            where TResponse : class
        {
            var content = new StringContent(RequestSerializer.Serialize(request, RequestInputTypes.JsonBody), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(endpoint, content);

            return await ConvertToResponse<TResponse>(response);
        }

        public static async Task<TResponse> ExecutePatchMethod<TRequest, TResponse>(this HttpClient httpClient, TRequest request, string endpoint)
            where TRequest : class
            where TResponse : class
        {
            var content = new StringContent(RequestSerializer.Serialize(request, RequestInputTypes.JsonBody), Encoding.UTF8, "application/json");

            var response = await httpClient.PatchAsync(endpoint, content);

            return await ConvertToResponse<TResponse>(response);
        }

        private static async Task<TResponse> ConvertToResponse<TResponse>(HttpResponseMessage response) where TResponse : class
        {
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<TResponse>(responseContent, serializeOptions)!;

                return responseData;
            }

            throw new FirestoreException($"{FirestoreConstants.Response.DefaultResponseNonSuccessMessage} {response.StatusCode}");
        }
    }
}
