namespace stockwatch.Clients.Ssi.Interfaces
{
    public interface ISsiClientTokenManager
    {
        public Task<string> GetToken();
        public void SetToken(string token);
    }
}
