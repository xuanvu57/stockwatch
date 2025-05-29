using Infrastructure.Clients.Firebase.Firestore.Models.Responses;

namespace Infrastructure.Clients.Firebase.Firestore.Interfaces
{
    public interface IFirestoreDocument
    {
        IFirestoreCollection Collection(string collectionId);

        Task<FirestoreDocumentResponse> GetAsync();

        Task<FirestoreDocumentResponse> SetValueAsync(Dictionary<string, object> fields);

        Task DeleteAsync();
    }
}
