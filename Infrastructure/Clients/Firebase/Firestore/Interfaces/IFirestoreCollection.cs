using Infrastructure.Clients.Firebase.Firestore.Models.Responses;

namespace Infrastructure.Clients.Firebase.Firestore.Interfaces
{
    public interface IFirestoreCollection
    {
        Task<FirestoreCollectionResponse> ExecuteAsync();

        IFirestoreDocument Document(string documentId = "");
    }
}
