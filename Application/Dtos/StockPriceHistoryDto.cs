using Application.Dtos.Bases;

namespace Application.Dtos
{
    public record StockPriceHistoryDto : StockPriceBaseDto
    {
        public required DateOnly AtDate { get; init; }
    }
}
