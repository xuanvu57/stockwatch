namespace Infrastructure.Clients.Ssi.Interfaces
{
    public interface ISsiClientTokenManager
    {
        public Task<string> GetToken();
    }
}
