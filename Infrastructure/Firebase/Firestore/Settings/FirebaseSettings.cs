namespace Infrastructure.Firebase.Firestore.Settings
{
    public record FirebaseSettings
    {
        public required string ProjectId { get; init; }
        public required Dictionary<string, object> Credential { get; init; }
    }
}
