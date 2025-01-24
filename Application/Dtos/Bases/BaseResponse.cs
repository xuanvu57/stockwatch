using static Domain.Constants.StockWatchEnums;

namespace Application.Dtos.Bases
{
    public record BaseResponse<T> where T : class
    {
        public IEnumerable<T> Data { get; init; } = [];
        public DateTime AtTime { get; init; } = DateTime.Now;
        public ResponseStatus Status { get; init; }
    }
}
