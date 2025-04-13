namespace Infrastructure.Clients.Firebase.Firestore.Models.Responses
{
    public record FirestoreCollectionResponse
    {
        public List<FirestoreDocumentResponse> Documents { get; init; } = [];
    }
}
