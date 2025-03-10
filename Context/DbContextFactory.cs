using Microsoft.EntityFrameworkCore;

namespace ApiElecateProspectsForm.Context
{
    public class DbContextFactory(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

    public ElecateDbContext CreateDbContext(string? clientDatabaseId = null)
    {
        string connectionString = BuildConnectionString(clientDatabaseId);
        return new ElecateDbContext(connectionString);
    }

    private string BuildConnectionString(string? databaseName = null)
    {
        string serverName = _configuration["ServerName"] ?? "";
        string defaultDatabaseName = _configuration["DataBaseName"] ?? "";
        string connectionStringTemplate = _configuration.GetConnectionString("DefaultConnection") ?? "";

        return connectionStringTemplate
            .Replace("{ServerName}", serverName)
            .Replace("{DataBaseName}", databaseName ?? defaultDatabaseName);
    }
}