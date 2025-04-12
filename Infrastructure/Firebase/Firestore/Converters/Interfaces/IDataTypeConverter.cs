namespace Infrastructure.Firebase.Firestore.Converters.Interfaces
{
    public interface IDataTypeConverter
    {
        string ConvertTo(object? value);

        object? ConvertFrom(string value);
    }
}
