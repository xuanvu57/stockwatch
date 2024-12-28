using Application.Attributes;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities;
using Infrastructure.Repositories.Bases;
using Microsoft.Extensions.Logging;
using static Domain.Constants.StockWatchEnums;

namespace Infrastructure.Repositories
{
    [DIService(DIServiceLifetime.Scoped)]
    public class FavoriteSymbolRepository(
        ILogger<FavoriteSymbolRepository> logger,
        ILocalFileService localFileService,
        IToastManagerService toastManagerService) :
        BaseFileRepository<FavoriteSymbolRepository>(logger, localFileService, toastManagerService, "FavoriteSymbol.json"), IFavoriteSymbolRepository
    {
        public async Task<IList<string>> Get()
        {
            var favoriteSymbols = await ReadFromFile<FavoriteSymbolEntity>();

            return favoriteSymbols?.Count > 0 ?
                favoriteSymbols.Select(x => x.SymbolId).ToList() :
                [];
        }

        public async Task<bool> Save(string symbolId)
        {
            var exsitingFavoriteSymbolIds = await Get();

            if (exsitingFavoriteSymbolIds.Contains(symbolId))
                return true;

            var favoriteSymbol = new FavoriteSymbolEntity()
            {
                SymbolId = symbolId,
                AtTime = DateTime.Now,
            };

            return await SaveToFile([favoriteSymbol]);
        }

        public async Task<bool> Remove(string symbolId)
        {
            var favoriteSymbols = await ReadFromFile<FavoriteSymbolEntity>();

            if (favoriteSymbols is null)
                return true;

            var symbol = favoriteSymbols.Find(x => x.SymbolId == symbolId);
            if (symbol is null)
                return true;

            favoriteSymbols.Remove(symbol);
            return await SaveToFile(favoriteSymbols, true);
        }
    }
}
