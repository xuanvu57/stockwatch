using Application.Dtos;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace stockwatch.Models
{
    public class PotentialSymbolModel : INotifyPropertyChanged
    {
        public required string SymbolId { get; init; }

        public bool IsFavorite { get; private set; }
        public int MatchedRecordCount { get; init; }
        public decimal AverageAmplitude { get; init; }
        public decimal AverageAmplitudeInPercentage { get; init; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ChangeFavorite()
        {
            IsFavorite = !IsFavorite;
            OnPropertyChanged(nameof(IsFavorite));
        }

        public static PotentialSymbolModel FromPotentialSymbol(PotentialSymbol potentialSymbol)
        {
            return new()
            {
                SymbolId = potentialSymbol.SymbolId,
                AverageAmplitude = potentialSymbol.AverageAmplitude,
                AverageAmplitudeInPercentage = potentialSymbol.AverageAmplitudeInPercentage,
                MatchedRecordCount = potentialSymbol.MatchedRecordCount,
                IsFavorite = potentialSymbol.IsFavorite
            };
        }
    }
}
