using Google.Cloud.Firestore;
using Infrastructure.Clients.Firebase.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Infrastructure.Configurations
{
    public static class FirebaseRegister
    {
        public static void RegisterFirestoreDb(this IServiceCollection serviceCollection)
        {
            var configuration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            var firebaseSettings = configuration.GetRequiredSection(nameof(FirebaseSettings)).Get<FirebaseSettings>()!;

            var jsonCredentials = JsonSerializer.Serialize(firebaseSettings.Credential);

            serviceCollection.AddSingleton(_ => new FirestoreDbBuilder
            {
                ProjectId = firebaseSettings.ProjectId,
                JsonCredentials = jsonCredentials
            }.Build());
        }
    }
}
