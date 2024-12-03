namespace Infrastructure.Clients.Ssi.Models
{
    public record AccessTokenRequest
    {
        public required string ConsumerId { get; init; }
        public required string ConsumerSecret { get; init; }
    }
}
