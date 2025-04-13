using Infrastructure.Clients.Firebase.Firestore.Converters.Interfaces;

namespace Infrastructure.Clients.Firebase.Firestore.Converters
{
    public class DecimalConverter : IDataTypeConverter
    {
        public string ConvertTo(object? value)
        {
            if (value is null)
            {
                return string.Empty;
            }

            if (value is decimal decimalValue)
            {
                return decimalValue.ToString();
            }

            throw new InvalidOperationException("Invalid type for DecimalConverter");
        }

        public object? ConvertFrom(string value)
        {
            try
            {
                if (value == string.Empty)
                {
                    return null;
                }

                return decimal.Parse(value);
            }
            catch
            {
                throw new InvalidOperationException("Invalid type for DecimalConverter");
            }
        }
    }
}
