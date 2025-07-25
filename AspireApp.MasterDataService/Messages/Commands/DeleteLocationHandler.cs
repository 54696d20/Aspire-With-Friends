using AspireApp.MasterDataService.Data;
using Dapper;
using Wolverine;

namespace AspireApp.MasterDataService.Messages.Commands;

public class DeleteLocationHandler
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IMessageBus _bus;

    public DeleteLocationHandler(IDbConnectionFactory connectionFactory, IMessageBus bus)
    {
        _connectionFactory = connectionFactory;
        _bus = bus;
    }

    public async Task<bool> Handle(DeleteLocationCommand command)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = "DELETE FROM Locations WHERE Id = @Id";
        var rows = await connection.ExecuteAsync(sql, new { command.Id });

        // Optionally publish an event here
        // if (rows > 0) await _bus.PublishAsync(new LocationChangedNotificationModel { Name = "Deleted" });

        return rows > 0;
    }
} 