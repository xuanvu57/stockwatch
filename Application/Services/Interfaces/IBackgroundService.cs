namespace Application.Services.Interfaces
{
    public interface IBackgroundService
    {
        bool IsRunning { get; }
        string ServiceName { get; }

        void Restart();
        void Stop();
        void AddSubscriber(IBackgroundServiceSubscriber subscriber);
        void RemoveSubscriber(IBackgroundServiceSubscriber subscriber);
    }
}
