using Application.Attributes;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Repositories.Interfaces;
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
        BaseRepository<FavoriteSymbolRepository, FavoriteSymbolEntity>(logger, localFileService, toastManagerService), IFavoriteSymbolRepository
    {
        public async Task<IList<string>> Get()
        {
            var favoriteSymbols = await GetAll();

            return favoriteSymbols.Count > 0 ?
                favoriteSymbols.Select(x => x.Id).ToList() :
                [];
        }

        public async Task<bool> Add(string symbolId)
        {
            var exsitingSymbolIds = await Get();

            if (exsitingSymbolIds.Contains(symbolId))
                return true;

            var favoriteSymbol = new FavoriteSymbolEntity()
            {
                Id = symbolId,
                AtTime = DateTime.Now,
            };

            return await Create(favoriteSymbol);
        }

        public async Task<bool> Remove(string symbolId)
        {
            return await Delete(symbolId);
        }
    }
}
