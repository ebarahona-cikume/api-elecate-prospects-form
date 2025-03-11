using Microsoft.EntityFrameworkCore;

namespace ApiElecateProspectsForm.Context
{
    public class DbContextFactory(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        public ElecateDbContext CreateElecateDbContext()
        {
            string connectionString = BuildConnectionString();
            DbContextOptions<ElecateDbContext> options = BuildDbContextOptions<ElecateDbContext>(connectionString);
            return new ElecateDbContext(options);
        }

        public ProspectDbContext CreateProspectDbContext(string clientDatabaseId)
        {
            string connectionString = BuildConnectionString(clientDatabaseId);
            DbContextOptions<ProspectDbContext> options = BuildDbContextOptions<ProspectDbContext>(connectionString);
            return new ProspectDbContext(options);
        }

        private static DbContextOptions<TContext> BuildDbContextOptions<TContext>(string connectionString) where TContext : DbContext
        {
            DbContextOptionsBuilder<TContext> optionsBuilder = new();
            optionsBuilder.UseSqlServer(connectionString);
            return optionsBuilder.Options;
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
}