using Infrastructure.Clients.Firebase.Settings;

namespace Infrastructure.Clients.Firebase.Firestore.DelegatingHandlers
{
    public class ApiKeyDelegatingHandler : DelegatingHandler
    {
        private readonly FirebaseSettings firebaseSetting;

        public ApiKeyDelegatingHandler(FirebaseSettings firebaseSetting)
        {
            InnerHandler = new HttpClientHandler();

            this.firebaseSetting = firebaseSetting;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var uriBuilder = new UriBuilder(request.RequestUri!);

            if (string.IsNullOrEmpty(uriBuilder.Query))
            {
                uriBuilder.Query = $"key={firebaseSetting.ApiKey}";
            }
            else
            {
                uriBuilder.Query = $"{uriBuilder.Query}&key={firebaseSetting.ApiKey}";
            }

            request.RequestUri = uriBuilder.Uri;

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
