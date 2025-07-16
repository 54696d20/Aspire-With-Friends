using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AspireApp.MasterDataService.Interfaces;
using AspireApp.MasterDataService.Messages;
using AspireApp.MasterDataService.Models;
using AspireApp.Shared.Messaging.Models;
using Dapper;
using Wolverine;

namespace AspireApp.MasterDataService.Services
{
    public class LocationService : ILocationService
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IMessageBus _bus;
        private readonly ILogger<LocationService> _logger;
        
        public LocationService(
            IDbConnectionFactory connectionFactory, 
            IMessageBus bus, 
            ILogger<LocationService> logger)
        {
            _connectionFactory = connectionFactory;
            _bus = bus;
            _logger = logger;
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
            var query = "INSERT INTO Locations (Name, Type) VALUES (@Name, @Type); SELECT CAST(SCOPE_IDENTITY() as int)";
            var id = await connection.ExecuteScalarAsync<int>(query, location);
            location.Id = id;
            
            // Send notification
            _logger.LogInformation("✅Publishing LocationChangedNotification for {LocationName}", location.Name);

            try
            {
                var publishTask = _bus.PublishAsync(
                    new LocationChangedNotificationModel { Name = location.Name }
                ).AsTask();

                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(3));

                var completedTask = await Task.WhenAny(publishTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    _logger.LogError("❌ PublishAsync timed out for location: {LocationName}", location.Name);
                }
                else
                {
                    _logger.LogInformation("✅Published LocationChangedNotification for {LocationName}", location.Name);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "❌ Unexpected error during PublishAsync for {LocationName}", location.Name);
                throw;
            }

            return id;
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
