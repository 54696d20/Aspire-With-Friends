using AspireApp.MasterDataService.Data;
using AspireApp.Shared.Messaging.Models;
using Dapper;
using Wolverine;

namespace AspireApp.MasterDataService.Messages.Commands;

public class CreateLocationHandler
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IMessageBus _bus;

    public CreateLocationHandler(IDbConnectionFactory connectionFactory, IMessageBus bus)
    {
        _connectionFactory = connectionFactory;
        _bus = bus;
    }

    public async Task<int> Handle(CreateLocationCommand command)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = "INSERT INTO Locations (Name, Type, ParentId) OUTPUT INSERTED.Id VALUES (@Name, @Type, @ParentId)";
        var id = await connection.ExecuteScalarAsync<int>(sql, new { command.Name, command.Type, command.ParentId });
        
        // Publish notification event
        await _bus.PublishAsync(new LocationChangedNotificationModel { Name = command.Name });

        return id;
    }
}