using Domain.Constants;

namespace Domain.Services
{
    public static class StockRulesService
    {
        public static DateOnly GetLatestAvailableDate(DateTime date)
        {
            var dateOnly = DateOnly.FromDateTime(date);
            return GetLatestAvailableDate(dateOnly);
        }

        public static DateOnly GetLatestAvailableDate(DateOnly date)
        {
            var latestAvailableDate = date.DayOfWeek switch
            {
                DayOfWeek.Saturday => date.AddDays(-1),
                DayOfWeek.Sunday => date.AddDays(-2),
                _ => date
            };

            return latestAvailableDate;
        }

        public static decimal CalculatePercentage(decimal value, decimal referencedValue) => (100 * value / referencedValue) - 100;

        public static DateOnly GetBeginningDateOfWeek(DateOnly date)
        {
            if (date.DayOfWeek == StockWatchConstants.BeginningDayOfWeek)
                return date;

            var beginningDate = date;
            while (beginningDate.DayOfWeek != StockWatchConstants.BeginningDayOfWeek)
            {
                beginningDate = beginningDate.AddDays(-1);
            }

            return beginningDate;
        }

        public static DateOnly GetBeginningDateOfMonth(DateOnly date) => new(date.Year, date.Month, 1);
    }
}
