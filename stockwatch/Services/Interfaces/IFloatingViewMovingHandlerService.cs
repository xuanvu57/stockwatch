namespace stockwatch.Services.Interfaces
{
    public interface IFloatingViewMovingHandlerService
    {
        void MovingHandler((int x, int y) toPosition);
    }
}
