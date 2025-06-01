using Application.Attributes;
using Infrastructure.Clients.Ssi.Constants;
using Infrastructure.Clients.Ssi.Interfaces;
using Infrastructure.Clients.Ssi.Settings;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using Microsoft.Extensions.Configuration;
using static Application.Constants.ApplicationEnums;
using static Infrastructure.Clients.Ssi.Interfaces.ISsiStreamClient;

namespace Infrastructure.Clients.Ssi
{
    [DIService(DIServiceLifetime.Scoped)]
    public class SsiStreamClient : ISsiStreamClient
    {
        public event OnDataReceived? OnDataReceivedHandler;

        private readonly ISsiClientTokenManager ssiClientTokenManager;
        private readonly SsiSettings ssiSettings;

        private readonly HubConnection hubConnection;
        private readonly IHubProxy hubProxy;

        public SsiStreamClient(IConfiguration configuration, ISsiClientTokenManager ssiClientTokenManager)
        {
            this.ssiClientTokenManager = ssiClientTokenManager;
            ssiSettings = configuration.GetRequiredSection(nameof(SsiSettings)).Get<SsiSettings>()!;

            var hubEndpoint = $"{ssiSettings.SsiStreamBaseAddress}{ssiSettings.HubEndpoint}";

            hubConnection = new HubConnection(hubEndpoint);
            hubProxy = hubConnection.CreateHubProxy(ssiSettings.HubName);
            hubProxy.On<string>(SsiConstants.StreamMethods.Broadcast, OnBroadcast);
        }

        public async Task ConnectAsync()
        {
            await StartConnectionAsync();
        }

        public async Task SwitchChannel(string channel)
        {
            await StartConnectionAsync();
            if (hubConnection is null || hubConnection.State is not ConnectionState.Connected)
            {
                throw new InvalidOperationException("HubConnection is null or disconnected");
            }

            await hubProxy.Invoke(SsiConstants.StreamMethods.SwtichChannels, channel);
        }

        public void Disconnect()
        {
            if (hubConnection is null || hubConnection.State is not ConnectionState.Connected) return;

            hubConnection.Stop();
        }

        private async Task StartConnectionAsync()
        {
            if (hubConnection is null)
            {
                throw new InvalidOperationException("HubConnection is null");
            }

            if (hubConnection.State is not ConnectionState.Connected)
            {
                var accessToken = await GetAccessToken();
                hubConnection.Headers.Remove(SsiConstants.AuthorizationRequestHeader);
                hubConnection.Headers.Add(SsiConstants.AuthorizationRequestHeader, $"{SsiConstants.AuthorizationSchema} {accessToken}");

                hubConnection.Start(new WebSocketTransport()).Wait();
            }
        }

        private async Task<string?> GetAccessToken()
        {
            var accessToken = await ssiClientTokenManager.GetToken();
            return accessToken;
        }

        private void OnBroadcast(string data)
        {
            OnDataReceivedHandler?.Invoke(data);
        }
    }
}
