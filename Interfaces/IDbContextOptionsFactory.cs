using Microsoft.EntityFrameworkCore;

namespace ApiElecateProspectsForm.Interfaces
{
    public interface IDbContextOptionsFactory
    {
        DbContextOptions<TContext> CreateDbContextOptions<TContext>(string connectionString) where TContext : DbContext;
    }
}
