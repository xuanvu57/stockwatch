using Application.Dtos.Bases;

namespace Application.Dtos
{
    public record StockPriceHistoryDto : StockPriceHistoryBaseDto
    {
        public required DateOnly AtDate { get; init; }
        public required decimal ChangedPrice { get; init; }
        public required decimal ChangedPricePercent { get; init; }
        public required decimal OpenPrice { get; init; }
        public required decimal ClosePrice { get; init; }
        public required decimal AveragePrice { get; init; }
        public required decimal TotalMatchVolumn { get; init; }
        public required decimal TotalMatchValue { get; init; }
    }
}
