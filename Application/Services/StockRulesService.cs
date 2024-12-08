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
    }
}
