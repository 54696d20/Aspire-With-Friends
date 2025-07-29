using AspireApp.MasterDataService.Data;
using AspireApp.MasterDataService.Messages.Events;
using Dapper;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace AspireApp.MasterDataService.Messages.Commands;

public class UpdateLocationHandler
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IMessageBus _bus;
    private readonly ILogger<UpdateLocationHandler> _logger;

    public UpdateLocationHandler(IDbConnectionFactory connectionFactory, IMessageBus bus, ILogger<UpdateLocationHandler> logger)
    {
        _connectionFactory = connectionFactory;
        _bus = bus;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateLocationCommand command)
    {
        _logger.LogInformation("Updating location: {LocationId} - {LocationName} ({LocationType})", 
            command.Id, command.Name, command.Type);
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "UPDATE Locations SET Name = @Name, Type = @Type, ParentId = @ParentId WHERE Id = @Id";
            var rows = await connection.ExecuteAsync(sql, new { command.Name, command.Type, command.ParentId, command.Id });

            if (rows > 0)
            {
                _logger.LogInformation("Location updated successfully: {LocationId} - {LocationName}", command.Id, command.Name);
                await _bus.PublishAsync(new LocationUpdatedEvent(command.Id, command.Name, command.Type, command.ParentId));
            }
            else
            {
                _logger.LogWarning("Location not found for update: {LocationId}", command.Id);
            }

            return rows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update location: {LocationId} - {LocationName}", command.Id, command.Name);
            throw;
        }
    }
} 