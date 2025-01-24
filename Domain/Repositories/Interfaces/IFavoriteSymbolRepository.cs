namespace Domain.Repositories.Interfaces
{
    public interface IFavoriteSymbolRepository
    {
        public Task<IList<string>> Get();
        public Task<bool> Add(string symbolId);
        public Task<bool> Remove(string symbolId);
    }
}
