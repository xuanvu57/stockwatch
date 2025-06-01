namespace Infrastructure.Clients.Ssi.Settings
{
    public record SsiSettings
    {

        public required string SsiBaseAddress { get; init; }
        public required string SsiStreamBaseAddress { get; init; }
        public required string HubEndpoint { get; init; }
        public required string HubName { get; init; }
        public required string ConsumerId { get; init; }
        public required string ConsumerSecrect { get; init; }
    }
}
