using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AspireApp.MasterDataService.Data
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<SqlConnection> CreateConnectionAsync()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}