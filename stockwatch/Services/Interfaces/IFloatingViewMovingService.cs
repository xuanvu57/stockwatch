namespace stockwatch.Services.Interfaces
{
    public interface IFloatingViewMovingService
    {
        void InitFloatingWindow((int height, int width) screenSize, (int x, int y) floatingWindowPosition, IFloatingViewMovingHandlerService floatingViewMovingHandler);
        void SetTouchDownPosition(int x, int y);
        void MoveFloatingWindow(int x, int y);
        bool ConsiderToMoveFloatingWindow(int x, int y);
    }
}
