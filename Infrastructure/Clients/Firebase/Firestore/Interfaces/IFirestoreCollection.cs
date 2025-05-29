using Infrastructure.Clients.Firebase.Firestore.Models.Responses;

namespace Infrastructure.Clients.Firebase.Firestore.Interfaces
{
    public interface IFirestoreCollection
    {
        Task<FirestoreCollectionResponse> GetAsync();

        IFirestoreDocument Document(string documentId = "");
    }
}
