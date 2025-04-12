using Application.Attributes;
using Application.Constants;
using Application.Services.Interfaces;
using Domain.Constants;
using Domain.Entities.Bases;
using Google.Cloud.Firestore;
using Infrastructure.Firebase.Firestore.Converters.Interfaces;
using Infrastructure.Repositories.Bases.Interfaces;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Reflection;
using static Application.Constants.ApplicationEnums;

namespace Infrastructure.Repositories.Bases
{
    [DIService(DIServiceLifetime.Scoped)]
    public class FirestoreBaseRepository<TEntity>(
        ILogger<FirestoreBaseRepository<TEntity>> logger,
        IToastManagerService toastManagerService,
        IMessageService messageService,
        IDataTypeConverterFactory dataTypeConverterFactory,
        FirestoreDb firestoreDb) : IBaseRepository<TEntity>
        where TEntity : StockBaseEntity
    {
        public async Task<List<TEntity>> GetAll()
        {
            try
            {
                var snapshot = await GetQuerySnapshot();

                return snapshot.Documents
                    .Select(x =>
                    {
                        var entity = ConvertTo(x);
                        entity = entity with { Id = x.Id };
                        return entity;
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ApplicationConsts.LoggedErrorMessage.ErrorMessageInReadingData} {nameof(TEntity)}");
                await toastManagerService.Show($"{messageService.GetMessage(MessageConstants.MSG_CannotAccessResourceToReadData)}, {ex.Message}");
                return [];
            }
        }

        public async Task<TEntity?> GetById(string id)
        {
            try
            {
                var documentSnapshot = await GetCollection().Document(id).GetSnapshotAsync();

                if (documentSnapshot is null || documentSnapshot.Exists)
                    return null;

                return ConvertTo(documentSnapshot);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ApplicationConsts.LoggedErrorMessage.ErrorMessageInReadingData} {nameof(TEntity)}");
                await toastManagerService.Show($"{messageService.GetMessage(MessageConstants.MSG_CannotAccessResourceToReadData)}, {ex.Message}");
                return null;
            }
        }

        public async Task<bool> Create(TEntity entity)
        {
            try
            {
                var documentReference = GetCollection().Document();

                var result = await documentReference.SetAsync(ConvertFrom(entity));

                return result != null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ApplicationConsts.LoggedErrorMessage.ErrorMessageInSavingData} {nameof(TEntity)}");
                await toastManagerService.Show($"{messageService.GetMessage(MessageConstants.MSG_CannotAccessResourceToWriteData)}, {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Delete(string id)
        {
            try
            {
                var documentReference = GetCollection().Document(id);

                var result = await documentReference.DeleteAsync();

                return result != null;
            }

            catch (Exception ex)
            {
                logger.LogError(ex, $"{ApplicationConsts.LoggedErrorMessage.ErrorMessageInDeletingData} {nameof(TEntity)}");
                await toastManagerService.Show($"{messageService.GetMessage(MessageConstants.MSG_CannotAccessResourceToDeleteData)}, {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Update(TEntity entity)
        {
            try
            {
                var documentReference = GetCollection().Document(entity.Id);

                var result = await documentReference.SetAsync(ConvertFrom(entity));

                return result != null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ApplicationConsts.LoggedErrorMessage.ErrorMessageInSavingData} {nameof(TEntity)}");
                await toastManagerService.Show($"{messageService.GetMessage(MessageConstants.MSG_CannotAccessResourceToWriteData)}, {ex.Message}");
                return false;
            }
        }

        private TEntity ConvertTo(DocumentSnapshot documentSnapshot)
        {
            var entity = Activator.CreateInstance<TEntity>() ?? throw new InvalidOperationException($"Cannot create an instance of {typeof(TEntity).Name}");

            var properties = entity.GetType().GetProperties();
            foreach (var property in properties)
            {
                var converter = dataTypeConverterFactory.GetConverter(property.PropertyType);
                var propertyValue = documentSnapshot.GetValue<string>(property.Name);

                property.SetValue(entity, converter.ConvertFrom(propertyValue ?? string.Empty));
            }

            return entity;
        }

        private Dictionary<string, object> ConvertFrom(TEntity entity)
        {
            var dictionary = new Dictionary<string, object>();

            var properties = entity.GetType().GetProperties();
            foreach (var property in properties)
            {
                var converter = dataTypeConverterFactory.GetConverter(property.PropertyType);
                var propertyValue = property.GetValue(entity);

                dictionary.Add(property.Name, converter.ConvertTo(propertyValue));
            }

            return dictionary;
        }

        private async Task<QuerySnapshot> GetQuerySnapshot()
        {
            var collection = GetCollection();
            var snapshot = await collection.GetSnapshotAsync();

            return snapshot;
        }

        private CollectionReference GetCollection()
        {
            return firestoreDb
                .Collection(GetEntityName());
        }

        private static string GetEntityName()
        {
            var entityName = typeof(TEntity).GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;

            return entityName ?? nameof(TEntity);
        }
    }
}
