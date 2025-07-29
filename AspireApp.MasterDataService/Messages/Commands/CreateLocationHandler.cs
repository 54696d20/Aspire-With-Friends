using AspireApp.MasterDataService.Data;
using AspireApp.Shared.Messaging.Models;
using AspireApp.MasterDataService.Messages.Events;
using Dapper;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace AspireApp.MasterDataService.Messages.Commands;

public class CreateLocationHandler
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IMessageBus _bus;
    private readonly ILogger<CreateLocationHandler> _logger;

    public CreateLocationHandler(IDbConnectionFactory connectionFactory, IMessageBus bus, ILogger<CreateLocationHandler> logger)
    {
        _connectionFactory = connectionFactory;
        _bus = bus;
        _logger = logger;
    }

    public async Task<int> Handle(CreateLocationCommand command)
    {
        _logger.LogInformation("Creating location: {LocationName} ({LocationType})", command.Name, command.Type);
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "INSERT INTO Locations (Name, Type, ParentId) OUTPUT INSERTED.Id VALUES (@Name, @Type, @ParentId)";
            var id = await connection.ExecuteScalarAsync<int>(sql, new { command.Name, command.Type, command.ParentId });
            
            _logger.LogInformation("Location created successfully: {LocationId} - {LocationName}", id, command.Name);
            
            // Publish domain event
            await _bus.PublishAsync(new LocationCreatedEvent(id, command.Name, command.Type, command.ParentId));
            
            // Publish notification event (keeping for backward compatibility)
            await _bus.PublishAsync(new LocationChangedNotificationModel { Name = command.Name });

            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create location: {LocationName} ({LocationType})", command.Name, command.Type);
            throw;
        }
    }
}