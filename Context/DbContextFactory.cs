using Microsoft.EntityFrameworkCore;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Services.DbContextFactory;

namespace ApiElecateProspectsForm.Context
{
    public class DbContextFactory(
        IConfiguration configuration,
        SqlServerDbContextOptionsFactory sqlServerFactory,
        PostgreSqlDbContextOptionsFactory postgreSqlFactory) : IDbContextFactory
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly SqlServerDbContextOptionsFactory _sqlServerFactory = sqlServerFactory;
        private readonly PostgreSqlDbContextOptionsFactory _postgreSqlFactory = postgreSqlFactory;

        public ElecateDbContext CreateElecateDbContext()
        {
            string connectionString = BuildConnectionString();
            DbContextOptions<ElecateDbContext> options = _sqlServerFactory.CreateDbContextOptions<ElecateDbContext>(connectionString);
            return new ElecateDbContext(options);
        }

        public ProspectDbContext CreateProspectDbContext(string clientDatabaseId)
        {
            string connectionString = BuildConnectionString(clientDatabaseId);
            DbContextOptions<ProspectDbContext> options = _sqlServerFactory.CreateDbContextOptions<ProspectDbContext>(connectionString);
            return new ProspectDbContext(options);
        }

        public SecretsDbContext CreateSecretsPostgresqlDbContext()
        {
            string connectionString = BuildConnectionString(null, "Postgresql");
            DbContextOptions<SecretsDbContext> options = _postgreSqlFactory.CreateDbContextOptions<SecretsDbContext>(connectionString);
            return new SecretsDbContext(options);
        }

        private string BuildConnectionString(string? databaseName = null, string? dbName = null)
        {
            string connectionString = "DefaultConnection";
            if (dbName == "Postgresql")
            {
                connectionString = "PostgresqlConnection";
            }
            string serverName = _configuration["ServerName"] ?? "";
            string defaultDatabaseName = _configuration["DataBaseName"] ?? "";
            string connectionStringTemplate = _configuration.GetConnectionString(connectionString) ?? "";

            return connectionStringTemplate
                .Replace("{ServerName}", serverName)
                .Replace("{DataBaseName}", databaseName ?? defaultDatabaseName);
        }
    }
}