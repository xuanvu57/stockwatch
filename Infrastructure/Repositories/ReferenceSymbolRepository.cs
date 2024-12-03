using Application.Attributes;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities;
using Infrastructure.Repositories.Bases;
using System.Text.Json;
using static Domain.Constants.StockWatchEnums;

namespace Infrastructure.Repositories
{
    [DIService(DIServiceLifetime.Scoped)]
    public class ReferenceSymbolRepository(IToastManagerService toastManagerService) : BaseFileRepository("ReferenceSymbol.json"), IReferenceSymbolRepository
    {
        public async Task<ReferenceSymbolInfo?> Get()
        {
            try
            {
                var rawData = await File.ReadAllTextAsync(FilePath);
                return JsonSerializer.Deserialize<ReferenceSymbolInfo>(rawData)!;
            }
            catch (Exception ex)
            {
                await toastManagerService.Show($"Cannot read data, {ex.Message}");
                return null;
            }
        }

        public async Task Save(ReferenceSymbolInfo symbol)
        {
            try
            {
                var serializedData = JsonSerializer.Serialize(symbol);
                await File.WriteAllTextAsync(FilePath, serializedData);
            }
            catch (Exception ex)
            {
                await toastManagerService.Show($"Cannot write data, {ex.Message}");
            }
        }
    }
}
