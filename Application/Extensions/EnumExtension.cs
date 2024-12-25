using Humanizer;

namespace Application.Extensions
{
    public static class Enum<T> where T : Enum
    {
        public static IEnumerable<string> ToDescriptions()
        {
            var enumValues = Enum.GetValues(typeof(T)).Cast<T>();

            return enumValues.Select(x => x.Humanize()).ToList();
        }


        public static T ToEnum(object? enumDescription)
        {
            if (!Enum.TryParse(typeof(T), enumDescription?.ToString(), out var value))
            {
                value = enumDescription?.ToString().DehumanizeTo(typeof(T));
            }

            return (T)(value ?? 0);
        }
    }
}
