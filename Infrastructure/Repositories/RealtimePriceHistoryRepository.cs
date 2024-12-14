using Application.Attributes;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities;
using Infrastructure.Repositories.Bases;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using static Domain.Constants.StockWatchEnums;

namespace Infrastructure.Repositories
{
    [DIService(DIServiceLifetime.Scoped)]
    public class RealtimePriceHistoryRepository(
        ILogger<RealtimePriceHistoryRepository> logger,
        ILocalFileService localFileService) : BaseFileRepository(localFileService, "RealtimePriceHistory.json"), IRealtimePriceHistoryRepository
    {
        public async Task<IList<RealtimePriceHistoryEntity>> Get(int symbolId, DateTime fromTime, DateTime toTime)
        {
            var prices = new List<RealtimePriceHistoryEntity>();

            return await Task.FromResult(prices);
        }

        public async Task Save(RealtimePriceHistoryEntity priceHistory)
        {
            try
            {
                var serializedData = JsonSerializer.Serialize(priceHistory);
                await File.WriteAllLinesAsync(FilePath, [serializedData]);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in saving {nameof(RealtimePriceHistoryEntity)}");
            }
        }
    }
}
