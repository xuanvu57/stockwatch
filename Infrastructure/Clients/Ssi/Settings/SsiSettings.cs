namespace Infrastructure.Clients.Settings
{
    public record SsiSettings
    {
        public required string SsiBaseAddress { get; init; }
        public required string ConsumerId { get; init; }
        public required string ConsumerSecrect { get; init; }
    }
}
