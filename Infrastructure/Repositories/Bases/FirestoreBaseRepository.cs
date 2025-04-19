using Application.Attributes;
using Application.Services.Interfaces;
using Domain.Entities.Bases;
using Google.Cloud.Firestore;
using Infrastructure.Clients.Firebase.Firestore.Constants;
using Infrastructure.Clients.Firebase.Firestore.Converters.Interfaces;
using Infrastructure.Clients.Firebase.Firestore.Exceptions;
using Microsoft.Extensions.Logging;
using static Application.Constants.ApplicationEnums;

namespace Infrastructure.Repositories.Bases
{
    [DIService(DIServiceLifetime.Skipped)]
    public class FirestoreBaseRepository<TEntity>(
        ILogger<FirestoreBaseRepository<TEntity>> logger,
        IToastManagerService toastManagerService,
        IMessageService messageService,
        IDataTypeConverterFactory dataTypeConverterFactory,
        FirestoreDb firestoreDb) : AbstractRepository<TEntity>(logger, toastManagerService, messageService)
        where TEntity : StockBaseEntity
    {
        protected override async Task<List<TEntity>> GetAllInternal()
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

        protected override async Task<TEntity?> GetByIdInternal(string id)
        {
            var documentSnapshot = await GetCollection().Document(id).GetSnapshotAsync();

            if (documentSnapshot is null || documentSnapshot.Exists)
                return null;

            return ConvertTo(documentSnapshot);
        }

        protected override async Task<bool> CreateInternal(TEntity entity)
        {
            var documentReference = GetCollection().Document();

            var result = await documentReference.SetAsync(ConvertFrom(entity));

            return result != null;
        }

        protected override async Task<bool> DeleteInternal(string id)
        {
            var documentReference = GetCollection().Document(id);

            var result = await documentReference.DeleteAsync();

            return result != null;
        }

        protected override async Task<bool> UpdateInternal(TEntity entity)
        {
            var documentReference = GetCollection().Document(entity.Id);

            var result = await documentReference.SetAsync(ConvertFrom(entity));

            return result != null;
        }

        private TEntity ConvertTo(DocumentSnapshot documentSnapshot)
        {
            var entity = Activator.CreateInstance<TEntity>() ?? throw new FirestoreException($"{FirestoreConstants.Exception.InitializeEntityErrorMessage} {typeof(TEntity).Name}");

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
    }
}
