namespace Application.Services.Interfaces
{
    public interface ILocalFileService
    {
        string GetRootDirectory();
        Task<string> ReadFileAsync(string filePath);
    }
}
