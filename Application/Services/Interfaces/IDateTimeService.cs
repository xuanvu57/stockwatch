namespace Application.Services.Interfaces
{
    public interface IDateTimeService
    {
        public Task<DateTime> GetCurrentBusinessDateTime();
        public Task<DateTime> GetCurrentSystemDateTime();
    }
}
