namespace Infrastructure.Clients.Ssi.Models.Responses
{
    public record SecuritiesResponse
    {
        public required string Market { get; init; }
        public required string Symbol { get; init; }
        public string StockName { get; init; } = string.Empty;
        public string StockEnName { get; init; } = string.Empty;
    }
}
