namespace Application.Settings
{
    public record ScheduleSettings
    {
        public required int FetchDataIntervalInSecond { get; init; }
    }
}
