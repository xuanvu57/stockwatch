namespace Infrastructure.Firebase.Firestore.Converters.Interfaces
{
    public interface IDataTypeConverterFactory
    {
        IDataTypeConverter GetConverter(Type type);
    }
}
