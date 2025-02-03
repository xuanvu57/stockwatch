using static Application.Constants.ApplicationEnums;

namespace Application.Dtos.Bases
{
    public record BaseResponse<T> where T : class
    {
        public IEnumerable<T> Data { get; init; } = [];
        public required DateTime AtTime { get; init; }
        public ResponseStatus Status { get; init; } = ResponseStatus.Success;
    }
}
