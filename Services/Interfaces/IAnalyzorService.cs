using stockwatch.Models;
using stockwatch.Models.StockWatchModels;

namespace stockwatch.Services.Interfaces
{
    public interface IAnalyzorService
    {
        public Task Analyze(SymbolInfo symbol, ReferenceSymbolInfo targetSymbol);
    }
}
