using Application.Attributes;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Repositories.Bases;
using Microsoft.Extensions.Logging;
using static Application.Constants.ApplicationEnums;

namespace Infrastructure.Repositories
{
    [DIService(DIServiceLifetime.Scoped)]
    public class FavoriteSymbolRepository(
        ILogger<FavoriteSymbolRepository> logger,
        ILocalFileService localFileService,
        IToastManagerService toastManagerService,
        IDateTimeService dateTimeService,
        IMessageService messageService) :
        BaseRepository<FavoriteSymbolRepository, FavoriteSymbolEntity>(logger, localFileService, toastManagerService, messageService), IFavoriteSymbolRepository
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

            var currentTime = await dateTimeService.GetCurrentSystemDateTime();
            var favoriteSymbol = new FavoriteSymbolEntity()
            {
                Id = symbolId,
                AtTime = currentTime,
            };

            return await Create(favoriteSymbol);
        }

        public async Task<bool> Remove(string symbolId)
        {
            return await Delete(symbolId);
        }
    }
}
