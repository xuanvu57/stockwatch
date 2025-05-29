using Application.Attributes;
using Application.Services.Interfaces;
using Domain.Entities.Bases;
using Infrastructure.Clients.Firebase.Firestore.Constants;
using Infrastructure.Clients.Firebase.Firestore.Converters.Interfaces;
using Infrastructure.Clients.Firebase.Firestore.Exceptions;
using Infrastructure.Clients.Firebase.Firestore.Interfaces;
using Infrastructure.Clients.Firebase.Firestore.Models.Responses;
using Microsoft.Extensions.Logging;
using static Application.Constants.ApplicationEnums;

namespace Infrastructure.Repositories.Bases
{
    [DIService(DIServiceLifetime.Scoped)]
    public class FirestoreRestApiBaseRepositoryy<TEntity>(
        ILogger<FirestoreRestApiBaseRepositoryy<TEntity>> logger,
        IToastManagerService toastManagerService,
        IMessageService messageService,
        IDataTypeConverterFactory dataTypeConverterFactory,
        IFirestoreClient firestoreClient) : AbstractRepository<TEntity>(logger, toastManagerService, messageService)
        where TEntity : StockBaseEntity
    {
        protected override async Task<List<TEntity>> GetAllInternal()
        {
            var collection = await GetCollection().GetAsync();

            return collection.Documents
                .Select(x =>
                {
                    var entity = ConvertTo(x);
                    entity = entity with { Id = x.Id };
                    return entity;
                })
                .ToList();
        }

        protected override async Task<TEntity?> GetByIdInternal(string id)
        {
            var document = await GetCollection().Document(id).GetAsync();

            if (document is null)
                return null;

            return ConvertTo(document);
        }

        protected override async Task<bool> CreateInternal(TEntity entity)
        {
            var document = GetCollection().Document();

            var result = await document.SetValueAsync(ConvertFrom(entity));

            return result != null;
        }

        protected override async Task<bool> DeleteInternal(string id)
        {
            var document = GetCollection().Document(id);

            await document.DeleteAsync();

            return true;
        }

        protected override async Task<bool> UpdateInternal(TEntity entity)
        {
            var document = GetCollection().Document(entity.Id);

            var result = await document.SetValueAsync(ConvertFrom(entity));

            return result != null;
        }

        private TEntity ConvertTo(FirestoreDocumentResponse document)
        {
            var entity = Activator.CreateInstance<TEntity>() ?? throw new FirestoreException($"{FirestoreConstants.Exception.InitializeEntityErrorMessage} {typeof(TEntity).Name}");

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
    }
}
