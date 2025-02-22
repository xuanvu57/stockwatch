namespace Application.Services.Interfaces
{
    public interface IBackgroundServiceSubscriber
    {
        Task HandleBackgroundServiceEvent<T>(T data);
    }
}
