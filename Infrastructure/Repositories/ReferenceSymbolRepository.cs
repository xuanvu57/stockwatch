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
    public class ReferenceSymbolRepository(ILogger<ReferenceSymbolRepository> logger, ILocalFileService localFileService, IToastManagerService toastManagerService) :
        BaseFileRepository(localFileService, "ReferenceSymbol.json"), IReferenceSymbolRepository
    {
        public async Task<ReferenceSymbolEntity?> Get()
        {
            try
            {
                var rawData = await File.ReadAllTextAsync(FilePath);
                return JsonSerializer.Deserialize<ReferenceSymbolEntity>(rawData)!;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in getting {nameof(ReferenceSymbolEntity)}");

                await toastManagerService.Show($"Cannot read data, {ex.Message}");
                return null;
            }
        }

        public async Task Save(ReferenceSymbolEntity symbol)
        {
            try
            {
                var serializedData = JsonSerializer.Serialize(symbol);
                await File.WriteAllTextAsync(FilePath, serializedData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in saving {nameof(ReferenceSymbolEntity)}");

                await toastManagerService.Show($"Cannot write data, {ex.Message}");
            }
        }
    }
}
