using Microsoft.Maui.Storage;

namespace Infrastructure.Repositories.Bases
{
    public abstract class BaseFileRepository(string fileName)
    {
        protected string FilePath { get; } = Path.Combine(FileSystem.AppDataDirectory, fileName);
    }
}
