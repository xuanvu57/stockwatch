namespace stockwatch.Services.Interfaces
{
    public interface IFloatingViewMovingService
    {
        void InitFloatingWindow((int height, int width) screenSize, IFloatingViewMovingHandlerService floatingViewMovingHandler);
        void SetTouchDownPosition(int x, int y);
        void MoveFloatingWindow(int x, int y);
        bool ConsiderOfMovingActionAfterUntouch(int x, int y);
        (int x, int y) GetLatestPosition();
    }
}
