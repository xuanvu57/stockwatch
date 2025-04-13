using Application.Attributes;
using Infrastructure.Clients.Firebase.Firestore.DelegatingHandlers;
using Infrastructure.Clients.Firebase.Firestore.Interfaces;
using Infrastructure.Clients.Firebase.Settings;
using Microsoft.Extensions.Configuration;
using static Application.Constants.ApplicationEnums;

namespace Infrastructure.Clients.Firebase.Firestore
{
    [DIService(DIServiceLifetime.Scoped)]
    public class FirestoreClient : IFirestoreClient
    {
        private readonly HttpClient httpClient;

        public FirestoreClient(IConfiguration configuration)
        {
            var firebaseSetting = configuration.GetRequiredSection(nameof(FirebaseSettings)).Get<FirebaseSettings>()!;

            httpClient = new HttpClient(new ApiKeyDelegatingHandler(firebaseSetting))
            {
                BaseAddress = new Uri(firebaseSetting.FirestoreEndpoint)
            };
        }

        public IFirestoreCollection Collection(string collectionId)
        {
            var firestoreCollection = new FirestoreCollection(httpClient, collectionId);
            return firestoreCollection;
        }
    }
}
