namespace Infrastructure.Clients.Ssi.Models
{
    public record SecuritiesRequest
    {
        public string Market { get; init; } = string.Empty;
        public int PageIndex { get; init; }
        public int PageSize { get; init; }
    }
}
