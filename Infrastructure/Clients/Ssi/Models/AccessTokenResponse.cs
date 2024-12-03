namespace Infrastructure.Clients.Ssi.Models
{
    public record AccessTokenResponse
    {
        public required string Message { get; init; }
        public required int Status { get; init; }
        public AccessTokenResponseData? Data { get; init; }
    }

    public record AccessTokenResponseData
    {
        public string AccessToken { get; init; } = string.Empty;
    }
}
