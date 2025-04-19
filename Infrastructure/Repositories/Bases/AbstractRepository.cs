using Domain.Entities.Bases;
using System.ComponentModel;
using System.Reflection;

namespace Infrastructure.Repositories.Bases
{
    public abstract class AbstractRepository<TEntity> where TEntity : StockBaseEntity
    {
        protected string GetEntityName()
        {
            var entityName = typeof(TEntity).GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;

            return entityName ?? nameof(TEntity);
        }
    }
}
