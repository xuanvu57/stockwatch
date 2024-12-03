namespace stockwatch.Configurations.Models
{
    public record ScheduleSettings
    {
        public required int FetchDataIntervalInSecond { get; init; }
    }
}
