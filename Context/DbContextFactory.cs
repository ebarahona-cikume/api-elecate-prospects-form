using ApiElecateProspectsForm.Context;

public class DbContextFactory
{
    private readonly IConfiguration _configuration;

    public DbContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ElecateDbContext CreateDbContext(string clientDatabaseId = null)
    {
        string connectionString;

        if (!string.IsNullOrEmpty(clientDatabaseId))
        {
            // Fetch connection string dynamically based on the client
            connectionString = GetClientConnectionString(clientDatabaseId);
        }
        else
        {
            // Use the default connection string
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        return new ElecateDbContext(connectionString);
    }

    private string GetClientConnectionString(string clientDatabaseId)
    {
        // Logic to retrieve the database connection string, e.g., from a config, another database, or a service
        return $"Server=your_server;Database=ClientDb_{clientDatabaseId};User Id=your_user;Password=your_password;";
    }
}
