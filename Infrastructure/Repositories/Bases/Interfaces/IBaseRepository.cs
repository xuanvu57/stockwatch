using Domain.Entities.Bases;

namespace Infrastructure.Repositories.Bases.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : StockBaseEntity
    {
        Task<List<TEntity>> GetAll();
        Task<TEntity?> GetById(string id);
        Task<bool> Create(TEntity entity);
        Task<bool> Update(TEntity entity);
        Task<bool> Delete(string id);
    }
}
