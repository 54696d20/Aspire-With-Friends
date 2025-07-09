using System.Data;
using System.Data.SqlClient;
using Dapper;
using AspireApp.MasterDataService.Models;

namespace AspireApp.MasterDataService.Data;

public class LocationRepository
{
    private readonly string _connectionString;

    public LocationRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("masterdatadb")!;
    }

    private IDbConnection Connection => new SqlConnection(_connectionString);

    public async Task<IEnumerable<Location>> GetAllAsync()
    {
        const string sql = "SELECT * FROM Locations";
        using var db = Connection;
        return await db.QueryAsync<Location>(sql);
    }

    public async Task<Location?> GetByIdAsync(int id)
    {
        const string sql = "SELECT * FROM Locations WHERE Id = @Id";
        using var db = Connection;
        return await db.QuerySingleOrDefaultAsync<Location>(sql, new { Id = id });
    }

    public async Task AddAsync(Location location)
    {
        const string sql = "INSERT INTO Locations (Name, Type, ParentId) VALUES (@Name, @Type, @ParentId)";
        using var db = Connection;
        await db.ExecuteAsync(sql, location);
    }
}