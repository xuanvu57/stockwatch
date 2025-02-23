using Application.Dtos.Bases;

namespace Application.Dtos
{
    public record StockPriceHistoryDto : StockPriceHistoryBaseDto
    {
        public required DateOnly AtDate { get; init; }
    }
}
