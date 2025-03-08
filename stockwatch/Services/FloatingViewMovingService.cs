using Application.Attributes;
using stockwatch.Constants;
using stockwatch.Services.Interfaces;
using static Application.Constants.ApplicationEnums;

namespace stockwatch.Services
{
    [DIService(DIServiceLifetime.Singleton)]
    public class FloatingViewMovingService : IFloatingViewMovingService
    {
        private (int x, int y) currentTouchPosition;
        private (int x, int y) latestTouchDownPosition;
        private (int x, int y) floatingWindowPosition;
        private (int height, int width) screenSize;

        private IFloatingViewMovingHandlerService? floatingViewMovingHandler;

        public void InitFloatingWindow((int height, int width) screenSize, IFloatingViewMovingHandlerService floatingViewMovingHandler)
        {
            this.screenSize = screenSize;
            floatingWindowPosition = GetLatestPosition();
            this.floatingViewMovingHandler = floatingViewMovingHandler;
        }

        public void SetTouchDownPosition(int x, int y)
        {
            latestTouchDownPosition = (x, y);
            currentTouchPosition = (x, y);
        }

        public void MoveFloatingWindow(int x, int y)
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

            floatingViewMovingHandler!.MovingHandler(floatingWindowPosition);
        }

        public bool ConsiderOfMovingActionAfterUntouch(int x, int y)
        {
            var movingDistance = GetDistance(latestTouchDownPosition, (x, y));
            var acceptOfMoving = movingDistance > DisplayConstants.MinimumDistanceToConsiderMoving;

            MoveFloatingWindowToTheSides();
            floatingViewMovingHandler!.MovingHandler(floatingWindowPosition);

            return acceptOfMoving;
        }

        public (int x, int y) GetLatestPosition()
        {
            if (floatingViewMovingHandler is null)
            {
                var defaultPositionX = screenSize.width - DisplayConstants.FloatingWindowWidth;
                var defaultPositionY = (screenSize.height / 2) - (DisplayConstants.FloatingWindowHeight / 2);
                return (defaultPositionX, defaultPositionY);
            }
            return floatingWindowPosition;
        }

        private void EnsureFloatingWindowInsideScreenView()
        {
            floatingWindowPosition.x = Math.Max(0, Math.Min(screenSize.width - DisplayConstants.FloatingWindowWidth, floatingWindowPosition.x));
            floatingWindowPosition.y = Math.Max(0, Math.Min(screenSize.height - DisplayConstants.FloatingWindowHeight, floatingWindowPosition.y));
        }

        private void MoveFloatingWindowToTheSides()
        {
            if (floatingWindowPosition.x + (DisplayConstants.FloatingWindowWidth / 2) < screenSize.width / 2)
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
