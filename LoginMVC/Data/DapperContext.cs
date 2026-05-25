using Microsoft.Data.SqlClient;
using System.Data;

namespace LoginMVC.Data;

public sealed class DapperContext
{
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("ConnectionString")
            ?? throw new InvalidOperationException(
                "Connection string 'ConnectionString' was not found.");
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}