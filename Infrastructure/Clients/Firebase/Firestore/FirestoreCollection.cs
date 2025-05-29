using Infrastructure.Clients.Firebase.Firestore.Extensions;
using Infrastructure.Clients.Firebase.Firestore.Interfaces;
using Infrastructure.Clients.Firebase.Firestore.Models.Responses;

namespace Infrastructure.Clients.Firebase.Firestore
{
    public class FirestoreCollection(HttpClient httpClient, params string[] collectionId) : IFirestoreCollection
    {
        private readonly string GetDocumentsEndpoint = $"{collectionId}";

        public IFirestoreDocument Document(string documentId = "")
        {
            var firestoreDocument = new FirestoreDocument(httpClient, collectionId, documentId);
            return firestoreDocument;
        }

        public Task<FirestoreCollectionResponse> GetAsync()
        {
            return httpClient.ExecuteGetMethod<FirestoreCollectionResponse>(GetDocumentsEndpoint);
        }
    }
}
