namespace Infrastructure.Clients.Firebase.Firestore.Models.Responses
{
    public record FirestoreDocumentResponse : FirestoreBaseResponse
    {
        public Dictionary<string, Dictionary<string, object>> Fields { get; init; } = [];
    }
}
