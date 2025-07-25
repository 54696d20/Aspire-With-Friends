using AspireApp.MasterDataService.Data;
using AspireApp.MasterDataService.Models;
using Dapper;

namespace AspireApp.MasterDataService.Messages.Queries;

public class GetAllLocationsHandler
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetAllLocationsHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Location>> Handle(GetAllLocationsQuery query)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<Location>("SELECT * FROM Locations");
    }
} 