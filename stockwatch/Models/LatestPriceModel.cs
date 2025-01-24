using System.ComponentModel;

namespace stockwatch.Models
{
    public record LatestPriceModel : INotifyPropertyChanged
    {
        public string SymbolId { get; init; } = "N/A";
        public decimal? Price { get; init; }
        public decimal? Percentage { get; init; }
        public DateTime? AtTime { get; init; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void NotifyPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }

        public LatestPriceModel With(string symbolId, decimal? price, decimal? percentage, DateTime atTime)
        {
            return this with
            {
                SymbolId = symbolId,
                Price = price,
                Percentage = percentage,
                AtTime = atTime
            };
        }
    }
}
