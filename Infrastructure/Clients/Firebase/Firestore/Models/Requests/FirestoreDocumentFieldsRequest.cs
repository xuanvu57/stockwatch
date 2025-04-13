namespace Infrastructure.Clients.Firebase.Firestore.Models.Requests
{
    public record FirestoreDocumentFieldsRequest
    {
        public Dictionary<string, Dictionary<string, object>> Fields { get; init; } = [];
    }
}
