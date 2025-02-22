namespace Application.Services.Interfaces
{
    public interface IBackgroundService
    {
        void Start();
        void Stop();
        void Subscribe(IBackgroundServiceSubscriber subscriber);
        void Unsubscribe(IBackgroundServiceSubscriber subscriber);
    }
}
