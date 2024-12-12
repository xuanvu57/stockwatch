namespace Application.Services.Interfaces
{
    public interface ILoadingService
    {
        public Task Show(string message = "");
        public Task Hide();
    }
}
