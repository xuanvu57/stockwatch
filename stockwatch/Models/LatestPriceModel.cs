using stockwatch.Constants;
using System.ComponentModel;

namespace stockwatch.Models
{
    public record LatestPriceModel : INotifyPropertyChanged
    {
        public string SymbolId { get; init; } = DisplayConstants.NotAvailableValue;
        public decimal? Price { get; init; }
        public decimal? Percentage { get; init; }
        public decimal? PercentageInDay { get; init; }
        public DateTime? AtTime { get; init; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void NotifyPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }

        public LatestPriceModel With(string symbolId, decimal? price, decimal? percentage, decimal? percentageInDay, DateTime atTime)
        {
            return this with
            {
                SymbolId = symbolId,
                Price = price,
                Percentage = percentage,
                PercentageInDay = percentageInDay,
                AtTime = atTime
            };
        }
    }
}
