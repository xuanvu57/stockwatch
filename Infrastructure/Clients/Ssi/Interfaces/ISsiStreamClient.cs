namespace Infrastructure.Clients.Ssi.Interfaces
{
    public interface ISsiStreamClient
    {
        public delegate Task OnDataReceived(string data);

        event OnDataReceived? OnDataReceivedHandler;
        Task ConnectAsync();
        void Disconnect();
        Task SwitchChannel(string channel);
    }
}
