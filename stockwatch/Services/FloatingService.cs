using Application.Attributes;
using stockwatch.Constants;
using stockwatch.Services.Interfaces;
using static Application.Constants.ApplicationEnums;

namespace stockwatch.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class FloatingService : IFloatingService
    {
        private (int x, int y) currentTouchPosition;
        private (int x, int y) latestTouchDownPosition;
        private (int x, int y) floatingWindowPosition;
        private (int height, int width) screenSize;

        public void InitFloatingWindow((int height, int width) screenSize, (int x, int y) floatingWindowPosition)
        {
            this.screenSize = screenSize;
            this.floatingWindowPosition = floatingWindowPosition;
        }

        public (int, int)? ConsiderToMoveFloatingWindow(int x, int y)
        {
            var movingDistance = GetDistance(latestTouchDownPosition, (x, y));
            var acceptOfMoving = movingDistance > DisplayConstants.MinimumDistanceToConsiderMoving;

            if (acceptOfMoving)
            {
                MoveFloatingWindowToTheSides();
                return floatingWindowPosition;
            }

            return null;
        }

        public (int, int) MoveFloatingWindow(int x, int y)
        {
            var nowX = x;
            var nowY = y;
            var movedX = nowX - currentTouchPosition.x;
            var movedY = nowY - currentTouchPosition.y;

            currentTouchPosition.x = nowX;
            currentTouchPosition.y = nowY;
            floatingWindowPosition.x += movedX;
            floatingWindowPosition.y += movedY;

            EnsureFloatingWindowInsideScreenView();

            return floatingWindowPosition;
        }

        public void SetTouchDownPosition(int x, int y)
        {
            latestTouchDownPosition = (x, y);
            currentTouchPosition = (x, y);
        }

        private void EnsureFloatingWindowInsideScreenView()
        {
            floatingWindowPosition.x = Math.Max(0, Math.Min(screenSize.width - DisplayConstants.FloatingWindowWidth, floatingWindowPosition.x));
            floatingWindowPosition.y = Math.Max(0, Math.Min(screenSize.height - DisplayConstants.FloatingWindowHeight, floatingWindowPosition.y));
        }

        private void MoveFloatingWindowToTheSides()
        {
            if (floatingWindowPosition.x < screenSize.width / 2)
            {
                floatingWindowPosition.x = 0;
            }
            else
            {
                floatingWindowPosition.x = screenSize.width - DisplayConstants.FloatingWindowWidth;
            }
        }

        private static double GetDistance((int x, int y) point1, (int x, int y) point2)
        {
            return Math.Sqrt(Math.Pow(point2.x - point1.x, 2) + Math.Pow(point2.y - point1.y, 2));
        }
    }
}
