using Application.Services.Interfaces;

namespace Infrastructure.Repositories.Bases
{
    public abstract class BaseFileRepository(ILocalFileService localFileService, string fileName)
    {
        protected string FilePath { get; } = Path.Combine(localFileService.GetRootDirectory(), fileName);
    }
}
