using Application.Dtos;

namespace stockwatch.Services.Interfaces
{
    public interface IFloatingViewAnimationService
    {
        void UpdateContent(object view, SymbolAnalyzingResultDto? symbolAnalyzingResult);
        void TouchFloatView(object view);
        void DropFloatView(object view, int positionX);
    }
}
