namespace Infrastructure.Clients.Ssi.Models.Responses.BaseResponses
{
    public record BaseResponse<TData> where TData : class
    {
        public required string Message { get; init; }
        public required string Status { get; init; }
        public int? TotalRecord { get; init; }
        public TData? Data { get; init; }
    }
}
