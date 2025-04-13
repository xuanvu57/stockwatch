namespace Infrastructure.Clients.Firebase.Firestore.Interfaces
{
    public interface IFirestoreClient
    {
        IFirestoreCollection Collection(string collectionId);
    }
}
