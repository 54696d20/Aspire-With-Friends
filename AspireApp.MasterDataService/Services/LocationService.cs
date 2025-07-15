using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AspireApp.MasterDataService.Interfaces;
using AspireApp.MasterDataService.Models;
using Dapper;

namespace AspireApp.MasterDataService.Services
{
    public class LocationService : ILocationService
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public LocationService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            return await connection.QueryAsync<Location>("SELECT * FROM Locations");
        }

        public async Task<Location?> GetByIdAsync(int id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            return await connection.QueryFirstOrDefaultAsync<Location>("SELECT * FROM Locations WHERE Id = @Id", new { Id = id });
        }

        public async Task<int> CreateAsync(Location location)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var query = "INSERT INTO Locations (Name) VALUES (@Name); SELECT CAST(SCOPE_IDENTITY() as int)";
            return await connection.ExecuteScalarAsync<int>(query, location);
        }

        public async Task<bool> UpdateAsync(Location location)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var query = "UPDATE Locations SET Name = @Name WHERE Id = @Id";
            var rows = await connection.ExecuteAsync(query, location);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var query = "DELETE FROM Locations WHERE Id = @Id";
            var rows = await connection.ExecuteAsync(query, new { Id = id });
            return rows > 0;
        }
    }
}
