using Application.Dtos.Requests;

namespace Application.Services.Interfaces
{
    public interface IDataCollectionService
    {
        Task<string> CollectData(DataCollectionRequest dataCollectionRequest);
    }
}
