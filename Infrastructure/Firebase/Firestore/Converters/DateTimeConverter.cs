using Infrastructure.Firebase.Firestore.Converters.Interfaces;
using System.Globalization;

namespace Infrastructure.Firebase.Firestore.Converters
{
    public class DateTimeConverter : IDataTypeConverter
    {
        const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

        public string ConvertTo(object? value)
        {
            if (value is null)
            {
                return string.Empty;
            }

            if (value is DateTime dateTimeValue)
            {
                return dateTimeValue.ToString(DateTimeFormat, CultureInfo.InvariantCulture);
            }

            throw new InvalidOperationException("Invalid type for DateTimeConverter");
        }

        public object? ConvertFrom(string value)
        {
            try
            {
                if (value == string.Empty)
                {
                    return (DateTime?)null;
                }

                return DateTime.ParseExact(value, DateTimeFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new InvalidOperationException("Invalid type for DateTimeConverter");
            }
        }
    }
}
