using Application.Attributes;
using Infrastructure.Clients.Firebase.Firestore.Converters.Interfaces;
using static Application.Constants.ApplicationEnums;

namespace Infrastructure.Clients.Firebase.Firestore.Converters
{
    [DIService(DIServiceLifetime.Singleton)]
    public class DataTypeConverterFactory : IDataTypeConverterFactory
    {
        private readonly Dictionary<Type, IDataTypeConverter> _converters;

        public DataTypeConverterFactory()
        {
            _converters = new Dictionary<Type, IDataTypeConverter>
            {
                { typeof(decimal), new DecimalConverter() },
                { typeof(DateTime), new DateTimeConverter() },
                { typeof(string), new StringConverter() },
            };
        }

        public IDataTypeConverter GetConverter(Type type)
        {
            if (_converters.TryGetValue(type, out var converter))
            {
                return converter;
            }

            throw new NotSupportedException($"No converter found for type {type}");
        }
    }
}
