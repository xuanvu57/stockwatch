namespace Application.Dtos.Bases
{
    public abstract record StockPriceHistoryBaseDto : StockPriceBaseDto
    {
        public decimal HighestPrice { get; init; }
        public decimal LowestPrice { get; init; }
    }
}
