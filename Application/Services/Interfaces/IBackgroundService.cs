namespace Application.Services.Interfaces
{
    public interface IBackgroundService
    {
        public bool IsRunning { get; }
        void Start();
        void Stop();
        void Subscribe(IBackgroundServiceSubscriber subscriber);
        void Unsubscribe(IBackgroundServiceSubscriber subscriber);
    }
}
