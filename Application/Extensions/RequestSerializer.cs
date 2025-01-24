using System.Reflection;
using System.Text.Json;
using static Application.Constants.ApplicationEnums;

namespace Application.Extensions
{
    public static class RequestSerializer
    {
        public static string Serialize<T>(T request, RequestInputTypes requestInputType = RequestInputTypes.JsonBody) where T : class
        {
            if (requestInputType == RequestInputTypes.JsonBody)
            {
                return JsonSerializer.Serialize(request);
            }
            else
            {
                var uriParameters = new List<string>();

                var type = request.GetType();
                IList<PropertyInfo> properties = new List<PropertyInfo>(type.GetProperties());

                foreach (var property in properties)
                {
                    var value = property.GetValue(request, null);

                    if (value is not null)
                    {
                        uriParameters.Add($"{property.Name}={value}");
                    }
                }

                return uriParameters.Count > 0 ? string.Join("&", uriParameters) : string.Empty;
            }
        }
    }
}
