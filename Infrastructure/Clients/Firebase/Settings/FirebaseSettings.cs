namespace Infrastructure.Clients.Firebase.Settings
{
    public record FirebaseSettings
    {
        public required string ProjectId { get; init; }
        public required string ApiKey { get; init; }
        public required string FirestoreEndpoint { get; init; }
        public required Dictionary<string, object> Credential { get; init; }
    }
}
