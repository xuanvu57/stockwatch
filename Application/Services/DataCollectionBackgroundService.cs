using Application.Attributes;
using Application.Constants;
using static Application.Constants.ApplicationEnums;

namespace Application.Services
{
    [DIService(DIServiceLifetime.Singleton)]
    public class DataCollectionBackgroundService
    {
        public bool IsRunning { get; private set; } = false;
        public string ServiceName => ApplicationConsts.DataCollectionBackgroundServiceName;


    }
}
