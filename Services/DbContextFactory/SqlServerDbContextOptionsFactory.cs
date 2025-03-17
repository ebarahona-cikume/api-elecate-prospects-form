using ApiElecateProspectsForm.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiElecateProspectsForm.Services.DbContextFactory
{
    public class SqlServerDbContextOptionsFactory : IDbContextOptionsFactory
    {
        public DbContextOptions<TContext> CreateDbContextOptions<TContext>(string connectionString) where TContext : DbContext
        {
            DbContextOptionsBuilder<TContext> optionsBuilder = new();
            optionsBuilder.UseSqlServer(connectionString);
            return optionsBuilder.Options;
        }
    }
}
