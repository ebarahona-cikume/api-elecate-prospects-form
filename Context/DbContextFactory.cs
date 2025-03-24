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
            bool isSQLAuth = bool.Parse(_configuration["IsSQLAuthenticationFormStructure"] ?? "");
            string defaultFormStructureServerName = _configuration["FormStructureServerName"] ?? "";
            string defaultFormStructureDataBaseName = _configuration["FormStructureDataBaseName"] ?? "";
            string defaultFormStructureUserName = _configuration["FormStructureUserName"] ?? "";
            string defaultFormStructurePassword = _configuration["FormStructurePassword"] ?? "";

            string connectionString = BuildConnectionString(
                isSQLAuth, 
                defaultFormStructureServerName,
                defaultFormStructureDataBaseName,
                defaultFormStructureUserName,
                defaultFormStructurePassword);

            DbContextOptions<ElecateDbContext> options = _sqlServerFactory.CreateDbContextOptions<ElecateDbContext>(connectionString);
            
            return new ElecateDbContext(options);
        }

        public ProspectDbContext CreateProspectDbContext(string defaultSaveInformationDataBaseName)
        {
            bool isSQLAuth = bool.Parse(_configuration["IsSQLAuthenticationSaveInformation"] ?? "");
            string defaultSaveInformationServerName = _configuration["SaveInformationServerName"] ?? "";
            string defaultSaveInformationUserName = _configuration["SaveInformationUserName"] ?? "";
            string defaultSaveInformationPassword = _configuration["SaveInformationPassword"] ?? "";

            string connectionString = BuildConnectionString(
                isSQLAuth, 
                defaultSaveInformationServerName,
                defaultSaveInformationDataBaseName,
                defaultSaveInformationUserName,
                defaultSaveInformationPassword);

            DbContextOptions<ProspectDbContext> options = _sqlServerFactory.CreateDbContextOptions<ProspectDbContext>(connectionString);
            
            return new ProspectDbContext(options);
        }

        public SecretsDbContext CreateSecretsPostgresqlDbContext()
        {
            string connectionString = BuildConnectionString();
            DbContextOptions<SecretsDbContext> options = _postgreSqlFactory.CreateDbContextOptions<SecretsDbContext>(connectionString);
            return new SecretsDbContext(options);
        }

        private string BuildConnectionString(
            bool isSQLAuth, 
            string serverName, 
            string databaseName,
            string userName,
            string password)
        {
            if (isSQLAuth)
            {
                return SQLAuth(serverName, databaseName, userName, password);
            }
            else
            {
                return WindowsAuth(serverName, databaseName);
            }
        }

        private string WindowsAuth(string serverName, string databaseName) 
        {
            string connectionStringTemplate = _configuration.GetConnectionString("WindowsAuthConnection") ?? "";

            return connectionStringTemplate
                .Replace("{ServerName}", serverName)
                .Replace("{DataBaseName}", databaseName);
        }
        private string SQLAuth(
            string serverName, 
            string databaseName, 
            string userName, 
            string password) 
        {
            string connectionStringTemplate = _configuration.GetConnectionString("SQLAuthConnection") ?? "";

            return connectionStringTemplate
                .Replace("{ServerName}", serverName)
                .Replace("{DataBaseName}", databaseName)
                .Replace("{UserName}", userName)
                .Replace("{Password}", password);
        }

        private string BuildConnectionString()
        {
            return _configuration.GetConnectionString("PostgresqlConnection") ?? "";
        }
    }
}