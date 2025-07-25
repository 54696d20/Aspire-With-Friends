using AspireApp.MasterDataService.Data;
using Dapper;
using Wolverine;

namespace AspireApp.MasterDataService.Messages.Commands;

public class UpdateLocationHandler
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IMessageBus _bus;

    public UpdateLocationHandler(IDbConnectionFactory connectionFactory, IMessageBus bus)
    {
        _connectionFactory = connectionFactory;
        _bus = bus;
    }

    public async Task<bool> Handle(UpdateLocationCommand command)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = "UPDATE Locations SET Name = @Name, Type = @Type, ParentId = @ParentId WHERE Id = @Id";
        var rows = await connection.ExecuteAsync(sql, new { command.Name, command.Type, command.ParentId, command.Id });

        // Optionally publish an event here
        // if (rows > 0) await _bus.PublishAsync(new LocationChangedNotificationModel { Name = command.Name });

        return rows > 0;
    }
} 