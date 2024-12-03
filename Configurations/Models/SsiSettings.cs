namespace stockwatch.Configurations.Models
{
    public record SsiSettings
    {
        public required string SsiBaseAddress { get; init; }
        public required string ConsumerId { get; init; }
        public required string ConsumerSecrect { get; init; }
    }
}
