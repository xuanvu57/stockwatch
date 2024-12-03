namespace stockwatch.Repositories
{
    public abstract class BaseFileRepository(string fileName)
    {
        protected string FilePath { get; } = Path.Combine(FileSystem.AppDataDirectory, fileName);
    }
}
