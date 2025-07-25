using AspireApp.MasterDataService.Data;
using AspireApp.MasterDataService.Models;

namespace AspireApp.MasterDataService.Messages.Queries;

public class GetLocationByIdHandler
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetLocationByIdHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Location?> Handle(GetLocationByIdQuery query)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<Location>(
            "SELECT * FROM Locations WHERE Id = @Id", new { query.Id });
    }
} 