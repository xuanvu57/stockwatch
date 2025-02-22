namespace Infrastructure.Clients.Ssi.Models.Requests
{
    public record DailyStockPriceRequest
    {
        public string Symbol { get; init; } = string.Empty;
        public required string FromDate { get; init; }
        public required string ToDate { get; init; }
        public int PageIndex { get; init; }
        public int PageSize { get; init; }
    }
}
