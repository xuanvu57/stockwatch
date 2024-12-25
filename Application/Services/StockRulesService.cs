using Domain.Constants;

namespace Application.Services
{
    public static class StockRulesService
    {
        public static DateOnly GetLatestAvailableDate()
        {
            var currentDate = DateTime.Now.Date;

            var latestAvailableDate = currentDate.DayOfWeek switch
            {
                DayOfWeek.Saturday => currentDate.AddDays(-1),
                DayOfWeek.Sunday => currentDate.AddDays(-2),
                _ => currentDate
            };

            return DateOnly.FromDateTime(latestAvailableDate);
        }

        public static decimal CalculatePercentage(decimal value, decimal referencedValue) => (100 * value / referencedValue) - 100;

        public static DateOnly GetBeginningDateOfWeek(DateOnly date)
        {
            if (date.DayOfWeek == StockWatchConstants.BeginningDateOfWeek)
                return date;

            var beginningDate = date;
            while (beginningDate.DayOfWeek != StockWatchConstants.BeginningDateOfWeek)
            {
                beginningDate = beginningDate.AddDays(-1);
            }

            return beginningDate;
        }

        public static DateOnly GetBeginningDateOfMonth(DateOnly date) => new(date.Year, date.Month, 1);
    }
}
