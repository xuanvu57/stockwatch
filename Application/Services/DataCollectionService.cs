using Application.Attributes;
using Application.Dtos;
using Application.Dtos.Requests;
using Application.Services.Interfaces;
using static Application.Constants.ApplicationEnums;

namespace Application.Services
{
    [DIService(DIServiceLifetime.Scoped)]
    public class DataCollectionService(
        IPriceHistoryCollectingService priceHistoryCollectingService) : IDataCollectionService
    {
        public async Task<string> CollectData(DataCollectionRequest dataCollectionRequest)
        {
            Dictionary<string, IEnumerable<StockPriceHistoryDto>> priceHistory;
            var advancedData = true;

            if (dataCollectionRequest.Market is null)
            {
                priceHistory = await priceHistoryCollectingService.GetBySymbols(
                    dataCollectionRequest.Symbols,
                    dataCollectionRequest.FromDate,
                    dataCollectionRequest.ToDate,
                    advancedData);
            }
            else
            {
                //TODO: correct the maxValue
                var maxValue = 1;
                priceHistory = await priceHistoryCollectingService.GetByMarket(
                    dataCollectionRequest.Market.Value,
                    maxValue,
                    dataCollectionRequest.FromDate,
                    dataCollectionRequest.ToDate,
                    advancedData);
            }

            await SaveDataToDatabase(priceHistory);

            return $"{priceHistory}";
        }

        private async Task SaveDataToDatabase(Dictionary<string, IEnumerable<StockPriceHistoryDto>> priceHistory)
        {
            // Implement the logic to save the collected data to the database.
            // This could involve using a repository or a direct database context.
            // For example:
            // await stockPriceRepository.Save(priceHistory);
        }
    }
}
