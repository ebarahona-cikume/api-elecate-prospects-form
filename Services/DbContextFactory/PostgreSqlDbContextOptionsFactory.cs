using ApiElecateProspectsForm.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiElecateProspectsForm.Services.DbContextFactory
{
    public class PostgreSqlDbContextOptionsFactory : IDbContextOptionsFactory
    {
        public DbContextOptions<TContext> CreateDbContextOptions<TContext>(string connectionString) where TContext : DbContext
        {
            DbContextOptionsBuilder<TContext> optionsBuilder = new();
            optionsBuilder.UseNpgsql(connectionString);
            return optionsBuilder.Options;
        }
    }
}
