namespace Application.Repositories.Interfaces
{
    public interface IFavoriteSymbolRepository
    {
        public Task<IList<string>> Get();
        public Task<bool> Save(string symbolId);
        public Task<bool> Remove(string symbolId);
    }
}
