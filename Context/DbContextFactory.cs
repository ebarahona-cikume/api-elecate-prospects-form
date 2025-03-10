using Microsoft.EntityFrameworkCore;

namespace ApiElecateProspectsForm.Context
{
    public class DbContextFactory(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        // Retorna el contexto de la base de datos principal
        public ElecateDbContext CreateElecateDbContext()
        {
            var options = new DbContextOptionsBuilder<ElecateDbContext>()
                .UseSqlServer(_configuration.GetConnectionString("DefaultConnection"))
                .Options;

            return new ElecateDbContext(options);
        }

        // Retorna el contexto de la base de datos dinámica del cliente
        public ProspectDbContext CreateProspectDbContext(string clientDatabaseId)
        {
            string connectionString = GetClientConnectionString(clientDatabaseId);
            return new ProspectDbContext(connectionString);
        }

        private static string GetClientConnectionString(string clientDatabaseId)
        {
            return $"Server=DESK-0068-WIN;Database={clientDatabaseId};Trusted_Connection=True;Encrypt=False;";
        }
    }
}