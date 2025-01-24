using Application.Dtos.Bases;

namespace Application.Dtos
{
    public record StockPriceHistory : StockPriceBaseData
    {
        public required DateOnly AtDate { get; init; }
    }
}
