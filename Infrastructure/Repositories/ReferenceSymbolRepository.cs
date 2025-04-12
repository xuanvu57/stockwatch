using Application.Attributes;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Repositories.Bases.Interfaces;
using static Application.Constants.ApplicationEnums;

namespace Infrastructure.Repositories
{
    [DIService(DIServiceLifetime.Scoped)]
    public class ReferenceSymbolRepository(IBaseRepository<ReferenceSymbolEntity> baseRepository) : IReferenceSymbolRepository
    {
        public async Task<ReferenceSymbolEntity?> Get()
        {
            var entities = await baseRepository.GetAll();

            return entities.FirstOrDefault();
        }

        public async Task Save(ReferenceSymbolEntity entity)
        {
            var existingEntities = await baseRepository.GetAll();

            foreach (var existingEntity in existingEntities)
            {
                await baseRepository.Delete(existingEntity.Id);
            }

            await baseRepository.Create(entity);
        }
    }
}
