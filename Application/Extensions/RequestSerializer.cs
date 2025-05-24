using System.Text.Json;
using static Application.Constants.ApplicationEnums;

namespace Application.Extensions
{
    public static class RequestSerializer
    {
        private readonly static JsonSerializerOptions serializeOptions = new() { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public static string Serialize<T>(T request, RequestInputTypes requestInputType) where T : class
        {
            if (requestInputType == RequestInputTypes.JsonBody)
            {
                return JsonSerializer.Serialize(request, serializeOptions);
            }
            else
            {
                var properties = request.GetType().GetProperties();

                var uriParameters = new List<string>();
                foreach (var property in properties)
                {
                    var value = property.GetValue(request, null);

                    if (value is not null)
                    {
                        uriParameters.Add($"{property.Name}={Uri.EscapeDataString(value.ToString() ?? string.Empty)}");
                    }
                }

                return string.Join("&", uriParameters);
            }
        }
    }
}
