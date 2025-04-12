using Infrastructure.Firebase.Firestore.Converters.Interfaces;

namespace Infrastructure.Firebase.Firestore.Converters
{
    public class StringConverter : IDataTypeConverter
    {
        public string ConvertTo(object? value)
        {
            return value?.ToString() ?? string.Empty;
        }

        public object? ConvertFrom(string value)
        {
            return value;
        }
    }
}
