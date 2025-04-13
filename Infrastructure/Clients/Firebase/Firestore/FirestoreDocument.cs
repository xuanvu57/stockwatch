using Infrastructure.Clients.Firebase.Firestore.Extensions;
using Infrastructure.Clients.Firebase.Firestore.Interfaces;
using Infrastructure.Clients.Firebase.Firestore.Models.Requests;
using Infrastructure.Clients.Firebase.Firestore.Models.Responses;

namespace Infrastructure.Clients.Firebase.Firestore
{
    public class FirestoreDocument(HttpClient httpClient, string collectionId, string documentId) : IFirestoreDocument
    {
        private readonly string GetDocumentEndpoint = $@"{collectionId}/{documentId}";
        private readonly string CreateDocumentEndpoint = $"{collectionId}?documentId={documentId}";
        private readonly string DeleteDocumentEndpoint = $@"{collectionId}/{documentId}";
        private readonly string UpdateOrCreateDocumentEndpoint = $@"{collectionId}/{documentId}";

        public Task<FirestoreDocumentResponse> ExecuteAsync()
        {
            return httpClient.ExecuteGetMethod<FirestoreDocumentResponse>(GetDocumentEndpoint);
        }

        public Task<FirestoreDocumentResponse> SetValueAsync(Dictionary<string, object> fields)
        {
            var request = new FirestoreDocumentFieldsRequest
            {
                Fields = fields.Select(x => new KeyValuePair<string, Dictionary<string, object>>(x.Key, new Dictionary<string, object> { { "stringValue", x.Value } })).ToDictionary(x => x.Key, x => x.Value)
            };

            if (string.IsNullOrEmpty(documentId))
            {
                return httpClient.ExecutePostMethod<FirestoreDocumentFieldsRequest, FirestoreDocumentResponse>(request, CreateDocumentEndpoint);
            }
            else
            {
                return httpClient.ExecutePatchMethod<FirestoreDocumentFieldsRequest, FirestoreDocumentResponse>(request, UpdateOrCreateDocumentEndpoint);
            }
        }

        public Task DeleteAsync()
        {
            return httpClient.DeleteAsync(DeleteDocumentEndpoint);
        }
    }
}
