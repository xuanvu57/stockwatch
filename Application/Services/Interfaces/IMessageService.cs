namespace Application.Services.Interfaces
{
    public interface IMessageService
    {
        string GetMessage(string messageKey, params object[] arguments);
    }
}
