namespace Application.Services.Interfaces
{
    public interface IBackgroundService
    {
        bool IsRunning { get; }

        void Restart();
        void Stop();
        void AddSubscriber(IBackgroundServiceSubscriber subscriber);
        void RemoveSubscriber(IBackgroundServiceSubscriber subscriber);
    }
}
