using System.ComponentModel;
using System.Globalization;

namespace stockwatch.Models
{
    public record LatestPriceModel : INotifyPropertyChanged
    {
        public string SymbolId { get; init; } = "N/A";
        public string Price { get; init; } = "N/A";
        public string AtTime { get; init; } = "N/A";

        public event PropertyChangedEventHandler? PropertyChanged;

        public void NotifyPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }

        public LatestPriceModel With(string? symbolId, decimal? price, DateTime atTime)
        {
            return this with
            {
                SymbolId = symbolId ?? "N/A",
                Price = price?.ToString("C", new CultureInfo("VN-vn")) ?? "N/A",
                AtTime = atTime.ToString("yyyy-MM-dd HH:mm:ss")
            };

        }
    }
}
