using Infrastructure.Clients.Firebase.Firestore.Models.Responses;

namespace Infrastructure.Clients.Firebase.Firestore.Interfaces
{
    public interface IFirestoreDocument
    {
        Task<FirestoreDocumentResponse> ExecuteAsync();

        Task<FirestoreDocumentResponse> SetValueAsync(Dictionary<string, object> fields);

        Task DeleteAsync();
    }
}
