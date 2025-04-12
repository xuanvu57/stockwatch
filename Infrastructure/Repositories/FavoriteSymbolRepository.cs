using Application.Attributes;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Repositories.Bases.Interfaces;
using static Application.Constants.ApplicationEnums;

namespace Infrastructure.Repositories
{
    [DIService(DIServiceLifetime.Scoped)]
    public class FavoriteSymbolRepository(IDateTimeService dateTimeService, IBaseRepository<FavoriteSymbolEntity> baseRepository) : IFavoriteSymbolRepository
    {
        public async Task<IList<string>> Get()
        {
            var favoriteSymbols = await baseRepository.GetAll();

            return favoriteSymbols.Count > 0 ?
                favoriteSymbols.Select(x => x.SymbolId).ToList() :
                [];
        }

        public async Task<bool> Add(string symbolId)
        {
            var exsitingSymbol = await GetBySymbolId(symbolId);

            if (exsitingSymbol is not null)
                return true;

            var currentTime = await dateTimeService.GetCurrentSystemDateTime();
            var favoriteSymbol = new FavoriteSymbolEntity()
            {
                SymbolId = symbolId,
                AtTime = currentTime,
            };

            return await baseRepository.Create(favoriteSymbol);
        }

        public async Task<bool> Remove(string symbolId)
        {
            var exsitingSymbol = await GetBySymbolId(symbolId);

            if (exsitingSymbol is null)
                return true;

            return await baseRepository.Delete(exsitingSymbol.Id);
        }

        private async Task<FavoriteSymbolEntity?> GetBySymbolId(string symbolId)
        {
            var favoriteSymbols = await baseRepository.GetAll();

            return favoriteSymbols.Find(x => x.SymbolId == symbolId);
        }
    }
}
