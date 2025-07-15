using System.Data.SqlClient;

public interface IDbConnectionFactory
{
    Task<SqlConnection> CreateConnectionAsync();
}