namespace stockwatch.Services.Interfaces
{
    public interface IFloatingService
    {
        void InitFloatingWindow((int height, int width) screenSize, (int x, int y) floatingWindowPosition);
        void SetTouchDownPosition(int x, int y);
        (int, int) MoveFloatingWindow(int x, int y);
        (int, int)? ConsiderToMoveFloatingWindow(int x, int y);
    }
}
