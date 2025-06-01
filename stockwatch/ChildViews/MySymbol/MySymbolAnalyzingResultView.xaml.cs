using Application.Dtos;
using stockwatch.Constants;
using stockwatch.Models;

namespace stockwatch.ChildViews.MySymbol;

public partial class MySymbolAnalyzingResultView : ContentView
{
    public LatestPriceModel LatestPrice { get; private set; } = new();

    public MySymbolAnalyzingResultView()
    {
        InitializeComponent();
    }

    public void ClearLatestData(string defaultSymbolId = "")
    {
        var symbolId = string.IsNullOrEmpty(defaultSymbolId)
            ? DisplayConstants.NotAvailableValue
            : defaultSymbolId;

        SetLatestData(new SymbolAnalyzingResultDto()
        {
            SymbolId = symbolId,
        });
    }

    public void SetLatestData(SymbolAnalyzingResultDto symbolAnalyzingResult)
    {
        LatestPrice = LatestPrice.With(
            symbolAnalyzingResult.SymbolId,
            symbolAnalyzingResult.Price,
            symbolAnalyzingResult.Percentage,
            symbolAnalyzingResult.PercentageInDay,
            symbolAnalyzingResult.AtTime);

        LatestPrice.NotifyPropertyChanged();
    }
}