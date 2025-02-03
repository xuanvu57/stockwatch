using Application.Attributes;
using Application.Services.Interfaces;
using static Application.Constants.ApplicationEnums;

namespace Application.Services
{
    [DIService(DIServiceLifetime.Singleton)]
    public class DateTimeService : IDateTimeService
    {
        public Task<DateTime> GetCurrentBusinessDateTime()
        {
            return Task.FromResult(DateTime.Now);
        }

        public Task<DateTime> GetCurrentSystemDateTime()
        {
            return Task.FromResult(DateTime.Now);
        }
    }
}
