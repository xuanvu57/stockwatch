using stockwatch.Models.StockWatchModels;

namespace stockwatch.Models
{
    public record ReferenceSymbolInfo : SymbolInfo
    {
        public decimal FloorPrice { get; init; }
        public decimal CeilingPrice { get; init; }
    }
}
