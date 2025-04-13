namespace Infrastructure.Clients.Firebase.Firestore.Models.Responses
{
    public record FirestoreBaseResponse
    {
        public string Id => Name[(Name.LastIndexOf('/') + 1)..];
        public required string Name { get; init; }
        public required DateTime CreateTime { get; init; }
        public required DateTime UpdateTime { get; init; }
    }
}
