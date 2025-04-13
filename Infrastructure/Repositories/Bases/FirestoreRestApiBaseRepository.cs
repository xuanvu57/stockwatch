using Application.Attributes;
using Application.Constants;
using Application.Services.Interfaces;
using Domain.Constants;
using Domain.Entities.Bases;
using Infrastructure.Clients.Firebase.Firestore.Converters.Interfaces;
using Infrastructure.Clients.Firebase.Firestore.Interfaces;
using Infrastructure.Clients.Firebase.Firestore.Models.Responses;
using Infrastructure.Repositories.Bases.Interfaces;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Reflection;
using static Application.Constants.ApplicationEnums;

namespace Infrastructure.Repositories.Bases
{
    [DIService(DIServiceLifetime.Scoped)]
    public class FirestoreRestApiBaseRepositoryy<TEntity>(
        ILogger<FirestoreRestApiBaseRepositoryy<TEntity>> logger,
        IToastManagerService toastManagerService,
        IMessageService messageService,
        IDataTypeConverterFactory dataTypeConverterFactory,
        IFirestoreClient firestoreClient) : IBaseRepository<TEntity>
        where TEntity : StockBaseEntity
    {
        public async Task<List<TEntity>> GetAll()
        {
            try
            {
                var collection = await GetCollection().ExecuteAsync();

                return collection.Documents
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
                var document = await GetCollection().Document(id).ExecuteAsync();

                if (document is null)
                    return null;

                return ConvertTo(document);
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
                var document = GetCollection().Document();

                var result = await document.SetValueAsync(ConvertFrom(entity));

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
                var document = GetCollection().Document(id);

                await document.DeleteAsync();

                return true;
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
                var document = GetCollection().Document(entity.Id);

                var result = await document.SetValueAsync(ConvertFrom(entity));

                return result != null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ApplicationConsts.LoggedErrorMessage.ErrorMessageInSavingData} {nameof(TEntity)}");
                await toastManagerService.Show($"{messageService.GetMessage(MessageConstants.MSG_CannotAccessResourceToWriteData)}, {ex.Message}");
                return false;
            }
        }

        private TEntity ConvertTo(FirestoreDocumentResponse document)
        {
            var entity = Activator.CreateInstance<TEntity>() ?? throw new InvalidOperationException($"Cannot create an instance of {typeof(TEntity).Name}");

            var properties = entity.GetType().GetProperties();
            foreach (var property in properties)
            {
                var field = document.Fields[property.Name];

                var converter = dataTypeConverterFactory.GetConverter(property.PropertyType);
                var propertyValue = field?.FirstOrDefault().Value.ToString();

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

        private IFirestoreCollection GetCollection()
        {
            return firestoreClient.Collection(GetEntityName());
        }

        private static string GetEntityName()
        {
            var entityName = typeof(TEntity).GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;

            return entityName ?? nameof(TEntity);
        }
    }
}
