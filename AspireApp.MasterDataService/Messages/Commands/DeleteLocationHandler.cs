using AspireApp.MasterDataService.Data;
using AspireApp.MasterDataService.Messages.Events;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using Wolverine;

namespace AspireApp.MasterDataService.Messages.Commands;

public class DeleteLocationHandler
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IMessageBus _bus;
    private readonly ILogger<DeleteLocationHandler> _logger;
    private readonly IDistributedCache _cache;

    public DeleteLocationHandler(IDbConnectionFactory connectionFactory, IMessageBus bus, ILogger<DeleteLocationHandler> logger, IDistributedCache cache)
    {
        _connectionFactory = connectionFactory;
        _bus = bus;
        _logger = logger;
        _cache = cache;
    }

    public async Task<bool> Handle(DeleteLocationCommand command)
    {
        _logger.LogInformation("Deleting location: {LocationId}", command.Id);
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            // Get location name before deletion for event
            var locationName = await connection.QueryFirstOrDefaultAsync<string>(
                "SELECT Name FROM Locations WHERE Id = @Id", new { command.Id });
            
            var sql = "DELETE FROM Locations WHERE Id = @Id";
            var rows = await connection.ExecuteAsync(sql, new { command.Id });
            if (rows > 0)
            {
                _logger.LogInformation("Location deleted successfully: {LocationId} - {LocationName}", command.Id, locationName);
                
                // Invalidate cache
                await _cache.RemoveAsync("locations_all");
                await _cache.RemoveAsync($"location_{command.Id}");
                _logger.LogInformation("Invalidated cache after deleting location {LocationId}", command.Id);
                
                if (!string.IsNullOrEmpty(locationName))
                {
                    await _bus.PublishAsync(new LocationDeletedEvent(command.Id, locationName));
                }
            }
            else
            {
                _logger.LogWarning("Location not found for deletion: {LocationId}", command.Id);
            }
            return rows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete location: {LocationId}", command.Id);
            throw;
        }
    }
} 